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
            this.ShowGroup2 = true;
            this.ShowGroup3 = false;
            this.ShowGroup1Command = new ShowGroup1Command(plotShownGraphsAction);
            this.ShowGroup2Command = new ShowGroup2Command(plotShownGraphsAction);
            this.ShowGroup3Command = new ShowGroup3Command(plotShownGraphsAction);
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

        // LineSeries Group1
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
            set
            {
                this.SetProperty(ref this._showGroup1, value);
                //((ShowGroup1Command)this.ShowGroup1Command).OnCanExecuteChanged();
            }
        }

        // LineSeries Group2
        private LineSeries _vectorLengthLineSeries;
        public LineSeries VectorLengthLineSeries
        {
            get { return _vectorLengthLineSeries; }
            set { this.SetProperty(ref this._vectorLengthLineSeries, value); }
        }

        private bool _showGroup2;
        public bool ShowGroup2
        {
            get { return _showGroup2; }
            set
            {
                this.SetProperty(ref this._showGroup2, value);
                //((ShowGroup2Command)this.ShowGroup2Command).OnCanExecuteChanged();
            }
        }

        // LineSeries Group3
        private LineSeries _stepLineSeries;
        public LineSeries StepLineSeries
        {
            get { return _stepLineSeries; }
            set { this.SetProperty(ref this._stepLineSeries, value); }
        }

        private bool _showGroup3;
        public bool ShowGroup3
        {
            get { return _showGroup3; }
            set
            {
                this.SetProperty(ref this._showGroup3, value);
                //((ShowGroup3Command)this.ShowGroup3Command).OnCanExecuteChanged();
            }
        }

        public ICommand ShowGroup1Command { get; set; }
        public ICommand ShowGroup2Command { get; set; }
        public ICommand ShowGroup3Command { get; set; }

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
                    currentViewModel.VectorLengthLineSeries != null && currentViewModel.VectorLengthLineSeries.Points.Count > 0)
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

        public Action<GraphPageViewModel> actionPointer { get; set; }

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            if (actionPointer != null && parameter != null && parameter.GetType().Equals(typeof(GraphPageViewModel)))
            {
                GraphPageViewModel currentViewModel = parameter as GraphPageViewModel;
                if (currentViewModel != null &&
                    currentViewModel.StepLineSeries != null && currentViewModel.StepLineSeries.Points.Count > 0)
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

    # endregion
}
