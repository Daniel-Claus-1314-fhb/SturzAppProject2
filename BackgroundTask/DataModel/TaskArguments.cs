using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public sealed class TaskArguments
    {        
        public string MeasurementId { get; set; }
        public string Filename { get; set; }
        // from Base Setting
        public TimeSpan TargetDuration { get; set; }
        public TimeSpan StartOffsetDuration { get; set; }
        // from Evaluation Setting
        public bool IsUsedEvaluation { get; set; }
        public bool IsRecordSamplesEvaluation { get; set; }
        public uint SampleBufferSize { get; set; }
        public double AccelerometerThreshold { get; set; }
        public double GyrometerThreshold { get; set; }
        public uint StepDistance { get; set; }
        public uint PeakJoinDistance { get; set; }
        // from Accelerometer Setting
        public bool IsUsedAccelerometer { get; set; }
        public bool IsRecordSamplesAccelerometer { get; set; }
        public uint ReportIntervalAccelerometer { get; set; }
        // from Gyrometer Setting
        public bool IsUsedGyrometer { get; set; }
        public bool IsRecordSamplesGyrometer { get; set; }
        public uint ReportIntervalGyrometer { get; set; }
        // from Quaternion Setting
        public bool IsUsedQuaternion { get; set; }
        public bool IsRecordSamplesQuaternion { get; set; }
        public uint ReportIntervalQuaternion { get; set; }
        // from Geolocation Setting
        public bool IsUsedGeolocation { get; set; }
        public bool IsRecordSamplesGeolocation { get; set; }
        public uint ReportIntervalGeolocation { get; set; }
    }
}
