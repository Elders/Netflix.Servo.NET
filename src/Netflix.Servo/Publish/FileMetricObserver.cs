using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Netflix.Servo.Util;
using slf4net;

namespace Netflix.Servo.Publish
{
    /**
 * Writes observations to a file. The format is a basic text file with tabs
 * separating the fields.
 */
    public class FileMetricObserver : BaseMetricObserver
    {
        private static ILogger LOGGER = LoggerFactory.GetLogger(typeof(FileMetricObserver));

        private static String FILE_DATE_FORMAT = "yyyy_dd_MM_HH_mm_ss_SSS";
        private string dir;
        private bool compress;
        private Clock clock;
        //private SimpleDateFormat fileFormat;

        /**
         * Creates a new instance that stores files in {@code dir} with a prefix of
         * {@code name} and a suffix of a timestamp in the format
         * {@code yyyy_dd_MM_HH_mm_ss_SSS}.
         *
         * @param name name to use as a prefix on files
         * @param dir  directory where observations will be stored
         */
        public FileMetricObserver(String name, string dir)
            : this(name, dir, false)
        {
        }

        /**
         * Creates a new instance that stores files in {@code dir} with a prefix of
         * {@code name} and a suffix of a timestamp in the format
         * {@code yyyy_dd_MM_HH_mm_ss_SSS}.
         *
         * @param name     name to use as a prefix on files
         * @param dir      directory where observations will be stored
         * @param compress whether to compress our output
         */
        public FileMetricObserver(String name, string dir, bool compress)
            : this(name,
                String.Format("'%s'_%s", name, FILE_DATE_FORMAT) + (compress ? "'.log.gz'" : "'.log'"),
                dir,
                compress)
        {
        }

        /**
         * Creates a new instance that stores files in {@code dir} with a name that
         * is created using {@code namePattern}.
         *
         * @param name        name of the observer
         * @param namePattern date format pattern used to create the file names
         * @param dir         directory where observations will be stored
         * @param compress    whether to compress our output
         */
        public FileMetricObserver(String name, String namePattern, string dir, bool compress)
            : this(name, namePattern, dir, compress, ClockWithOffset.INSTANCE)
        {
        }

        /**
         * Creates a new instance that stores files in {@code dir} with a name that
         * is created using {@code namePattern}.
         *
         * @param name        name of the observer
         * @param namePattern date format pattern used to create the file names
         * @param dir         directory where observations will be stored
         * @param compress    whether to compress our output
         * @param clock       clock instance to use for getting the time used in the filename
         */

        public FileMetricObserver(String name, String namePattern, string dir,
                                  bool compress, Clock clock)
            : base(name)
        {
            this.dir = dir;
            this.compress = compress;
            this.clock = clock;
        }

        /**
         * {@inheritDoc}
         */
        public override void updateImpl(List<Metric> metrics)
        {
            Preconditions.checkNotNull(metrics, "metrics");
            var builder = new StringBuilder();
            foreach (var m in metrics)
            {
                builder.Append(m.getConfig().getName());
                builder.Append('\t');
                builder.Append(m.getConfig().getTags().ToString());
                builder.Append('\t');
                builder.Append(m.getValue().ToString());
                builder.Append('\n');
            }

            using (var memoryStream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(builder.ToString());

                if (compress)
                {
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                    {
                        gZipStream.Write(data, 0, data.Length);

                        using (var fileStream = File.Create(Path.Combine(dir, clock.now().ToString())))
                        {
                            byte[] bytesInStream = new byte[gZipStream.Length];
                            gZipStream.Read(bytesInStream, 0, bytesInStream.Length);
                            fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                        }
                    }
                }
                else
                {
                    memoryStream.Write(data, 0, data.Length);

                    using (var fileStream = File.Create(Path.Combine(dir, clock.now().ToString())))
                    {
                        byte[] bytesInStream = new byte[memoryStream.Length];
                        memoryStream.Read(bytesInStream, 0, bytesInStream.Length);
                        fileStream.Write(bytesInStream, 0, bytesInStream.Length);
                    }
                }
            }
            //File file = new File(dir fileFormat.format(new Date(clock.now())));
            //    Writer out = null;
            //    try
            //    {
            //        try
            //        {
            //            LOGGER.Debug("writing {} metrics to file {}", metrics.Count, file);
            //            OutputStream fileOut = new FileOutputStream(file, true);
            //            if (compress)
            //            {
            //                fileOut = new GZIPOutputStream(fileOut);
            //            }
            //out = new OutputStreamWriter(fileOut, "UTF-8");
            //            for (Metric m : metrics)
            //            {
            //  out.append(m.getConfig().getName()).append('\t')
            //      .append(m.getConfig().getTags().ToString()).append('\t')
            //      .append(m.getValue().ToString()).append('\n');
            //            }
            //        }
            //        catch (Exception t)
            //        {
            //            if (out != null) {
            //  out.close();
            //  out = null;
            //            }
            //            throw;
            //        }
            //        finally
            //        {
            //            if (out != null) {
            //  out.close();
            //            }
            //        }
            //    }
            //    catch (IOException e)
            //    {
            //        incrementFailedCount();
            //        LOGGER.Error("failed to write update to file " + file, e);
            //    }
        }
    }
}
