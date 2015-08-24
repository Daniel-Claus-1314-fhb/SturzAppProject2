using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class Measurement
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        /// <summary>
        /// Creates a default measurement.
        /// </summary>
        public Measurement()
        {
            this.Name = "Neue_Messung";
            this.Id = String.Format("{0}", DateTime.Now.Ticks);
            this.AccelerometerFilename = String.Format("Accelerometer_{0}.csv", this._id);
            this.CreateDateTime = DateTime.Now;
            this.Setting = new MeasurementSetting();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        /// <summary>
        /// Name of the measurment.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of the measurment.
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Filename of the accelerometer data.
        /// </summary>
        private string _accelerometerFilename;
        public string AccelerometerFilename
        {
            get { return _accelerometerFilename; }
            set { _accelerometerFilename = value; }
        }
        /// <summary>
        /// Settings of the measurement
        /// </summary>
        private MeasurementSetting _setting;
        public MeasurementSetting Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }
        /// <summary>
        /// Enumation which discribes the current state of the measurment
        /// </summary>
        private MeasurementState _measurementState;
        public MeasurementState MeasurementState
        {
            get { return _measurementState; }
            set { _measurementState = value; }
        }
        /// <summary>
        /// DateTime of creation
        /// </summary>
        private DateTime _createDateTime;
        public DateTime CreateDateTime
        {
            get { return _createDateTime; }
            set { _createDateTime = value; }
        }
        /// <summary>
        /// DateTime of measurement start.
        /// </summary>
        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }
        /// <summary>
        /// DateTime of the end of the measurement.
        /// </summary>
        private DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }
        /// <summary>
        /// Total detected steps during the measurement.
        /// </summary>
        private uint _totalSteps;
        public uint TotalSteps
        {
            get { return _totalSteps; }
            set { _totalSteps = value; }
        }

        #endregion
    }
}
