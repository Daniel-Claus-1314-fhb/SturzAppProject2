using System;
using System.Collections.Generic;
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
    }
}
