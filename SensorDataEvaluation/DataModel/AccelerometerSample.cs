using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class AccelerometerSample
    {
        public AccelerometerSample() { }

        public AccelerometerSample(TimeSpan measurementTime, double coordinateX, double coordinateY, double coordinateZ)
        {
            this.MeasurementTime = measurementTime;
            this.CoordinateX = coordinateX;
            this.CoordinateY = coordinateY;
            this.CoordinateZ = coordinateZ;
        }

        public TimeSpan MeasurementTime { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public double CoordinateZ { get; set; }
    }
}
