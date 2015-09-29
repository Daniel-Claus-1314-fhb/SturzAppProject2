using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public sealed class TaskArguments
    {
        public TaskArguments(string measurementId, string filename, uint reportInterval, uint processedSampleCount, 
            double accelerometerThreshold, double gyrometerThreshold, uint stepDistance, uint peakJoinDistance)
        {
            this.MeasurementId = measurementId;
            this.Filename = filename;
            this.ReportInterval = reportInterval;
            this.ProcessedSampleCount = processedSampleCount;
            this.AccelerometerThreshold = accelerometerThreshold;
            this.GyrometerThreshold = gyrometerThreshold;
            this.StepDistance = stepDistance;
            this.PeakJoinDistance = peakJoinDistance;
        }

        public string MeasurementId { get; set; }
        public string Filename { get; set; }
        public uint ReportInterval{ get; set; }
        public uint ProcessedSampleCount { get; set; }
        public double AccelerometerThreshold { get; set; }
        public double GyrometerThreshold { get; set; }
        public uint StepDistance { get; set; }
        public uint PeakJoinDistance { get; set; }
    }
}
