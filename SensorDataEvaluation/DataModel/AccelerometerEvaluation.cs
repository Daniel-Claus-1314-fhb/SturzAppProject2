using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class AccelerometerEvaluation
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public AccelerometerEvaluation(string filename, uint processingListCount)
        {
            this._filename = filename;
            this._processingListCount = processingListCount;
            this.StepThreshold = 0.3d;
            this.StepDistance = new TimeSpan(TimeSpan.TicksPerMillisecond * 300);

            this._accelerometerAnalysisList = new List<object[]>();
            this._accelerometerEvaluationList = new List<object[]>();
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        private string _filename;
        public string Filename
        {
            get { return _filename; }
        }
        
        private uint _processingListCount { get; set; }
        public double StepThreshold { get; private set; }
        public TimeSpan StepDistance { get; private set; }

        private uint _totalSteps;
        public uint  TotalSteps
        {
            get { return _totalSteps; }
            set { _totalSteps = value; }
        }
        public uint AddTotalSteps
        {
            set { _totalSteps += value; }
        }

        /// <summary>
        /// List of accelerometer tuples which will analysed.
        /// 
        /// object[0]: timespan since the start of measurement. (TimeSpan)
        /// object[1]: X coordinate of the accelerometer tuple. (double)
        /// object[2]: Y coorfinate of the accelerometer tuple. (double)
        /// object[3]: Z coordinate of the accelerometer tuple. (double)
        /// object[4]: is accelerometer tuple already processed by analysis. (bool)
        /// </summary>
        private List<object[]> _accelerometerAnalysisList;
        public List<object[]> AccelerometerAnalysisList
        {
            get { return _accelerometerAnalysisList; }
            set { _accelerometerAnalysisList = value; }
        }

        /// <summary>
        /// List of accelerometer vector lengths. 
        /// Represents the result of analysis.
        /// 
        /// object[0]: timespan since the start of measurement. (TimeSpan)
        /// object[1]: length of the accelerometer vector. (double)
        /// object[2]: is new step detected at this certain position. (bool)
        /// </summary>
        private List<object[]> _accelerometerEvaluationList;
        public List<object[]> AccelerometerEvaluationList
        {
            get { return _accelerometerEvaluationList; }
            set { _accelerometerEvaluationList = value; }
        }

        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################
        
        public void AddAccelerometerDataForAnalysis(List<Tuple<TimeSpan, double, double, double>> accelerometerTuples)
        {
            if (accelerometerTuples != null)
            {
                if (_accelerometerAnalysisList.Count > 1)
                {
                    _accelerometerAnalysisList.RemoveRange(0, _accelerometerAnalysisList.Count - 2);
                    _accelerometerEvaluationList.Clear();
                }

                foreach (Tuple<TimeSpan, double, double, double> accelerometerTuple in accelerometerTuples)
                {
                    TimeSpan timeSpan = accelerometerTuple.Item1;
                    double accelerometerX = accelerometerTuple.Item2;
                    double accelerometerY = accelerometerTuple.Item3;
                    double accelerometerZ = accelerometerTuple.Item4;
                    bool isAnalysed = false;
                    _accelerometerAnalysisList.Add(new object[5] { timeSpan, accelerometerX, accelerometerY, accelerometerZ, isAnalysed });
                }
            }
        }
    }
}
