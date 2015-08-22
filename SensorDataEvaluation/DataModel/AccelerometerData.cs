using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace SensorDataEvaluation.DataModel
{

    public class AccelerometerData
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public AccelerometerData(string accerlerometerFilename)
        {
            this._filename = accerlerometerFilename;
            this._startDateTime = DateTimeOffset.MinValue;
            this._listChangeCounter = 0;
            this._processingListCount = 1000;
            this._accelerometerTupleListEven = new List<Tuple<TimeSpan, double, double, double>>();
            this._accelerometerTupleListOdd = new List<Tuple<TimeSpan, double, double, double>>();
        }

        public AccelerometerData(string accerlerometerDataId, uint processingListSize) 
            : this (accerlerometerDataId)
        {
            this._processingListCount = processingListSize;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################


        private string _filename;
        public string Filename
        {
            get { return _filename; }
        }

        private long _listChangeCounter;
        public long ListChangeCounter
        {
            get { return _listChangeCounter; }
        }

        private DateTimeOffset _startDateTime;
        private uint _processingListCount { get; set; }

        /// <summary>
        /// item1: timespan since the start of measurement.
        /// item2: X coordinate of the accelerometer tuple.
        /// item3: Y coorfinate of the accelerometer tuple.
        /// item4: Z coordinate of the accelerometer tuple.
        /// </summary>
        private List<Tuple<TimeSpan, double, double, double>> _accelerometerTupleListEven { get; set; }
        private List<Tuple<TimeSpan, double, double, double>> _accelerometerTupleListOdd { get; set; }


        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        /// <summary>
        /// Adds a new accelerometer reading into the active accelerometer reading list.
        /// </summary>
        /// <param name="accelerometerReading"></param>
        public void AddAccelerometerReading(AccelerometerReading accelerometerReading)
        {
            if (accelerometerReading != null)
            {
                if (_startDateTime.CompareTo(DateTimeOffset.MinValue) == 0)
                {
                    _startDateTime = accelerometerReading.Timestamp;
                }

                TimeSpan item1 = accelerometerReading.Timestamp.Subtract(_startDateTime);
                double item2 = accelerometerReading.AccelerationX;
                double item3 = accelerometerReading.AccelerationY;
                double item4 = accelerometerReading.AccelerationZ;
                this.AddAccelerometerTuple(new Tuple<TimeSpan, double, double, double>(item1, item2, item3, item4));
            }
        }

        /// <summary>
        /// Adds a new accelerometer tuple into the active accelerometer tuple list.
        /// </summary>
        /// <param name="accelerometerTuple"></param>
        private void AddAccelerometerTuple(Tuple<TimeSpan, double, double, double> accelerometerTuple)
        {
            bool isListSwitchRequired = false;

            if (_listChangeCounter % 2 == 0) 
            {
                _accelerometerTupleListEven.Add(accelerometerTuple);
                if (_accelerometerTupleListEven.Count >= _processingListCount)
                {
                    isListSwitchRequired = true;
                }
            }
            else
            {
                _accelerometerTupleListOdd.Add(accelerometerTuple);
                if (_accelerometerTupleListOdd.Count >= _processingListCount)
                {
                    isListSwitchRequired = true;
                }
            }

            if (isListSwitchRequired)
            {
                // clear passiv tuple list before switch them to activ tuple list.
                GetPassivTupleList().Clear();
                // exchange activ and passiv tuple list.
                _listChangeCounter++;
                // propagate switch of tuple lists.
                OnTupleListsHasSwitched(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns the accelerometer tuple list which is currently used to store new accelerometer tuple.
        /// </summary>
        /// <returns></returns>
        public List<Tuple<TimeSpan, double, double, double>> GetActivTupleList()
        {
            if (_listChangeCounter % 2 == 0)
                return _accelerometerTupleListEven;
            else
                return _accelerometerTupleListOdd;
        }

        /// <summary>
        /// Returns the accelerometer tuple list which is currently NOT used store new accelerometer tuples.
        /// </summary>
        /// <returns></returns>
        public List<Tuple<TimeSpan, double, double, double>> GetPassivTupleList()
        {
            if (_listChangeCounter % 2 == 1)
                return _accelerometerTupleListEven;
            else
                return _accelerometerTupleListOdd;
        }

        //###################################################################################################################
        //################################################## Event handler ##################################################
        //###################################################################################################################

        protected virtual void OnTupleListsHasSwitched(EventArgs e)
        {
            EventHandler handler = TupleListsHasSwitched;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler TupleListsHasSwitched;
    }
}
