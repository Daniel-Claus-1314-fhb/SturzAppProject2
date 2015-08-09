using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class MeasurementList
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public MeasurementList()
        {
            this.Measurements = new List<Measurement>();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        private List<Measurement> _measurements;
        public List<Measurement> Measurements
        {
            get { return _measurements; }
            set { _measurements = value; }
        }

        public Measurement GetById(string id)
        {
            if (id != null)
            {
                foreach (Measurement measurement in this.Measurements)
                {
                    if (measurement.Id.Equals(id))
                    {
                        return measurement;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a certain measurement to the list of measurements.
        /// </summary>
        public void Insert(Measurement measurement)
        {
            this._measurements.Insert(0, measurement);
        }

        public bool Update(Measurement updateMeasurement)
        {
            bool isUpdated = false;

            if (updateMeasurement != null)
            {
                Measurement measurementFromList = GetById(updateMeasurement.Id);
                if (measurementFromList != null)
                {
                    isUpdated = true;
                    // update relevant informations
                    measurementFromList.Name = updateMeasurement.Name;
                    measurementFromList.StartTime = updateMeasurement.StartTime;
                    measurementFromList.EndTime = updateMeasurement.EndTime;
                    measurementFromList.Setting = updateMeasurement.Setting;
                }
            }
            return isUpdated;
        }

        public bool Update(MeasurementViewModel updateMeasurementViewModel)
        {
            bool isUpdated = false;

            if (updateMeasurementViewModel != null)
            {
                Measurement measurementFromList = GetById(updateMeasurementViewModel.Id);
                if (measurementFromList != null)
                {
                    isUpdated = true;
                    // update relevant informations
                    measurementFromList.Name = updateMeasurementViewModel.Name;
                    measurementFromList.StartTime = updateMeasurementViewModel.StartTime;
                    measurementFromList.EndTime = updateMeasurementViewModel.EndTime;
                    //measurementFromList.Setting = updateMeasurementViewModel.Setting;
                }
            }
            return isUpdated;
        }

        /// <summary>
        /// removes a certain measurement from the list of measurements.
        /// </summary>
        public bool Delete(string deleteId)
        {
            bool isDeleted = false;

            if (deleteId != null && deleteId.Length > 0)
            {
                Measurement measurementFromList = GetById(deleteId);
                if (measurementFromList != null)
                {
                    isDeleted = this._measurements.Remove(measurementFromList);
                }
            }
            return isDeleted;
        }
    }
}
