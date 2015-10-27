using BackgroundTask.DataModel;
using BackgroundTask.ViewModel.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System.Threading;
using Windows.UI.Core;

namespace BackgroundTask.ViewModel
{

    public class MeasurementViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public MeasurementViewModel()
        {
            this.MeasurementState = MeasurementState.Initialized;
        }

        /// <summary>
        /// Constructor to create a new measurementViewModel by a given measurementModel.
        /// </summary>
        /// <param name="measurement"></param>
        public MeasurementViewModel(MeasurementModel measurement) : this()
        {
            this.Id = measurement.Id;
            this.CreateDateTime = measurement.CreateDateTime;
            this.StartTime = measurement.StartTime;
            this.EndTime = measurement.EndTime;
            this.MeasurementState = measurement.MeasurementState;
            this.Setting = new SettingViewModel(measurement.Setting);
            this.TotalSteps = measurement.TotalSteps;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        /// <summary>
        /// Id of the measurment.
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }
        /// <summary>
        /// Settings of the measurement
        /// </summary>
        private SettingViewModel _setting;
        public SettingViewModel Setting
        {
            get { return _setting; }
            set { this.SetProperty(ref this._setting, value); }
        }
        /// <summary>
        /// Enumation which discribes the current state of the measurment
        /// </summary>
        private MeasurementState _measurementState;
        public MeasurementState MeasurementState
        {
            get { return _measurementState; }
            set { this.SetProperty(ref this._measurementState, value); }
        }
        /// <summary>
        /// DateTime of creation
        /// </summary>
        private DateTime _createDateTime;
        public DateTime CreateDateTime
        {
            get { return _createDateTime; }
            set { this.SetProperty(ref this._createDateTime, value); }
        }
        /// <summary>
        /// DateTime of measurement start.
        /// </summary>
        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set { this.SetProperty(ref this._startTime, value); }
        }
        /// <summary>
        /// DateTime of the end of the measurement.
        /// </summary>
        private DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
            set { this.SetProperty(ref this._endTime, value); }
        }
        /// <summary>
        /// Duration of the measurment. Will calucated by startTime and endTime.
        /// </summary>
        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get 
            {
                TimeSpan currentTimeSpan = new TimeSpan(0L);

                if (this.StartTime.CompareTo(DateTime.MinValue) != 0 &&
                    this.EndTime.CompareTo(DateTime.MinValue) != 0)
                {
                    currentTimeSpan = _endTime.Subtract(_startTime);
                }
                else if (this.StartTime.CompareTo(DateTime.MinValue) != 0 &&
                    this.EndTime.CompareTo(DateTime.MinValue) == 0)
                {
                    currentTimeSpan = DateTime.Now.Subtract(_startTime);
                }
                return currentTimeSpan;

            }
            set { this.SetProperty(ref this._duration, value); }
        }
        /// <summary>
        /// Total detected steps during the measurement.
        /// </summary>
        private uint _totalSteps;
        public uint TotalSteps
        {
            get { return _totalSteps; }
            set { this.SetProperty(ref this._totalSteps, value); }
        }
        /// <summary>
        /// Data of the graph.
        /// </summary>
        private OxyplotData _oxyplotData;
        public OxyplotData OxyplotData
        {
            get { return _oxyplotData; }
            set { this.SetProperty(ref this._oxyplotData, value); }
        }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        /// <summary>
        /// Set all values to start a measurement.
        /// </summary>
        public void StartMeasurement()
        {
            this.StartTime = DateTime.Now;
            this.MeasurementState = MeasurementState.Started;
        }

        /// <summary>
        /// Set all values to stop a measurement.
        /// </summary>
        public void StopMeasurement()
        {
            this.EndTime = DateTime.Now;
            this.MeasurementState = MeasurementState.Stopped;
        }
        /// <summary>
        /// Set all values to delete a measurement.
        /// </summary>
        public void DeleteMeasurement()
        {
            this.MeasurementState = MeasurementState.Deleted;
        }

        // property changed logic by jump start
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;
            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}