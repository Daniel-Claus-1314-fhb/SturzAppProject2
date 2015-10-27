using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackgroundTask.ViewModel
{
    public class GraphPageViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors
        
        public GraphPageViewModel(Action<GraphPageViewModel> plotShownGraphsAction)
        {
            this.ShowGroup1 = false;
            this.ShowGroup2 = false;
            this.ShowGroup3 = false;
            this.ShowGroup4 = false;
            this.ShowGroup5 = false;
            this.ShowGroup6 = false;
            this.ShowGroup1Command = new ShowGroup1Command(plotShownGraphsAction);
            this.ShowGroup2Command = new ShowGroup2Command(plotShownGraphsAction);
            this.ShowGroup3Command = new ShowGroup3Command(plotShownGraphsAction);
            this.ShowGroup4Command = new ShowGroup4Command(plotShownGraphsAction);
            this.ShowGroup5Command = new ShowGroup5Command(plotShownGraphsAction);
            this.ShowGroup6Command = new ShowGroup6Command(plotShownGraphsAction);
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { this.SetProperty(ref this._plotModel, value); }
        }

        // LineSeries Group1 #################################################################################
        private LineSeries _accelerometerXLineSeries;
        public LineSeries AccelerometerXLineSeries
        {
            get { return _accelerometerXLineSeries; }
            set { this.SetProperty(ref this._accelerometerXLineSeries, value); }
        }
        private LineSeries _accelerometerYLineSeries;
        public LineSeries AccelerometerYLineSeries
        {
            get { return _accelerometerYLineSeries; }
            set { this.SetProperty(ref this._accelerometerYLineSeries, value); }
        }
        private LineSeries _accelerometerZLineSeries;
        public LineSeries AccelerometerZLineSeries
        {
            get { return _accelerometerZLineSeries; }
            set { this.SetProperty(ref this._accelerometerZLineSeries, value); }
        }

        private bool _showGroup1;
        public bool ShowGroup1
        {
            get { return _showGroup1; }
            set { this.SetProperty(ref this._showGroup1, value); }
        }

        // LineSeries Group2 #################################################################################
        private LineSeries _gyrometerXLineSeries;
        public LineSeries GyrometerXLineSeries
        {
            get { return _gyrometerXLineSeries; }
            set { _gyrometerXLineSeries = value; }
        }

        private LineSeries _gyrometerYLineSeries;
        public LineSeries GyrometerYLineSeries
        {
            get { return _gyrometerYLineSeries; }
            set { _gyrometerYLineSeries = value; }
        }

        private LineSeries _gyrometerZLineSeries;
        public LineSeries GyrometerZLineSeries
        {
            get { return _gyrometerZLineSeries; }
            set { _gyrometerZLineSeries = value; }
        }

        private bool _showGroup2;
        public bool ShowGroup2
        {
            get { return _showGroup2; }
            set { this.SetProperty(ref this._showGroup2, value); }
        }

        // LineSeries Group3 #################################################################################
        private LineSeries _quaterionWLineSeries;
        public LineSeries QuaterionWLineSeries
        {
            get { return _quaterionWLineSeries; }
            set { _quaterionWLineSeries = value; }
        }
        private LineSeries _quaterionXLineSeries;
        public LineSeries QuaterionXLineSeries
        {
            get { return _quaterionXLineSeries; }
            set { _quaterionXLineSeries = value; }
        }
        private LineSeries _quaterionYLineSeries;
        public LineSeries QuaterionYLineSeries
        {
            get { return _quaterionYLineSeries; }
            set { _quaterionYLineSeries = value; }
        }
        private LineSeries _quaterionZLineSeries;
        public LineSeries QuaterionZLineSeries
        {
            get { return _quaterionZLineSeries; }
            set { _quaterionZLineSeries = value; }
        }

        private bool _showGroup3;
        public bool ShowGroup3
        {
            get { return _showGroup3; }
            set { this.SetProperty(ref this._showGroup3, value); }
        }


        // LineSeries Group4 #################################################################################
        private LineSeries _accelerometerVectorLengthLineSeries;
        public LineSeries AccelerometerVectorLengthLineSeries
        {
            get { return _accelerometerVectorLengthLineSeries; }
            set { this.SetProperty(ref this._accelerometerVectorLengthLineSeries, value); }
        }

        private bool _showGroup4;
        public bool ShowGroup4
        {
            get { return _showGroup4; }
            set { this.SetProperty(ref this._showGroup4, value); }
        }

        // LineSeries Group5 #################################################################################
        private LineSeries _gyrometerVectorLengthLineSeries;
        public LineSeries GyrometerVectorLengthLineSeries
        {
            get { return _gyrometerVectorLengthLineSeries; }
            set { this.SetProperty(ref this._gyrometerVectorLengthLineSeries, value); }
        }

        private bool _showGroup5;
        public bool ShowGroup5
        {
            get { return _showGroup5; }
            set { this.SetProperty(ref this._showGroup5, value); }
        }

        // LineSeries Group6 #################################################################################
        private LineSeries _assumedAccelerometerStepLineSeries;
        public LineSeries AssumedAccelerometerStepLineSeries
        {
            get { return _assumedAccelerometerStepLineSeries; }
            set { this.SetProperty(ref this._assumedAccelerometerStepLineSeries, value); }
        }

        private LineSeries _assumedGyrometerStepLineSeries;
        public LineSeries AssumedGyrometerStepLineSeries
        {
            get { return _assumedGyrometerStepLineSeries; }
            set { this.SetProperty(ref this._assumedGyrometerStepLineSeries, value); }
        }

        private LineSeries _detectedStepLineSeries;
        public LineSeries DetectedStepLineSeries
        {
            get { return _detectedStepLineSeries; }
            set { this.SetProperty(ref this._detectedStepLineSeries, value); }
        }

        private bool _showGroup6;
        public bool ShowGroup6
        {
            get { return _showGroup6; }
            set { this.SetProperty(ref this._showGroup6, value); }
        }

        public ICommand ShowGroup1Command { get; set; }
        public ICommand ShowGroup2Command { get; set; }
        public ICommand ShowGroup3Command { get; set; }
        public ICommand ShowGroup4Command { get; set; }
        public ICommand ShowGroup5Command { get; set; }
        public ICommand ShowGroup6Command { get; set; }

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

    public class ShowGroup1Command : ICommand
    {
        public ShowGroup1Command(Action<GraphPageViewModel> appBarButtonClickAction)
        {
            this.actionPointer = appBarButtonClickAction;
        }

        private Action<GraphPageViewModel> actionPointer  { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null &&
                    currentViewModel.AccelerometerXLineSeries != null && currentViewModel.AccelerometerXLineSeries.Points.Count > 0 &&
                    currentViewModel.AccelerometerYLineSeries != null && currentViewModel.AccelerometerYLineSeries.Points.Count > 0 &&
                    currentViewModel.AccelerometerZLineSeries != null && currentViewModel.AccelerometerZLineSeries.Points.Count > 0)
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
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null)
                {
                    currentViewModel.ShowGroup1 = !currentViewModel.ShowGroup1;
                    this.actionPointer(currentViewModel);
                    this.OnCanExecuteChanged();
                }
            }
        }
    }

    public class ShowGroup2Command : ICommand
    {
        public ShowGroup2Command(Action<GraphPageViewModel> appBarButtonClickAction)
        {
            this.actionPointer = appBarButtonClickAction;
        }

        private Action<GraphPageViewModel> actionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null &&
                    currentViewModel.GyrometerXLineSeries != null && currentViewModel.GyrometerXLineSeries.Points.Count > 0 &&
                    currentViewModel.GyrometerYLineSeries != null && currentViewModel.GyrometerYLineSeries.Points.Count > 0 &&
                    currentViewModel.GyrometerZLineSeries != null && currentViewModel.GyrometerZLineSeries.Points.Count > 0)
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
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null)
                {
                    currentViewModel.ShowGroup2 = !currentViewModel.ShowGroup2;
                    this.actionPointer(currentViewModel);
                    this.OnCanExecuteChanged();
                }
            }
        }
    }

    public class ShowGroup3Command : ICommand
    {
        public ShowGroup3Command(Action<GraphPageViewModel> appBarButtonClickAction)
        {
            this.actionPointer = appBarButtonClickAction;
        }

        private Action<GraphPageViewModel> actionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null &&
                    currentViewModel.QuaterionWLineSeries != null && currentViewModel.QuaterionWLineSeries.Points.Count > 0 &&
                    currentViewModel.QuaterionXLineSeries != null && currentViewModel.QuaterionXLineSeries.Points.Count > 0 &&
                    currentViewModel.QuaterionYLineSeries != null && currentViewModel.QuaterionYLineSeries.Points.Count > 0 &&
                    currentViewModel.QuaterionZLineSeries != null && currentViewModel.QuaterionZLineSeries.Points.Count > 0)
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
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null)
                {
                    currentViewModel.ShowGroup3 = !currentViewModel.ShowGroup3;
                    this.actionPointer(currentViewModel);
                    this.OnCanExecuteChanged();
                }
            }
        }
    }

    public class ShowGroup4Command : ICommand
    {
        public ShowGroup4Command(Action<GraphPageViewModel> appBarButtonClickAction)
        {
            this.actionPointer = appBarButtonClickAction;
        }

        private Action<GraphPageViewModel> actionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null &&
                    currentViewModel.AccelerometerVectorLengthLineSeries != null && currentViewModel.AccelerometerVectorLengthLineSeries.Points.Count > 0)
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
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null)
                {
                    currentViewModel.ShowGroup4 = !currentViewModel.ShowGroup4;
                    this.actionPointer(currentViewModel);
                    this.OnCanExecuteChanged();
                }
            }
        }
    }

    public class ShowGroup5Command : ICommand
    {
        public ShowGroup5Command(Action<GraphPageViewModel> appBarButtonClickAction)
        {
            this.actionPointer = appBarButtonClickAction;
        }

        private Action<GraphPageViewModel> actionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null &&
                    currentViewModel.GyrometerVectorLengthLineSeries != null && currentViewModel.GyrometerVectorLengthLineSeries.Points.Count > 0)
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
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null)
                {
                    currentViewModel.ShowGroup5 = !currentViewModel.ShowGroup5;
                    this.actionPointer(currentViewModel);
                    this.OnCanExecuteChanged();
                }
            }
        }
    }

    public class ShowGroup6Command : ICommand
    {
        public ShowGroup6Command(Action<GraphPageViewModel> appBarButtonClickAction)
        {
            this.actionPointer = appBarButtonClickAction;
        }

        public Action<GraphPageViewModel> actionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null &&
                    currentViewModel.AssumedAccelerometerStepLineSeries != null && currentViewModel.AssumedAccelerometerStepLineSeries.Points.Count > 0 &&
                    currentViewModel.AssumedGyrometerStepLineSeries != null && currentViewModel.AssumedGyrometerStepLineSeries.Points.Count > 0 &&
                    currentViewModel.DetectedStepLineSeries != null && currentViewModel.DetectedStepLineSeries.Points.Count > 0)
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
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null)
                {
                    currentViewModel.ShowGroup6 = !currentViewModel.ShowGroup6;
                    this.actionPointer(currentViewModel);
                    this.OnCanExecuteChanged();
                }
            }
        }
    }

    # endregion
}
