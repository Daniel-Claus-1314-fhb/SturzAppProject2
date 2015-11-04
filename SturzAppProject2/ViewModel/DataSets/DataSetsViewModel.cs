using BackgroundTask.DataModel.DataSets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.ViewModel.DataSets
{
    public class DataSetsViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public DataSetsViewModel(MeasurementDataSets dataSets)
        {
            this.IsAvailableAccelerometer = dataSets.accelerometerDataSet.IsAvailable;
            this.TotalCountAccelerometer = dataSets.accelerometerDataSet.TotalCount;

            this.IsAvailableGyrometer = dataSets.gyrometerDataSet.IsAvailable;
            this.TotalCountGyrometer = dataSets.gyrometerDataSet.TotalCount;

            this.IsAvailableQuaternion = dataSets.quaterionDataSet.IsAvailable;
            this.TotalCountQuaternion = dataSets.quaterionDataSet.TotalCount;

            this.IsAvailableGeolocation = dataSets.geolocationDataSet.IsAvailable;
            this.TotalCountGeolocation = dataSets.geolocationDataSet.TotalCount;

            this.IsAvailableEvaluation = dataSets.evaluationDataSet.IsAvailable;
            this.TotalCountEvaluation = dataSets.evaluationDataSet.TotalCount;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private bool _isAvailableAccelerometer;
        public bool IsAvailableAccelerometer
        {
            get { return _isAvailableAccelerometer; }
            set { this.SetProperty(ref this._isAvailableAccelerometer, value); }
        }
        private int _totalCountAccelerometer;
        public int TotalCountAccelerometer
        {
            get { return _totalCountAccelerometer; }
            set { this.SetProperty(ref this._totalCountAccelerometer, value); }
        }


        private bool _isAvailableGyrometer;
        public bool IsAvailableGyrometer
        {
            get { return _isAvailableGyrometer; }
            set { this.SetProperty(ref this._isAvailableGyrometer, value); }
        }
        private int _totalCountGyrometer;
        public int TotalCountGyrometer
        {
            get { return _totalCountGyrometer; }
            set { this.SetProperty(ref this._totalCountGyrometer, value); }
        }


        private bool _isAvailableQuaternion;
        public bool IsAvailableQuaternion
        {
            get { return _isAvailableQuaternion; }
            set { this.SetProperty(ref this._isAvailableQuaternion, value); }
        }
        private int _totalCountQuaternion;
        public int TotalCountQuaternion
        {
            get { return _totalCountQuaternion; }
            set { this.SetProperty(ref this._totalCountQuaternion, value); }
        }


        private bool _isAvailableGeolocation;
        public bool IsAvailableGeolocation
        {
            get { return _isAvailableGeolocation; }
            set { this.SetProperty(ref this._isAvailableGeolocation, value); }
        }
        private int _totalCountGeolocation;
        public int TotalCountGeolocation
        {
            get { return _totalCountGeolocation; }
            set { this.SetProperty(ref this._totalCountGeolocation, value); }
        }


        private bool _isAvailableEvaluation;
        public bool IsAvailableEvaluation
        {
            get { return _isAvailableEvaluation; }
            set { this.SetProperty(ref this._isAvailableEvaluation, value); }
        }
        private int _totalCountEvaluation;
        public int TotalCountEvaluation
        {
            get { return _totalCountEvaluation; }
            set { this.SetProperty(ref this._totalCountEvaluation, value); }
        }
                

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
}
