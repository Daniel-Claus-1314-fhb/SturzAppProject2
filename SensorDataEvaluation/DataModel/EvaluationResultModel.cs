﻿using System;
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
                if (enumerator.Current.IsStepDetected)
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
                var currentEvaluation = enumerator.Current;
                stringbuilder.Append(String.Format(new CultureInfo("en-US"), "{0},{1:f3},{2:g}\n",
                    currentEvaluation.MeasurementTime.TotalMilliseconds, currentEvaluation.AccelerometerVectorLength, currentEvaluation.IsStepDetected ? 1 : 0));
            }
            return stringbuilder.ToString();
        }
    }
}
