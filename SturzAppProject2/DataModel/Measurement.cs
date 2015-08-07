using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    class Measurement
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public Measurement(String name, MeasurementSetting setting)
        {
            this.Name = name;
            this._id = String.Format("Measurement_{0}", DateTime.Now.Ticks);
            this._accelerometerFilename = String.Format("{0}_Accelerometer", this._id);
            this._gyrometerFilename = String.Format("{0}_Gyrometer", this._id);
            this._setting = setting;
            this._isStarted = false;
            this._isFinished = false;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _accelerometerFilename;
        public string AccelerometerFilename
        {
            get { return _accelerometerFilename; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _gyrometerFilename;
        public string GyrometerFilename
        {
            get { return _gyrometerFilename; }
        }

        /// <summary>
        /// 
        /// </summary>
        private MeasurementSetting _setting;
        public MeasurementSetting Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private MeasurementEvaluation _evaluation;
        public MeasurementEvaluation Evaluation
        {
            get { return _evaluation; }
            set { _evaluation = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
        }
        private bool _isStarted;
        public bool IsStared
        {
            get { return _isStarted; }
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
        }
        private bool _isFinished;
        public bool IsFinished
        {
            get { return _isFinished; }
        }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        public void StartMeasurement()
        {
            this._isStarted = true;
            this._startTime = DateTime.Now;
        }

        public void FinishMeasurement()
        {
            this._isFinished = true;
            this._endTime = DateTime.Now;
        }

        public TimeSpan Duration 
        {
            get 
            {
                if (_isStarted && _isFinished)
                    return _endTime.Subtract(_startTime);
                else if (_isStarted && !_isFinished)
                    return DateTime.Now.Subtract(_startTime);
                else
                    return new TimeSpan(0L);
            }
        }

        #endregion
    }
}
