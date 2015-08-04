using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SturzAppProject2.DataModel
{
    class Measurement
    {
        public Measurement(String name, MeasurementSetting setting)
        {
            this.Name = name;
            this._measurementId = String.Format("Measurement_{0}", DateTime.Now.Ticks);
            this._accelerometerId = String.Format("{0}_Accelerometer", this._measurementId);
            this._gyrometerId = String.Format("{0}_Gyrometer", this._measurementId);
            this._setting = setting;
            this._isStarted = false;
            this._isFinished = false;
        }
        
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _measurementId;
        public string MeasurementId
        {
            get { return _measurementId; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _accelerometerId;
        public string AccelerometerId
        {
            get { return _accelerometerId; }
        }

        /// <summary>
        /// 
        /// </summary>
        private string _gyrometerId;
        public string Gyrometer
        {
            get { return _gyrometerId; }
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
        private DateTime _startDateTime;
        public DateTime StartDateTime
        {
            get { return _startDateTime; }
        }
        private bool _isStarted;
        public bool IsStared
        {
            get { return _isStarted; }
        }
        public void StartMeasurement()
        {
            this._isStarted = true;
            this._startDateTime = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        private DateTime _finishDateTime;
        public DateTime FinishDateTime
        {
            get { return _finishDateTime; }
        }
        private bool _isFinished;
        public bool IsFinished
        {
            get { return _isFinished; }
        }
        public void FinishMeasurement()
        {
            this._isFinished = true;
            this._finishDateTime = DateTime.Now;
        }

        public TimeSpan Duration 
        {
            get 
            {
                if (_isStarted && _isFinished)
                    return _finishDateTime.Subtract(_startDateTime);
                else if (_isStarted && !_isFinished)
                    return DateTime.Now.Subtract(_startDateTime);
                else
                    return new TimeSpan(0L);
            }
        }
    }
}
