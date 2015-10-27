using BackgroundTask.ViewModel.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel.Setting
{
    public class SettingModel
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public SettingModel() { }

        public SettingModel(SettingViewModel settingViewModel)
        {
            if (settingViewModel.BaseSettingViewModel != null) 
            {
                this.Name = settingViewModel.BaseSettingViewModel.Name;
                this.TargetDuration = settingViewModel.BaseSettingViewModel.TargetDuration;
                this.StartOffsetDuration = settingViewModel.BaseSettingViewModel.StartOffsetDuration;
            }

            if (settingViewModel.EvaluationSettingViewModel != null)
            {
                this.IsUsedEvaluation = settingViewModel.EvaluationSettingViewModel.IsUsed;
                this.IsRecordSamplesEvaluation = settingViewModel.EvaluationSettingViewModel.IsRecordSamples;
                this.SampleBufferSize = settingViewModel.EvaluationSettingViewModel.SampleBufferSize;
                this.AccelerometerThreshold = settingViewModel.EvaluationSettingViewModel.AccelerometerThreshold;
                this.GyrometerThreshold = settingViewModel.EvaluationSettingViewModel.GyrometerThreshold;
                this.StepDistance = settingViewModel.EvaluationSettingViewModel.StepDistance;
                this.PeakJoinDistance = settingViewModel.EvaluationSettingViewModel.PeakJoinDistance;
            }

            if (settingViewModel.AccerlerometerSettingViewModel != null)
            {
                this.IsUsedAccelerometer = settingViewModel.AccerlerometerSettingViewModel.IsUsed;
                this.IsRecordSamplesAccelerometer = settingViewModel.AccerlerometerSettingViewModel.IsRecordSamples;
                this.ReportIntervalAccelerometer = settingViewModel.AccerlerometerSettingViewModel.ReportInterval;
            }

            if (settingViewModel.GyrometerSettingViewModel != null)
            {
                this.IsUsedGyrometer = settingViewModel.GyrometerSettingViewModel.IsUsed;
                this.IsRecordSamplesGyrometer = settingViewModel.GyrometerSettingViewModel.IsRecordSamples;
                this.ReportIntervalGyrometer = settingViewModel.GyrometerSettingViewModel.ReportInterval;
            }

            if (settingViewModel.QuaternionSettingViewModel != null)
            {
                this.IsUsedQuaternion = settingViewModel.QuaternionSettingViewModel.IsUsed;
                this.IsRecordSamplesQuaternion = settingViewModel.QuaternionSettingViewModel.IsRecordSamples;
                this.ReportIntervalQuaternion = settingViewModel.QuaternionSettingViewModel.ReportInterval;
            }

            if (settingViewModel.GeolocationSettingViewModel != null)
            {
                this.IsUsedGeolocation = settingViewModel.GeolocationSettingViewModel.IsUsed;
                this.IsRecordSamplesGeolocation = settingViewModel.GeolocationSettingViewModel.IsRecordSamples;
                this.ReportIntervalGeolocation = settingViewModel.GeolocationSettingViewModel.ReportInterval;
            }
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        // from Base Setting
        public string Name { get; set; }
        public TimeSpan TargetDuration { get; set; }
        public TimeSpan StartOffsetDuration { get; set; }

        // from Evaluation Setting
        public bool IsUsedEvaluation { get; set; }
        public bool IsRecordSamplesEvaluation { get; set; }
        public uint SampleBufferSize { get; set; }
        public double AccelerometerThreshold { get; set; }
        public double GyrometerThreshold { get; set; }
        public uint StepDistance { get; set; }
        public uint PeakJoinDistance { get; set; }

        // from Accelerometer Setting
        public bool IsUsedAccelerometer { get; set; }
        public bool IsRecordSamplesAccelerometer { get; set; }
        public uint ReportIntervalAccelerometer { get; set; }

        // from Gyrometer Setting
        public bool IsUsedGyrometer { get; set; }
        public bool IsRecordSamplesGyrometer { get; set; }
        public uint ReportIntervalGyrometer { get; set; }

        // from Quaternion Setting
        public bool IsUsedQuaternion { get; set; }
        public bool IsRecordSamplesQuaternion { get; set; }
        public uint ReportIntervalQuaternion { get; set; }

        // from Geolocation Setting
        public bool IsUsedGeolocation { get; set; }
        public bool IsRecordSamplesGeolocation { get; set; }
        public uint ReportIntervalGeolocation { get; set; }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        public static SettingModel DefaultSettingModel()
        {
            SettingModel defaultSettingModel = new SettingModel();
            defaultSettingModel.Name = "NeueMessung";
            defaultSettingModel.TargetDuration = TimeSpan.MinValue;
            defaultSettingModel.StartOffsetDuration = TimeSpan.MinValue;
            defaultSettingModel.IsUsedEvaluation = true;
            defaultSettingModel.IsRecordSamplesEvaluation = true;
            defaultSettingModel.SampleBufferSize = 500;
            defaultSettingModel.AccelerometerThreshold = 1.5d;
            defaultSettingModel.GyrometerThreshold = 150d;
            defaultSettingModel.StepDistance = 300;
            defaultSettingModel.PeakJoinDistance = 150;
            defaultSettingModel.IsUsedAccelerometer = true;
            defaultSettingModel.IsRecordSamplesAccelerometer = true;
            defaultSettingModel.ReportIntervalAccelerometer = 20;
            defaultSettingModel.IsUsedGyrometer = true;
            defaultSettingModel.IsRecordSamplesGyrometer = true;
            defaultSettingModel.ReportIntervalGyrometer = 20;
            defaultSettingModel.IsUsedQuaternion = false;
            defaultSettingModel.IsRecordSamplesQuaternion = true;
            defaultSettingModel.ReportIntervalQuaternion = 20;
            defaultSettingModel.IsUsedGeolocation = false;
            defaultSettingModel.IsRecordSamplesGeolocation = true;
            defaultSettingModel.ReportIntervalGeolocation = 20;
            return defaultSettingModel;
        }
    }
}
