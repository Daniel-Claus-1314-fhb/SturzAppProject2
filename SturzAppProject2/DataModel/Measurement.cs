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

        public Measurement()
        {
            this.Name = "Unbenannte Messung";
            this._id = String.Format("Measurement_{0}", DateTime.Now.Ticks);
            this._accelerometerFilename = String.Format("Accelerometer_{0}.csv", this._id);
            this._gyrometerFilename = String.Format("Gyrometer_{0}.csv", this._id);
            this.CreateDateTime = DateTime.Now;
            this.Setting = new MeasurementSetting();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public string Name { get; set; }

        private string _id;
        public string Id
        {
            get { return _id; }
        }

        private string _accelerometerFilename;
        public string AccelerometerFilename
        {
            get { return _accelerometerFilename; }
        }

        private string _gyrometerFilename;
        public string GyrometerFilename
        {
            get { return _gyrometerFilename; }
        }

        private MeasurementSetting _setting;
        public MeasurementSetting Setting
        {
            get { return _setting; }
            set { _setting = value; }
        }

        private MeasurementEvaluation _evaluation;
        public MeasurementEvaluation Evaluation
        {
            get { return _evaluation; }
            set { _evaluation = value; }
        }

        private DateTime _createDateTime;
        public DateTime CreateDateTime
        {
            get { return _createDateTime; }
            set { _createDateTime = value; }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods
        

        #endregion
    }
}
