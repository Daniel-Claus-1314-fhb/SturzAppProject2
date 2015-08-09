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
        
        public MeasurementPageViewModel(Func<MeasurementViewModel, bool> startMeasurementMethod, Func<MeasurementViewModel, bool> stopMeasurementMethod,
            Func<MeasurementViewModel, bool> exportMeasurementMethod, Func<MeasurementViewModel, bool> deleteMeasurementMethod)
        {
            this.MeasurementViewModel = new MeasurementViewModel();
            this.StartMeasurementCommand = new StartMeasurementCommand(startMeasurementMethod);
            this.StopMeasurementCommand = new StopMeasurementCommand(stopMeasurementMethod);
            this.ExportMeasurementCommand = new ExportMeasurementCommand(exportMeasurementMethod);
            this.DeleteMeasurementCommand = new DeleteMeasurementCommand(deleteMeasurementMethod);
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
        public StartMeasurementCommand(Func<MeasurementViewModel, bool> funcPointer) 
        {
            this.FuncPointer = funcPointer;
        }

        public Func<MeasurementViewModel, bool> FuncPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.FuncPointer != null && 
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
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
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.FuncPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    FuncPointer(measurementViewModel);
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
        public StopMeasurementCommand(Func<MeasurementViewModel, bool> funcPointer) 
        {
            this.FuncPointer = funcPointer;
        }

        public Func<MeasurementViewModel, bool> FuncPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.FuncPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
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
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.FuncPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    FuncPointer(measurementViewModel);
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
        public ExportMeasurementCommand(Func<MeasurementViewModel, bool> funcPointer) 
        {
            this.FuncPointer = funcPointer;
        }

        public Func<MeasurementViewModel, bool> FuncPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.FuncPointer != null &&
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
            if (this.FuncPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    FuncPointer(measurementViewModel);
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
        public DeleteMeasurementCommand(Func<MeasurementViewModel, bool> funcPointer) 
        {
            this.FuncPointer = funcPointer;
        }

        public Func<MeasurementViewModel, bool> FuncPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (this.FuncPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
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
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (this.FuncPointer != null &&
                parameter != null &&
                parameter.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = parameter as MeasurementViewModel;
                if (measurementViewModel != null)
                {
                    FuncPointer(measurementViewModel);
                }
            }
        }
    }

    #endregion
}
