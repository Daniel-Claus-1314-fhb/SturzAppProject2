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
            this.PeakThreshold = 0.3d;
            this.StepDistance = 300;
        }

        public TaskArguments(string measurementId, string accelerometerFilename, uint reportInterval, uint processedSampleCount, double peakThreshold, uint stepDistance)
            : this(measurementId, accelerometerFilename, reportInterval)
        {
            this.ProcessedSampleCount = processedSampleCount;
            this.PeakThreshold = peakThreshold;
            this.StepDistance = stepDistance;
        }

        public string MeasurementId { get; set; }
        public string Filename { get; set; }
        public uint ReportInterval{ get; set; }
        public uint ProcessedSampleCount { get; set; }
        public double PeakThreshold { get; set; }
        public uint StepDistance { get; set; }
    }
}
