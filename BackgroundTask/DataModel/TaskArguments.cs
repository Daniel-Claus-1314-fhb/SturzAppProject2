using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public sealed class TaskArguments
    {
        public TaskArguments()
        {
            ReportInterval = 50;
        }

        public TaskArguments(string measurementId, string filename, uint reportInterval)
        {
            this.MeasurementId = measurementId;
            this.Filename = filename;
            this.ReportInterval = reportInterval;
            this.ProcessedSampleCount = 200;
            this.AccelerometerThreshold = 1.8d;
            this.GyrometerThreshold = 275d;
            this.StepDistance = 300;
        }

        public TaskArguments(string measurementId, string accelerometerFilename, uint reportInterval, uint processedSampleCount, double accelerometerThreshold, 
            double gyrometerThreshold, uint stepDistance)
            : this(measurementId, accelerometerFilename, reportInterval)
        {
            this.ProcessedSampleCount = processedSampleCount;
            this.AccelerometerThreshold = accelerometerThreshold;
            this.GyrometerThreshold = gyrometerThreshold;
            this.StepDistance = stepDistance;
        }

        public string MeasurementId { get; set; }
        public string Filename { get; set; }
        public uint ReportInterval{ get; set; }
        public uint ProcessedSampleCount { get; set; }
        public double AccelerometerThreshold { get; set; }
        public double GyrometerThreshold { get; set; }
        public uint StepDistance { get; set; }
    }
}
