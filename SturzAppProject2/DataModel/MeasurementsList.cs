using BackgroundTask.DataModel.Setting;
using BackgroundTask.Service;
using BackgroundTask.ViewModel;
using BackgroundTask.ViewModel.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class GlobalMeasurementModel
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public GlobalMeasurementModel()
        {
            this.Measurements = new List<MeasurementModel>();
            this.GlobalSetting = SettingModel.DefaultSettingModel();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        private List<MeasurementModel> _measurements;
        public List<MeasurementModel> Measurements
        {
            get { return _measurements; }
            set { _measurements = value; }
        }

        private SettingModel _globalSetting;
        public SettingModel GlobalSetting
        {
            get { return _globalSetting; }
            set { _globalSetting = value; }
        }

        protected virtual void OnMeasurementListUpdated(EventArgs e)
        {
            if (MeasurementListUpdated != null) { MeasurementListUpdated(this, e); }
        }

        protected virtual void OnGlobalSettingUpdated(EventArgs e)
        {
            if (GlobalSettingUpdated != null) { GlobalSettingUpdated(this, e); }
        }

        public event EventHandler MeasurementListUpdated;
        public event EventHandler GlobalSettingUpdated;
        
        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        public MeasurementModel GetMeasurementById(string id)
        {
            if (id != null)
            {
                foreach (MeasurementModel measurement in this.Measurements)
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
        public void Insert(MeasurementModel measurement)
        {
            this._measurements.Insert(0, measurement);
            OnMeasurementListUpdated(EventArgs.Empty);
        }

        public bool UpdateMeasurementInList(MeasurementViewModel updateMeasurementViewModel)
        {
            bool isUpdated = false;

            if (updateMeasurementViewModel != null)
            {
                MeasurementModel measurementFromList = GetMeasurementById(updateMeasurementViewModel.Id);
                if (measurementFromList != null)
                {
                    // update relevant informations
                    measurementFromList.StartTime = updateMeasurementViewModel.StartTime;
                    measurementFromList.EndTime = updateMeasurementViewModel.EndTime;
                    measurementFromList.MeasurementState = updateMeasurementViewModel.MeasurementState;
                    measurementFromList.TotalSteps = updateMeasurementViewModel.TotalSteps;
                    measurementFromList.Setting = new SettingModel(updateMeasurementViewModel.Setting);

                    isUpdated = true;
                    OnMeasurementListUpdated(EventArgs.Empty);
                }
            }
            return isUpdated;
        }

        /// <summary>
        /// removes a certain measurement from the list of measurements.
        /// </summary>
        public async Task<bool> Delete(string deleteId)
        {
            bool isDeleted = false;

            if (deleteId != null && deleteId.Length > 0)
            {
                MeasurementModel measurementFromList = GetMeasurementById(deleteId);
                if (measurementFromList != null)
                {
                    await FileService.DeleteAllMeasurementFilesAsync(measurementFromList.DataSets, measurementFromList.Filename);
                    isDeleted = this._measurements.Remove(measurementFromList);
                    OnMeasurementListUpdated(EventArgs.Empty);
                }
            }
            return isDeleted;
        }

        public bool UpdateGlobalSetting(SettingViewModel updateSettingViewModel) 
        {
            bool isUpdated = false;

            if (updateSettingViewModel != null)
            {
                this._globalSetting = new SettingModel(updateSettingViewModel);
                isUpdated = true;
                OnGlobalSettingUpdated(EventArgs.Empty);
            }
            return isUpdated;
        }

        public bool UpdateMeasurementSettingById(String id, SettingViewModel updateSettingViewModel)
        {
            bool isUpdated = false;

            if (id != null && updateSettingViewModel != null)
            {
                MeasurementModel measurementFromList = GetMeasurementById(id);
                if (measurementFromList != null)
                {
                    measurementFromList.Setting = new SettingModel(updateSettingViewModel);
                    isUpdated = true;
                    OnMeasurementListUpdated(EventArgs.Empty);
                }
            }
            return isUpdated;
        }

        public async Task<bool> AnalyseDataSetsOfMeasurementByIdAsync(string measurmentId)
        {
            bool isAnalysed = false;

            if (measurmentId != null)
            {
                MeasurementModel measurementFromList = GetMeasurementById(measurmentId);
                if (measurementFromList != null)
                {
                    await measurementFromList.AnalyseMeasurementDataSetsAsync();
                    isAnalysed = true;
                    OnMeasurementListUpdated(EventArgs.Empty);
                }
            }
            return isAnalysed;
        }
    }
}
