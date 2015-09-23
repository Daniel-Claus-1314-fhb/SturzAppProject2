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

        public EvaluationSample(TimeSpan measurementTime, double accelerometerVectorLength)
        {
            this.MeasurementTime = measurementTime;
            this.AccelerometerVectorLength = accelerometerVectorLength;
            this.IsStepDetected = false;
        }

        public EvaluationSample(TimeSpan measurementTime, double accelerometerVectorLength, bool isStepDetected)
            : this(measurementTime, accelerometerVectorLength)
        {
            this.IsStepDetected = isStepDetected;
        }

        public TimeSpan MeasurementTime { get; set; }
        public double AccelerometerVectorLength { get; set; }
        public bool IsStepDetected { get; set; }
    }
}
