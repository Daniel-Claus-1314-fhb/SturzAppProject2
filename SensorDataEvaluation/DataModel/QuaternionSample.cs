
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class QuaternionSample
    {
        public TimeSpan MeasurementTime { get; set; }
        public double AngleW { get; set; }
        public double CoordianteX { get; set; }
        public double CoordianteY { get; set; }
        public double CoordianteZ { get; set; }
    }
}
