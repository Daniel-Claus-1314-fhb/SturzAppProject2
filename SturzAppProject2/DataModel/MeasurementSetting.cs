using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class MeasurementSetting
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region construtors

        public MeasurementSetting()
        {
            this.ReportInterval = 50;
            this.ProcessedSamplesCount = 200;
            this.UseAccelerometer = true;
            this.UseGyrometer = false;
        }

        public MeasurementSetting(MeasurementSettingViewModel measurementSettingViewModel)
        {
            this.ReportInterval = measurementSettingViewModel.ReportInterval;
            this.ProcessedSamplesCount = measurementSettingViewModel.ProcessedSampleCount;
            this.UseAccelerometer = measurementSettingViewModel.UseAccelerometer;
            this.UseGyrometer = measurementSettingViewModel.UseGyrometer;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public uint ReportInterval { get; set; }
        public uint ProcessedSamplesCount { get; set; }
        public bool UseAccelerometer { get; set; }
        public bool UseGyrometer { get; set; }

        #endregion
    }
}
