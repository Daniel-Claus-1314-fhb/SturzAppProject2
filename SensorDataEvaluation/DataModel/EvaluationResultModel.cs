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
            this._evaluationResultList = new List<EvaluationSample>();
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################
                
        public uint DetectedSteps
        {
            get { return countDetectedSteps(); }
        }
        
        private List<EvaluationSample> _evaluationResultList;
        public List<EvaluationSample> EvaluationResultList
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
                if (enumerator.Current.IsDetectedStep)
                {
                    detectedStepsCount++;
                }
            }
            return detectedStepsCount;
        }
        
        public string ToEvaluationExportCSVString()
        {
            StringBuilder stringbuilder = new StringBuilder();
            var enumerator = _evaluationResultList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EvaluationSample evaluationSample = enumerator.Current;
                stringbuilder.Append(evaluationSample.ToExportCSVString());
            }
            return stringbuilder.ToString();
        }

        public byte[] ToEvaluationBytes()
        {
            List<byte[]> resultByteArrays = new List<byte[]>();
            var enumerator = _evaluationResultList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                resultByteArrays.Add(enumerator.Current.ToByteArray());
            }
            return resultByteArrays.SelectMany(a => a).ToArray();
        }
    }
}
