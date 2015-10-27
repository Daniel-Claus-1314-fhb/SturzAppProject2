using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgroundTask.ViewModel
{
    public class OverviewPageViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Constructors

        public OverviewPageViewModel()
        {
            this._measurementViewModels = new ObservableCollection<MeasurementViewModel>();
            this.CreateMeasurementCommand = new CreateMeasurementClick();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private ObservableCollection<MeasurementViewModel> _measurementViewModels;
        public ObservableCollection<MeasurementViewModel> MeasurementViewModels
        {
            get { return _measurementViewModels; }
            set { this.SetProperty(ref this._measurementViewModels, value); }
        }

        public ICommand CreateMeasurementCommand { get; set; }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        public void InsertMeasurement(MeasurementViewModel measurementViewModel)
        {
            if (measurementViewModel != null)
            {
                this.MeasurementViewModels.Insert(0, measurementViewModel);
            }
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
    
    //###################################################################################
    //##################################### Commands ####################################
    //###################################################################################

    #region Commands

    public class CreateMeasurementClick : ICommand
    {
        public Func<bool> FuncPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            if (FuncPointer != null &&
                parameter != null && 
                parameter.GetType() == typeof(OverviewPageViewModel))
            {
                OverviewPageViewModel overviewPageViewModel = parameter as OverviewPageViewModel;

                if (overviewPageViewModel != null &&
                    overviewPageViewModel.MeasurementViewModels != null &&
                    overviewPageViewModel.MeasurementViewModels.Count < 50 )
                {
                    return true;
                }
            }
            return false;
        }

        public event EventHandler CanExecuteChanged;
        public void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (FuncPointer != null)
            {
                FuncPointer(); 
                OnCanExecuteChanged();
            }
        }
    }

    #endregion
}
