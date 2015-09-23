
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class QuaternionSample
    {
        public QuaternionSample() { }

        public QuaternionSample(TimeSpan measurementTime, double angleW, double coordinateX, double coordinateY, double coordinateZ) 
        {
            this.MeasurementTime = measurementTime;
            this.AngleW = angleW;
            this.CoordinateX = coordinateX;
            this.CoordinateY = coordinateY;
            this.CoordinateZ = coordinateZ;
        }

        public TimeSpan MeasurementTime { get; set; }
        public double AngleW { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public double CoordinateZ { get; set; }
    }
}
