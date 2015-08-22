using BackgroundTask.Service;
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

        protected virtual void OnMeasurementListUpdated(EventArgs e)
        {
            EventHandler handler = MeasurementListUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler MeasurementListUpdated;




        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

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
            OnMeasurementListUpdated(EventArgs.Empty);
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
                    measurementFromList.MeasurementState = updateMeasurementViewModel.MeasurementState;
                    measurementFromList.Setting = new MeasurementSetting(updateMeasurementViewModel.MeasurementSetting);
                    OnMeasurementListUpdated(EventArgs.Empty);
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
                    FileService.DeleteAccelerometerMeasurementAsync(measurementFromList.AccelerometerFilename);
                    FileService.DeleteAccelerometerEvaluationAsync(measurementFromList.AccelerometerFilename);
                    FileService.DeleteGyrometerMeasurementAsync(measurementFromList.GyrometerFilename);
                    isDeleted = this._measurements.Remove(measurementFromList);
                    OnMeasurementListUpdated(EventArgs.Empty);
                }
            }
            return isDeleted;
        }
    }
}
