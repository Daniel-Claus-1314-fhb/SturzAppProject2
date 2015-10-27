using BackgroundTask.DataModel.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.ViewModel.Setting
{
    public class SettingViewModel
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors
        
        public SettingViewModel(SettingModel settingModel) {

            this.BaseSettingViewModel = new BaseSettingViewModel(settingModel.Name, settingModel.TargetDuration, settingModel.StartOffsetDuration);
            this.EvaluationSettingViewModel = new EvaluationSettingViewModel(settingModel.IsUsedEvaluation, settingModel.IsRecordSamplesEvaluation, settingModel.SampleBufferSize, settingModel.AccelerometerThreshold, settingModel.GyrometerThreshold, settingModel.StepDistance, settingModel.PeakJoinDistance);
            this.AccerlerometerSettingViewModel = new AccelerometerSettingViewModel(settingModel.IsUsedAccelerometer, settingModel.IsRecordSamplesAccelerometer, settingModel.ReportIntervalAccelerometer);
            this.GyrometerSettingViewModel = new GyrometerSettingViewModel(settingModel.IsUsedGyrometer, settingModel.IsRecordSamplesGyrometer, settingModel.ReportIntervalGyrometer);
            this.QuaternionSettingViewModel = new QuaternionSettingViewModel(settingModel.IsUsedQuaternion, settingModel.IsRecordSamplesQuaternion, settingModel.ReportIntervalQuaternion);
            this.GeolocationSettingViewModel = new GeolocationSettingViewModel(settingModel.IsUsedGeolocation, settingModel.IsRecordSamplesGeolocation, settingModel.ReportIntervalGeolocation);
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public BaseSettingViewModel BaseSettingViewModel { get; set; }

        public EvaluationSettingViewModel EvaluationSettingViewModel { get; set; }

        public AccelerometerSettingViewModel AccerlerometerSettingViewModel { get; set; }
        public GyrometerSettingViewModel GyrometerSettingViewModel { get; set; }
        public QuaternionSettingViewModel QuaternionSettingViewModel { get; set; }
        public GeolocationSettingViewModel GeolocationSettingViewModel { get; set; }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        #endregion
    }
}
