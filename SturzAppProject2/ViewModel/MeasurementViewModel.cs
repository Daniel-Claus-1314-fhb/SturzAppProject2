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
            this.StartMeasurementCommand = new StartMeasurementCommand();
            this.StopMeasurementCommand = new StopMeasurementCommand();
            this.ExportMeasurementCommand = new ExportMeasurmentCommand();
            this.DeleteMeasurementCommand = new DeleteMeasurementCommand();
        }

        public MeasurementViewModel(Measurement measurement) : this()
        {
            this.Name = measurement.Name;
            this.Id = measurement.Id;
            this.CreateDateTime = measurement.CreateDateTime;
            this.StartTime = measurement.StartTime;
            this.EndTime = measurement.EndTime;

            this.MeasurementState = determineMeasurementState(measurement.StartTime, measurement.EndTime);
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
                if (StartTime != null && EndTime != null)
                    return _endTime.Subtract(_startTime);
                else if (StartTime != null && EndTime == null)
                    return DateTime.Now.Subtract(_startTime);
                else
                    return new TimeSpan(0L);
            }
        }

        public ICommand StartMeasurementCommand { get; set; }
        public ICommand StopMeasurementCommand { get; set; }
        public ICommand ExportMeasurementCommand { get; set; }
        public ICommand DeleteMeasurementCommand { get; set; }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        private MeasurementState determineMeasurementState(DateTime startTime, DateTime endTime)
        {
            if (startTime == null && endTime == null)
            {
                return MeasurementState.Initialized;
            }
            else if (startTime != null && endTime == null)
            {
                return MeasurementState.Started;
            }
            else
            {
                return MeasurementState.Stopped;
            }
        }

        public void StartMeasurement()
        {
            this.StartTime = DateTime.Now;
        }

        public void StopMeasurement()
        {
            this.EndTime = DateTime.Now;
        }

        public void ChangeMeasurementState(MeasurementState measurementState)
        {
            this.MeasurementState = measurementState;
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

    public enum MeasurementState
    {
        Initialized,
        Started,
        Stopped
    }

    //###################################################################################
    //##################################### Commands ####################################
    //###################################################################################

    #region Commands

    /// <summary>
    /// Decides whether a measurement could be started and started the measurement when its possible. 
    /// A measurement can be started only, when the state of the measurement is 'initialized'.
    /// </summary>
    public class StartMeasurementCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;

                if (measurementViewModel != null &&
                    measurementViewModel.MeasurementState == MeasurementState.Initialized)
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    measurementViewModel.StartMeasurement();
                    measurementViewModel.ChangeMeasurementState(MeasurementState.Started);
                }
            }
        }
    }

    /// <summary>
    /// Decides whether a measurement could be stopped and stops the measurement when its possible. 
    /// A measurement can be stopped only, when the state of the measurement is 'started'.
    /// </summary>
    public class StopMeasurementCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;

                if (measurementViewModel != null &&
                    measurementViewModel.MeasurementState == MeasurementState.Started)
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    measurementViewModel.StopMeasurement();
                    measurementViewModel.ChangeMeasurementState(MeasurementState.Stopped);
                }
            }
        }
    }

    /// <summary>
    /// Decides whether the data of a measurement could be exported and export the data when its possible. 
    /// The data can be exported only, when the state of the measurement is 'stopped'.
    /// </summary>
    public class ExportMeasurmentCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;

                if (measurementViewModel != null &&
                    measurementViewModel.MeasurementState == MeasurementState.Stopped)
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            // TODO Export data of the measurement.
        }
    }

    /// <summary>
    /// Decides whether the measurement could be deleted and deletes the measurement when its possible. 
    /// The measurement can be deleted only, when the state of the measurement is 'initialized' or 'stopped'.
    /// </summary>
    public class DeleteMeasurementCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;

                if (measurementViewModel != null &&
                    (measurementViewModel.MeasurementState == MeasurementState.Initialized ||
                    measurementViewModel.MeasurementState == MeasurementState.Stopped))
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            // TODO Delete measurement and all data.
        }
    }

    #endregion
}

