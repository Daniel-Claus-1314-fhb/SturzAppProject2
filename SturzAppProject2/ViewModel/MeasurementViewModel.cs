using BackgroundTask.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public MeasurementViewModel(Measurement measurement) : this()
        {
            this.Name = measurement.Name;
            this.Id = measurement.Id;
            this.CreateDateTime = measurement.CreateDateTime;
            this.StartTime = measurement.StartTime;
            this.EndTime = measurement.EndTime;
            this.MeasurementState = measurement.MeasurementState;
            this.MeasurementSetting = new MeasurementSettingViewModel(measurement.Setting);
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.SetProperty(ref this._name, value); }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }

        private MeasurementState _measurementState;
        public MeasurementState MeasurementState
        {
            get { return _measurementState; }
            set { this.SetProperty(ref this._measurementState, value); }
        }

        private MeasurementSettingViewModel _measuermentSetting;
        public MeasurementSettingViewModel MeasurementSetting
        {
            get { return _measuermentSetting; }
            set { this.SetProperty(ref this._measuermentSetting, value); }
        }
        

        private DateTime _createDateTime;
        public DateTime CreateDateTime
        {
            get { return _createDateTime; }
            set { this.SetProperty(ref this._createDateTime, value); }
        }
        
        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set { this.SetProperty(ref this._startTime, value); }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
            set { this.SetProperty(ref this._endTime, value); }
        }

        public TimeSpan Duration
        {
            get
            {
                if (this.StartTime.CompareTo(DateTime.MinValue) != 0 &&
                    this.EndTime.CompareTo(DateTime.MinValue) != 0)
                    return _endTime.Subtract(_startTime);

                else if (this.StartTime.CompareTo(DateTime.MinValue) != 0 &&
                    this.EndTime.CompareTo(DateTime.MinValue) == 0)
                    return DateTime.Now.Subtract(_startTime);

                else
                    return new TimeSpan(0L);
            }
        }

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

        public void StartMeasurement()
        {
            this.StartTime = DateTime.Now;
            this.MeasurementState = MeasurementState.Started;
        }

        public void StopMeasurement()
        {
            this.EndTime = DateTime.Now;
            this.MeasurementState = MeasurementState.Stopped;
        }

        public void AbortMeasurement()
        {
            this.EndTime = DateTime.Now;
            this.MeasurementState = MeasurementState.Aborted;
        }

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