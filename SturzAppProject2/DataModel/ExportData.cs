using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class ExportData
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public ExportData()
        {
            this.AccelerometerSamples = new List<AccelerometerSample>();
            this.GyrometerSamples = new List<GyrometerSample>();
            this.QuaternionSamples = new List<QuaternionSample>();
            this.EvaluationSamples = new List<EvaluationSample>();
        }

        public ExportData(List<AccelerometerSample> accelerometerSamples, List<GyrometerSample> gyrometerSamples, List<QuaternionSample> quaternionSamples, List<EvaluationSample> evaluationSamples)
        {
            this.AccelerometerSamples = accelerometerSamples;
            this.GyrometerSamples = gyrometerSamples;
            this.QuaternionSamples = quaternionSamples;
            this.EvaluationSamples = evaluationSamples;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public List<AccelerometerSample> AccelerometerSamples { get; set; }
        public List<GyrometerSample> GyrometerSamples { get; set; }
        public List<QuaternionSample> QuaternionSamples { get; set; }
        public List<EvaluationSample> EvaluationSamples { get; set; }

        #endregion
    }
}
