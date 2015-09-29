using System;
using System.Collections.Generic;
using System.Globalization;
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

        public string ToCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:f3},{3:f3}\n", this.MeasurementTime.TotalMilliseconds, this.VelocityX, this.VelocityY, this.VelocityZ);
        }

        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Gyrometer,MeasurementTimeInMilliseconds,VelocityX,VelocityY,VelocityZ\n");
        }
        
        public string ToExportCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "1,{0},{1:f3},{2:f3},{3:f3}\n", this.MeasurementTime.TotalMilliseconds, this.VelocityX, this.VelocityY, this.VelocityZ);
        }
    }
}
