using System;
using System.Collections.Generic;
using System.Globalization;
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

        public string ToCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:f3},{3:f3}\n", this.MeasurementTime.TotalMilliseconds, this.CoordinateX, this.CoordinateY, this.CoordinateZ);
        }

        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Accelerometer,MeasurementTimeInMilliseconds,CoordinateX,CoordinateY,CoordinateZ\n");
        }

        public string ToExportCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "0,{0},{1:f3},{2:f3},{3:f3}\n", this.MeasurementTime.TotalMilliseconds, this.CoordinateX, this.CoordinateY, this.CoordinateZ);
        }
    }
}
