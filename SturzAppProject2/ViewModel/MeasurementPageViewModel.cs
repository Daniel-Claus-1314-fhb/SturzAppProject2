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
    public class MeasurementPageViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public MeasurementPageViewModel(Action<MeasurementViewModel> startMeasurementMethod, Action<MeasurementViewModel> stopMeasurementMethod,
            Action<MeasurementViewModel> exportMeasurementMethod, Action<MeasurementViewModel> deleteMeasurementMethod, Action<MeasurementViewModel> showMeasurementGraphMethod)
        {
            this.MeasurementViewModel = new MeasurementViewModel();
            this.StartMeasurementCommand = new StartMeasurementCommand(startMeasurementMethod);
            this.StopMeasurementCommand = new StopMeasurementCommand(stopMeasurementMethod);
            this.ExportMeasurementCommand = new ExportMeasurementCommand(exportMeasurementMethod);
            this.DeleteMeasurementCommand = new DeleteMeasurementCommand(deleteMeasurementMethod);
            this.ShowMeasurementGraphCommand = new ShowMeasurementGraphCommand(showMeasurementGraphMethod);
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private MeasurementViewModel _measurementViewModel;
        public MeasurementViewModel MeasurementViewModel
        {
            get { return _measurementViewModel; }
            set { this.SetProperty(ref this._measurementViewModel, value); }
        }
        
        public ICommand StartMeasurementCommand { get; set; }
        public ICommand StopMeasurementCommand { get; set; }
        public ICommand ExportMeasurementCommand { get; set; }
        public ICommand DeleteMeasurementCommand { get; set; }
        public ICommand ShowMeasurementGraphCommand { get; set; }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

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

    //########################################################################################
    //##################################### Command classes ##################################
    //########################################################################################

    #region Commands

    /// <summary>
    /// Decides whether a measurement could be started and started the measurement when its possible. 
    /// A measurement can be started only, when the state of the measurement is 'initialized'.
    /// </summary>
    public class StartMeasurementCommand : ICommand
    {
        public StartMeasurementCommand(Action<MeasurementViewModel> actionPointer)
        {
            this.ActionPointer = actionPointer;
        }

        public Action<MeasurementViewModel> ActionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;

                if (measurementViewModel != null &&
                    measurementViewModel.MeasurementState == MeasurementState.Initialized &&
                    measurementViewModel.MeasurementState != MeasurementState.Deleted)
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    ActionPointer(measurementViewModel);
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
        public StopMeasurementCommand(Action<MeasurementViewModel> actionPointer)
        {
            this.ActionPointer = actionPointer;
        }

        public Action<MeasurementViewModel> ActionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;

                if (measurementViewModel != null &&
                    measurementViewModel.MeasurementState == MeasurementState.Started &&
                    measurementViewModel.MeasurementState != MeasurementState.Deleted)
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    ActionPointer(measurementViewModel);
                }
            }
        }
    }

    /// <summary>
    /// Decides whether the data of a measurement could be exported and export the data when its possible. 
    /// The data can be exported only, when the state of the measurement is 'stopped'.
    /// </summary>
    public class ExportMeasurementCommand : ICommand
    {
        public ExportMeasurementCommand(Action<MeasurementViewModel> actionPointer)
        {
            this.ActionPointer = actionPointer;
        }

        public Action<MeasurementViewModel> ActionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
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
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    ActionPointer(measurementViewModel);
                }
            }
        }
    }

    /// <summary>
    /// Decides whether the measurement could be deleted and deletes the measurement when its possible. 
    /// The measurement can be deleted only, when the state of the measurement is 'initialized' or 'stopped'.
    /// </summary>
    public class DeleteMeasurementCommand : ICommand
    {
        public DeleteMeasurementCommand(Action<MeasurementViewModel> actionPointer)
        {
            this.ActionPointer = actionPointer;
        }

        public Action<MeasurementViewModel> ActionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;

                if (measurementViewModel != null &&
                    (measurementViewModel.MeasurementState == MeasurementState.Initialized ||
                    measurementViewModel.MeasurementState == MeasurementState.Stopped) &&
                    measurementViewModel.MeasurementState != MeasurementState.Deleted)
                {
                    canExecute = true;
                }
            }
            return canExecute;
        }

        public event EventHandler CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    ActionPointer(measurementViewModel);
                }
            }
        }
    }

    /// <summary>
    /// Decides whether the measurement has data which can be shown in a oxyplot grath or not.
    /// </summary>
    public class ShowMeasurementGraphCommand : ICommand
    {
        public ShowMeasurementGraphCommand(Action<MeasurementViewModel> actionPointer)
        {
            this.ActionPointer = actionPointer;
        }

        public Action<MeasurementViewModel> ActionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
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
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.ActionPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    ActionPointer(measurementViewModel);
                }
            }
        }
    }

    #endregion
}
