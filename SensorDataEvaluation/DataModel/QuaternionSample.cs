
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public string ToCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:f3},{3:f3},{4:f3}\n", this.MeasurementTime.TotalMilliseconds, this.AngleW, this.CoordinateX, this.CoordinateY, this.CoordinateZ);
        }

        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Quaternion,MeasurementTimeInMilliseconds,AngleW,CoordinateX,CoordinateY,CoordinateZ\n");
        }

        public string ToExportCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "2,{0},{1:f3},{2:f3},{3:f3},{4:f3}\n", this.MeasurementTime.TotalMilliseconds, this.AngleW, this.CoordinateX, this.CoordinateY, this.CoordinateZ);
        }
    }
}
