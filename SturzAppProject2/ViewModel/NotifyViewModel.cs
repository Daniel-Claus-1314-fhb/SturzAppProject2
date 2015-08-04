using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.ViewModel
{
    public class NotifyViewModel : INotifyPropertyChanged
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public NotifyViewModel()
        {
            this.Message = "Keine Message";
            this.Level = NotifyLevel.Info;
        }

        public NotifyViewModel(string message, NotifyLevel level)
        {
            this.Message = message;
            this.Level = level;
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        private string _message;
        public string Message
        {
            get { return _message; }
            set { this.SetProperty(ref this._message, value); }
        }

        private NotifyLevel _level;
        public NotifyLevel Level
        {
            get { return _level; }
            set { this.SetProperty(ref this._level, value); }
        }

        private int _activeTaskCount;
        public int ActiveTaskCount
        {
            get { return _activeTaskCount; }
            set { this.SetProperty(ref this._activeTaskCount, value); }
        }

        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        public void ShowMessage(string message, NotifyLevel level)
        {
            if (message != null)
            {
                this.Message = message;
                this.Level = level;
            }
        }

        public void ResetMessage()
        {
            this.Message = "Keine Message";
            this.Level = NotifyLevel.Info;
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

    public enum NotifyLevel
    {
        Info,
        Warn,
        Error
    };
}
