using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class MeasurementList
    {
        private List<Measurement> _measurements;
        public List<Measurement> Measurements
        {
            get { return _measurements; }
            set { _measurements = value; }
        }

        /// <summary>
        /// Adds a certain measurement to the list of measurements.
        /// </summary>
        public void Add(Measurement measurement)
        {
            this._measurements.Add(measurement);
        }

        /// <summary>
        /// removes a certain measurement from the list of measurements.
        /// </summary>
        public void Remove(Measurement measurement)
        {
            this._measurements.Remove(measurement);
        }
    }
}
