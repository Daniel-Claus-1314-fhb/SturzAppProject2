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
            ReportInterval = 16;
        }

        public TaskArguments(string measurementId, string accelerometerFilename, string gyrometerFilename, uint reportInterval)
        {
            this.MeasurementId = measurementId;
            this.AccelerometerFilename = accelerometerFilename;
            this.GyrometerFilename = gyrometerFilename;
            this.ReportInterval = reportInterval;
            this.ProcessedSampleCount = 1000;
        }

        public TaskArguments(string measurementId, string accelerometerFilename, string gyrometerFilename, uint reportInterval, uint processedSampleCount)
            : this(measurementId, accelerometerFilename, gyrometerFilename, reportInterval)
        {
            this.ProcessedSampleCount = processedSampleCount;
        }

        public string MeasurementId { get; set; }
        public string AccelerometerFilename { get; set; }
        public string GyrometerFilename { get; set; }
        public uint ReportInterval{ get; set; }
        public uint ProcessedSampleCount { get; set; }
    }
}
