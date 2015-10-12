using System;

namespace Netflix.Servo.Atlas
{
    public interface ServoAtlasConfig
    {
        /**
         * Return the URI used to POST values to atlas.
         */
        String getAtlasUri();

        /**
         * Return the size of the queue to be used when pushing metrics to
         * the atlas backends. A value of 1000 is quite safe here, but might need
         * to be tweaked if attempting to send hundreds of batches per second.
         */
        int getPushQueueSize();


        /**
         * Whether we should send metrics to atlas. This can be used when running in a dev environment
         * for example to avoid affecting production metrics by dev machines.
         */
        bool shouldSendMetrics();

        /**
         * The maximum size of the batch of metrics to be sent to atlas.
         * If attempting to send more metrics than this value,
         * the {@link AtlasMetricObserver} will split them into batches before sending
         * them to the atlas backends.
         * <p/>
         * A value of 10000 works well for most workloads.
         */
        int batchSize();
    }
}
