using BackgroundTask.DataModel.Setting;
using BackgroundTask.ViewModel.DataSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel.DataSets
{
    public class MeasurementDataSets
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public MeasurementDataSets() { }

        public MeasurementDataSets(DataSetsViewModel dataSetsViewModel)
        {
            this.accelerometerDataSet = new AccelerometerDataSet(dataSetsViewModel.IsAvailableAccelerometer, dataSetsViewModel.TotalCountAccelerometer);
            this.gyrometerDataSet = new GyrometerDataSet(dataSetsViewModel.IsAvailableGyrometer, dataSetsViewModel.TotalCountGyrometer);
            this.quaterionDataSet = new QuaterionDataSet(dataSetsViewModel.IsAvailableQuaternion, dataSetsViewModel.TotalCountQuaternion);
            this.geolocationDataSet = new GeolocationDataSet(dataSetsViewModel.IsAvailableGeolocation, dataSetsViewModel.TotalCountGeolocation);
            this.evaluationDataSet = new EvaluationDataSet(dataSetsViewModel.IsAvailableEvaluation, dataSetsViewModel.TotalCountEvaluation);
        }
        
        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public AccelerometerDataSet accelerometerDataSet { get; set; }
        public GyrometerDataSet gyrometerDataSet { get; set; }
        public QuaterionDataSet quaterionDataSet { get; set; }
        public GeolocationDataSet geolocationDataSet { get; set; }
        public EvaluationDataSet evaluationDataSet { get; set; }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        public static MeasurementDataSets NewDefaultMeasurementDataSets()
        {
            MeasurementDataSets result = new MeasurementDataSets();
            result.accelerometerDataSet = AccelerometerDataSet.NewDefaultDataSet();
            result.gyrometerDataSet = GyrometerDataSet.NewDefaultDataSet();
            result.quaterionDataSet = QuaterionDataSet.NewDefaultDataSet();
            result.geolocationDataSet = GeolocationDataSet.NewDefaultDataSet();
            result.evaluationDataSet = EvaluationDataSet.NewDefaultDataSet();
            return result;
        }

        public async Task AnalyseDataSetsAsync(string filename, SettingModel setting)
        {
            if (setting.IsUsedAccelerometer && setting.IsRecordSamplesAccelerometer)
            {
                await accelerometerDataSet.AnalyseDataSetAsync(filename);
            } 
            if (setting.IsUsedGyrometer && setting.IsRecordSamplesGyrometer)
            {
                await gyrometerDataSet.AnalyseDataSetAsync(filename);
            }
            if (setting.IsUsedQuaternion && setting.IsRecordSamplesQuaternion)
            {
                await quaterionDataSet.AnalyseDataSetAsync(filename);
            }
            if (setting.IsUsedGeolocation && setting.IsRecordSamplesGeolocation)
            {
                await geolocationDataSet.AnalyseDataSetAsync(filename);
            }
            if (setting.IsUsedEvaluation && setting.IsRecordSamplesEvaluation)
            {
                await evaluationDataSet.AnalyseDataSetAsync(filename);
            }
            return;
        }

        #endregion
    }
}
