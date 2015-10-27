using BackgroundTask.DataModel.Setting;
using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class MeasurementModel
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public MeasurementModel() { }

        public MeasurementModel(MeasurementViewModel measurementViewModel)
        {
            this.StartTime = measurementViewModel.StartTime;
            this.EndTime = measurementViewModel.EndTime;
            this.MeasurementState = measurementViewModel.MeasurementState;
            this.TotalSteps = measurementViewModel.TotalSteps;
            this.Setting = new SettingModel(measurementViewModel.Setting);
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        /// <summary>
        /// Id of the measurment.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Filename of the different files data.
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// Settings of the measurement
        /// </summary>
        public SettingModel Setting { get; set; }
        /// <summary>
        /// Enumation which discribes the current state of the measurment
        /// </summary>
        public MeasurementState MeasurementState { get; set; }
        /// <summary>
        /// DateTime of creation
        /// </summary>
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// DateTime of measurement start.
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// DateTime of the end of the measurement.
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Total detected steps during the measurement.
        /// </summary>
        public uint TotalSteps { get; set; }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        public static MeasurementModel NewMeasurementModel(SettingModel settingModel)
        {
            MeasurementModel createMeasurementModel = new MeasurementModel();
            createMeasurementModel.Id = String.Format("{0}", DateTime.Now.Ticks);
            createMeasurementModel.Filename = String.Format("Measurement_{0}.bin", createMeasurementModel.Id);
            createMeasurementModel.CreateDateTime = DateTime.Now;
            if (settingModel != null)
            {
                createMeasurementModel.Setting = settingModel;
            }
            else
            {
                createMeasurementModel.Setting = SettingModel.DefaultSettingModel();
            }
            return createMeasurementModel;
        }
    }
}
