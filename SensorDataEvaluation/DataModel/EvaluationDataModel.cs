using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class EvaluationDataModel
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public EvaluationDataModel()
        {
            this.AccelerometerSampleAnalysisList = new List<AccelerometerSample>();
            this.GyrometerSampleAnalysisList = new List<GyrometerSample>();
            this.QuaternionSampleAnalysisList = new List<QuaternionSample>();
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        /// <summary>
        /// List of accelerometer tuples which will analysed.
        /// </summary>
        public List<AccelerometerSample> AccelerometerSampleAnalysisList;

        /// <summary>
        /// List of gyrometer tuples which will analysed.
        /// </summary>
        public List<GyrometerSample> GyrometerSampleAnalysisList;

        /// <summary>
        /// List of quaternion tuples which will analysed.
        /// </summary>
        public List<QuaternionSample> QuaternionSampleAnalysisList;


        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################
        
        public void AddAllAccelerometerAnalysisFromSampleList(List<AccelerometerSample> accelerometerSampleList)
        {
            this.AccelerometerSampleAnalysisList.Clear();
            this.AccelerometerSampleAnalysisList.AddRange(accelerometerSampleList);
        }

        public void AddAllGyrometerAnalysisFromSampleList(List<GyrometerSample> gyrometerSampleList)
        {
            this.GyrometerSampleAnalysisList.Clear();
            this.GyrometerSampleAnalysisList.AddRange(gyrometerSampleList);
        }

        public void AddAllQuaternionAnalysisFromSampleList(List<QuaternionSample> quaternionSampleList)
        {
            this.QuaternionSampleAnalysisList.Clear();
            this.QuaternionSampleAnalysisList.AddRange(quaternionSampleList);
        }
    }
}
