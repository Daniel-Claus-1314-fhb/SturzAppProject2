using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class EvaluationResultModel
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public EvaluationResultModel()
        {
            this._evaluationResultList = new List<object[]>();
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################
                
        public uint DetectedSteps
        {
            get { return countDetectedSteps(); }
        }

        /// <summary>
        /// List of accelerometer vector lengths. 
        /// Represents the result of analysis.
        /// 
        /// object[0]: timespan since the start of measurement. (TimeSpan)
        /// object[1]: length of the accelerometer vector. (double)
        /// object[2]: is new step detected at this certain position. (bool)
        /// </summary>
        private List<object[]> _evaluationResultList;
        public List<object[]> EvaluationResultList
        {
            get { return _evaluationResultList; }
            set { _evaluationResultList = value; }
        }

        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        private uint countDetectedSteps()
        {
            uint detectedStepsCount = 0;

            var enumerator = _evaluationResultList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current[2].GetType().Equals(typeof(bool))
                    && (bool)enumerator.Current[2])
                {
                    detectedStepsCount++;
                }
            }
            return detectedStepsCount;
        }
        
        public string ToEvaluationResultCSVString()
        {
            StringBuilder stringbuilder = new StringBuilder();
            var enumerator = _evaluationResultList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentAccelerometerEvaluation = enumerator.Current;
                stringbuilder.Append(String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:g}\n",
                    ((TimeSpan)currentAccelerometerEvaluation[0]).TotalMilliseconds, (double)currentAccelerometerEvaluation[1], (bool)currentAccelerometerEvaluation[2] ? 1 : 0));
            }
            return stringbuilder.ToString();
        }
    }
}
