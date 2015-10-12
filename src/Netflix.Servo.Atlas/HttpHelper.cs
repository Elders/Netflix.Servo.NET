using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Java.Util.Concurrent;
using Java.Util.Concurrent.Atomic;
using Newtonsoft.Json;
using slf4net;

namespace Netflix.Servo.Atlas
{
    /**
  * Helper class to make http requests using rxhttp. For internal use of servo only.
  */
    //@Singleton
    public class HttpHelper
    {
        private static ILogger LOGGER = LoggerFactory.GetLogger(typeof(HttpHelper));
        private static String SMILE_CONTENT_TYPE = "application/x-jackson-smile";

        private RxHttp rxHttp;

        /**
         * An HTTP Response. For internal use of servo only.
         */
        public class Response
        {
            public int status;
            public byte[] body;
            public NameValueCollection headers;

            /**
             * Get the HTTP status code.
             */
            public int getStatus()
            {
                return status;
            }

            /**
             * Get the body of the response as a byte array.
             */
            public byte[] getBody()
            {
                var temp = new List<byte>(body);

                return temp.ToArray();
            }

            /**
             * Get the rxnetty {@link HttpResponseHeaders} for this response.
             */
            public NameValueCollection getHeaders()
            {
                return headers;
            }
        }

        /**
         * Create a new HttpHelper using the given {@link RxHttp} instance.
         */
        //@Inject
        public HttpHelper(RxHttp rxHttp)
        {
            this.rxHttp = rxHttp;
        }

        /**
         * Get the underlying {@link RxHttp} instance.
         */
        public RxHttp getRxHttp()
        {
            return rxHttp;
        }

        /**
         * POST to the given URI the passed {@link JsonPayload}.
         */
        public ObservableCollection<HttpResponse>
        postSmile(String uriStr, JsonPayload payload)
        {
            byte[] entity = toByteArray(payload);
            Uri uri;
            Uri.TryCreate(uriStr, UriKind.RelativeOrAbsolute, out uri);
            return rxHttp.post(uri, SMILE_CONTENT_TYPE, entity);
        }

        private byte[] toByteArray(JsonPayload payload)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (JsonTextWriter gen = new JsonTextWriter(sw))
                    {
                        gen.Formatting = Formatting.Indented;
                        payload.toJson(gen);

                        return Encoding.UTF8.GetBytes(sw.ToString());
                    }
                }

            }
            catch (IOException e)
            {
                throw;
            }
        }

        private void logErr(String prefix, Exception e, int sent, int total)
        {
            if (LOGGER.IsWarnEnabled)
            {
                Exception cause = e.InnerException != null ? e.InnerException : e;
                String msg = String.Format("%s exception %s:%s Sent %d/%d",
                    prefix,
                    cause.Source, cause.Message,
                    sent, total);
                LOGGER.Warn(msg);

                var inner = cause.InnerException;

                while (inner != null)
                {
                    LOGGER.Warn(" Exception {}: {}", inner.Source, inner.Message);
                    inner = inner.InnerException;
                }
            }
        }

        /**
         * Attempt to send all the batches totalling numMetrics in the allowed time.
         *
         * @return The total number of metrics sent.
         */
        public int sendAll(List<ObservableCollection<int>> batches,
                           int numMetrics, long timeoutMillis)
        {
            AtomicBoolean err = new AtomicBoolean(false);
            AtomicInteger updated = new AtomicInteger(0);
            LOGGER.Debug("Got {} ms to send {} metrics", timeoutMillis, numMetrics);
            try
            {
                CountdownEvent completed = new CountdownEvent(1);

                Subscription s = Observable.mergeDelayError(Observable.from(batches))
                    .timeout(timeoutMillis, TimeUnit.MILLISECONDS)
                    .subscribeOn(Schedulers.immediate())
                    .subscribe(updated::addAndGet, exc =>
                    {
                        logErr("onError caught", exc, updated.GetAndSet(updated.Value), numMetrics);
                        err.GetAndSet(true);
                        completed.countDown();
                    }, completed::countDown);
                try
                {
                    completed.Wait((int)timeoutMillis);
                }
                catch (CancellationException interrupted)
                {
                    err.GetAndSet(true);
                    s.unsubscribe();
                    LOGGER.Warn("Timed out sending metrics. {}/{} sent", updated.GetAndSet(updated.Value), numMetrics);
                }
            }
            catch (Exception e)
            {
                err.GetAndSet(true);
                logErr("Unexpected ", e, updated.GetAndSet(updated.Value), numMetrics);
            }

            if (updated.GetAndSet(updated.Value) < numMetrics && !err.GetAndSet(err.Value))
            {
                LOGGER.Warn("No error caught, but only {}/{} sent.", updated.GetAndSet(updated.Value), numMetrics);
            }
            return updated.GetAndSet(updated.Value);
        }

        /**
         * Perform an HTTP get in the allowed time.
         */
        public Response get(HttpRequest req, long timeout, TimeUnit timeUnit)
        {
            String uri = req.RawUrl;
            Response result = new Response();
            try
            {
                Func<HttpResponse, ObservableCollection<byte[]>> process = response =>
                {
                    result.status = response.StatusCode;
                    result.headers = response.Headers;
                    Func<ByteArrayOutputStream, ByteBuf, ByteArrayOutputStream>
                         accumulator = (baos, bb) =>
                         {
                             try
                             {
                                 bb.readBytes(baos, bb.readableBytes());
                             }
                             catch (IOException e)
                             {
                                 throw;
                             }
                             return baos;
                         };
                    return response.getContent()
                        .reduce(new ByteArrayOutputStream(), accumulator)
                        .map(ByteArrayOutputStream::toByteArray);
                };

                result.body = rxHttp.submit(req)
                    .flatMap(process)
                    .subscribeOn(Schedulers.io())
                    .toBlocking()
                    .toFuture()
                    .get(timeout, timeUnit);
                return result;
            }
            catch (Exception e)
            {
                throw new RuntimeException("failed to get url: " + uri, e);
            }
        }
    }
}
