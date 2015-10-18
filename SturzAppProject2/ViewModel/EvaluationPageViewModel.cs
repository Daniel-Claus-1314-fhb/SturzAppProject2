using BackgroundTask.DataModel;
using SensorDataEvaluation.DataModel;
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
    public class EvaluationPageViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public EvaluationPageViewModel()
        {
            this.MeasurementViewModel = new MeasurementViewModel();
            this.EvaluationState = EvaluationState.Stopped;
            this.EvaluationDataModel = new EvaluationDataModel();
            this.EvalautionResultModel = new EvaluationResultModel();
            this.StartEvaluationCommand = new StartEvaluationCommand();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        private MeasurementViewModel _measurementViewModel;
        public MeasurementViewModel MeasurementViewModel
        {
            get { return _measurementViewModel; }
            set { this.SetProperty(ref this._measurementViewModel, value); }
        }

        private EvaluationDataModel _evaluationDataModel;
        public EvaluationDataModel EvaluationDataModel
        {
            get { return _evaluationDataModel; }
            set { _evaluationDataModel = value; }
        }

        private EvaluationResultModel _evaluationResultModel;
        public EvaluationResultModel EvalautionResultModel
        {
            get { return _evaluationResultModel; }
            set { _evaluationResultModel = value; }
        }

        private EvaluationState _evaluationState;
        public EvaluationState EvaluationState
        {
            get { return _evaluationState; }
            set { _evaluationState = value; }
        }
        

        public ICommand StartEvaluationCommand { get; set; }

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
    /// 
    /// </summary>
    public class StartEvaluationCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            bool canExecute = false;

            if (parameter != null &&
                parameter.GetType() == typeof(EvaluationPageViewModel))
            {
                EvaluationPageViewModel evaluationPageViewModel = parameter as EvaluationPageViewModel;
                
                if (evaluationPageViewModel.MeasurementViewModel != null &&
                    evaluationPageViewModel.MeasurementViewModel.MeasurementState == MeasurementState.Stopped &&

                    evaluationPageViewModel.EvaluationDataModel.AccelerometerSampleAnalysisList != null &&
                    evaluationPageViewModel.EvaluationDataModel.AccelerometerSampleAnalysisList.Count > 0 &&

                    evaluationPageViewModel.EvaluationDataModel.GyrometerSampleAnalysisList != null &&
                    evaluationPageViewModel.EvaluationDataModel.GyrometerSampleAnalysisList.Count > 0 &&

                    evaluationPageViewModel.EvaluationDataModel.QuaternionSampleAnalysisList != null &&
                    evaluationPageViewModel.EvaluationDataModel.QuaternionSampleAnalysisList.Count > 0 &&

                    evaluationPageViewModel.EvaluationState == EvaluationState.Stopped)
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
            
        }
    }

    #endregion
}
