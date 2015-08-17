using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace BackgroundTask.DataModel
{

    internal class AccelerometerData
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public AccelerometerData(string accerlerometerFilename)
        {
            this._accelerometerFilename = accerlerometerFilename;
            this._listChangeCounter = 0;
            this._processingListCount = 1000;
            this._accelerometerReadingsListEven = new List<AccelerometerReading>();
            this._accelerometerReadingsListOdd = new List<AccelerometerReading>();
        }

        public AccelerometerData(string accerlerometerDataId, uint processingListSize) 
            : this (accerlerometerDataId)
        {
            this._processingListCount = processingListSize;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        private string _accelerometerFilename;
        public string AccelerometerFilename
        {
            get { return _accelerometerFilename; }
        }

        private long _listChangeCounter;
        public long ListChangeCounter
        {
            get { return _listChangeCounter; }
        }

        private uint _processingListCount { get; set; }

        private List<AccelerometerReading> _accelerometerReadingsListEven { get; set; }
        private List<AccelerometerReading> _accelerometerReadingsListOdd { get; set; }


        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        public void AddAccelerometerReading(AccelerometerReading accelerometerReading)
        {
            bool isListSwitchRequired = false;

            if (_listChangeCounter % 2 == 0) 
            {
                _accelerometerReadingsListEven.Add(accelerometerReading);
                if (_accelerometerReadingsListEven.Count >= _processingListCount)
                {
                    isListSwitchRequired = true;
                }
            }
            else
            {
                _accelerometerReadingsListOdd.Add(accelerometerReading);
                if (_accelerometerReadingsListOdd.Count >= _processingListCount)
                {
                    isListSwitchRequired = true;
                }
            }

            if (isListSwitchRequired)
            {
                // clear passiv reading list before switch them to activ reading list.
                GetPassivReadingsList().Clear();
                // exchange activ and passiv reading list.
                _listChangeCounter++;
                // propagate switch of reading lists.
                OnReadingsListsHasSwitched(EventArgs.Empty);
            }
        }

        public List<AccelerometerReading> GetActivReadingsList()
        {
            if (_listChangeCounter % 2 == 0)
                return _accelerometerReadingsListEven;
            else
                return _accelerometerReadingsListOdd;
        }

        public List<AccelerometerReading> GetPassivReadingsList()
        {
            if (_listChangeCounter % 2 == 1)
                return _accelerometerReadingsListEven;
            else
                return _accelerometerReadingsListOdd;
        }

        //###################################################################################################################
        //################################################## Event handler ##################################################
        //###################################################################################################################

        protected virtual void OnReadingsListsHasSwitched(EventArgs e)
        {
            EventHandler handler = ReadingsListsHasSwitched;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler ReadingsListsHasSwitched;
    }
}
