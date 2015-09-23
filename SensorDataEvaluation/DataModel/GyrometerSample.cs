using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class GyrometerSample
    {
        public GyrometerSample() { }

        public GyrometerSample(TimeSpan measurementTime, double velocityX, double velocityY, double velocityZ) 
        {
            this.MeasurementTime = measurementTime;
            this.VelocityX = velocityX;
            this.VelocityY = velocityY;
            this.VelocityZ = velocityZ;
        }

        public TimeSpan MeasurementTime { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public double VelocityZ { get; set; }
    }
}
