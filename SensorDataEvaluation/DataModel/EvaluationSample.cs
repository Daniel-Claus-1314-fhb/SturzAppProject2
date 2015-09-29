using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class EvaluationSample
    {
        public EvaluationSample() { }

        public EvaluationSample(TimeSpan measurementTime, double accelerometerVectorLength, double gyrometerVectorLength)
        {
            this.MeasurementTime = measurementTime;
            this.AccelerometerVectorLength = accelerometerVectorLength;
            this.GyrometerVectorLength = gyrometerVectorLength;
            this.IsDetectedStep = false;
        }

        public EvaluationSample(TimeSpan measurementTime, double accelerometerVectorLength, double gyrometerVectorLength, 
            bool isAssumedAccelerometerStep, bool isAssumedGyrometerStep, bool isDetectedStep)
            : this(measurementTime, accelerometerVectorLength, gyrometerVectorLength)
        {
            this.IsAssumedAccelerometerStep = isAssumedAccelerometerStep;
            this.IsAssumedGyrometerStep = isAssumedGyrometerStep;
            this.IsDetectedStep = isDetectedStep;
        }

        public TimeSpan MeasurementTime { get; set; }
        public double AccelerometerVectorLength { get; set; }
        public double GyrometerVectorLength { get; set; }
        public bool IsAssumedAccelerometerStep { get; set; }
        public bool IsAssumedGyrometerStep { get; set; }
        public bool IsDetectedStep { get; set; }

        public string ToCSVString()
        {
            return String.Format(String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:f3},{3:g},{4:g},{5:g}\n",
                    this.MeasurementTime.TotalMilliseconds, this.AccelerometerVectorLength, this.GyrometerVectorLength,
                    this.IsAssumedAccelerometerStep ? 1 : 0, this.IsAssumedGyrometerStep ? 1 : 0, this.IsDetectedStep ? 1 : 0));
        }

        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Evaluation,MeasurementTimeInMilliseconds,AccelerometerVectorLength,GyrometerVectorLength,AccelerometerPeak,GyrometerPeak,DetectedStep\n");
        }

        public string ToExportCSVString()
        {
            return String.Format(String.Format(new CultureInfo("en-US"), "3,{0},{1:f3},{2:f3},{3:g},{4:g},{5:g}\n",
                    this.MeasurementTime.TotalMilliseconds, this.AccelerometerVectorLength, this.GyrometerVectorLength,
                    this.IsAssumedAccelerometerStep ? 1 : 0, this.IsAssumedGyrometerStep ? 1 : 0, this.IsDetectedStep ? 1 : 0));
        }
    }
}
