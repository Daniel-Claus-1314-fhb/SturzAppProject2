using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace SensorDataEvaluation.DataModel
{
    public class MeasurementData
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public MeasurementData(string MeasurementFilename)
        {
            this._filename = MeasurementFilename;
            this._startDateTime = DateTimeOffset.MinValue;
            this._listChangeCounter = 0;
            this._processingListCount = 1000;
            this._accelerometerListEven = new List<AccelerometerSample>();
            this._accelerometerListOdd = new List<AccelerometerSample>();

            this._gyrometerListEven = new List<GyrometerSample>();
            this._gyrometerListOdd = new List<GyrometerSample>();

            this._quaternionListEven = new List<QuaternionSample>();
            this._quaternionListOdd = new List<QuaternionSample>();
        }

        public MeasurementData(string measurementFilename, uint processingListSize) 
            : this (measurementFilename)
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
        private uint _processingListCount;

        /// <summary>
        /// item1: timespan since the start of measurement.
        /// item2: X coordinate of the accelerometer.
        /// item3: Y coorfinate of the accelerometer.
        /// item4: Z coordinate of the accelerometer.
        /// </summary>
        private List<AccelerometerSample> _accelerometerListEven { get; set; }
        private List<AccelerometerSample> _accelerometerListOdd { get; set; }

        /// <summary>
        /// item1: timespan since the start of measurement.
        /// item2: X angle velocity of the gyrometer.
        /// item3: Y angle velocity of the gyrometer.
        /// item4: Z angle velocity of the gyrometer.
        /// </summary>
        private List<GyrometerSample> _gyrometerListEven { get; set; }
        private List<GyrometerSample> _gyrometerListOdd { get; set; }


        /// <summary>
        /// item1: timespan since the start of measurement.
        /// item2: W rotaion angle of the quaternion.
        /// item3: X coordinate of the quaternion vector.
        /// item4: Y coorfinate of the quaternion vector.
        /// item5: Z coordinate of the quaternion vector.
        /// </summary>
        private List<QuaternionSample> _quaternionListEven { get; set; }
        private List<QuaternionSample> _quaternionListOdd { get; set; }


        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        private void SwitchBetweenLists()
        {
            // clear passiv list before switch them to activ list.
            GetPassivAccelerometerList().Clear();
            GetPassivGyrometerList().Clear();
            GetPassivQuaternionList().Clear();
            // exchange activ and passiv list.
            _listChangeCounter++;
            // propagate switch of lists.
            OnListsHasSwitched(EventArgs.Empty);
        }

        //################################################## Accelerometer ##################################################
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

                AccelerometerSample accelerometerSample = new AccelerometerSample();
                accelerometerSample.MeasurementTime = accelerometerReading.Timestamp.Subtract(_startDateTime);
                accelerometerSample.CoordinateX = accelerometerReading.AccelerationX;
                accelerometerSample.CoordinateY = accelerometerReading.AccelerationY;
                accelerometerSample.CoordinateZ = accelerometerReading.AccelerationZ;
                this.AddAccelerometerSample(accelerometerSample);
            }
        }
        /// <summary>
        /// Adds a new accelerometer sample into the active accelerometer list.
        /// </summary>
        /// <param name="accelerometerSample"></param>
        private void AddAccelerometerSample(AccelerometerSample accelerometerSample)
        {
            GetActivAccelerometerList().Add(accelerometerSample);
            // Decides whether a list switch is necessary.
            if (GetActivAccelerometerList().Count >= _processingListCount)
            {
                SwitchBetweenLists();
            }
        }

        /// <summary>
        /// Returns the accelerometer list which is currently used to store new accelerometer samples.
        /// </summary>
        /// <returns></returns>
        public List<AccelerometerSample> GetActivAccelerometerList()
        {
            return _listChangeCounter % 2 == 0 ? _accelerometerListEven : _accelerometerListOdd;
        }
        /// <summary>
        /// Returns the accelerometer list which is currently NOT used store new accelerometer samples.
        /// </summary>
        /// <returns></returns>
        public List<AccelerometerSample> GetPassivAccelerometerList()
        {
            return _listChangeCounter % 2 == 1 ? _accelerometerListEven : _accelerometerListOdd;
        }

        /// <summary>
        /// Creates a csv string of the accelerometer list.
        /// </summary>
        /// <param name="useActiveList">Decides whether active or passive list is used in csv creation.</param>
        /// <returns></returns>
        public string ToAccelerometerCSVString(bool useActiveList)
        {
            StringBuilder stringbuilder = new StringBuilder();
            IList<AccelerometerSample> accelerometerSampleList = useActiveList ? this.GetActivAccelerometerList() : this.GetPassivAccelerometerList();
            var enumerator = accelerometerSampleList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccelerometerSample accelerometerSample = enumerator.Current;
                stringbuilder.Append(accelerometerSample.ToCSVString());
            }
            return stringbuilder.ToString();
        }

        //################################################## Gyrometer ######################################################
        
        /// <summary>
        /// Adds a new gyrometer reading into the active gyrometer reading list.
        /// </summary>
        /// <param name="gyrometerReading"></param>
        public void AddGyrometerReading(GyrometerReading gyrometerReading)
        {
            if (gyrometerReading != null)
            {
                if (_startDateTime.CompareTo(DateTimeOffset.MinValue) == 0)
                {
                    _startDateTime = gyrometerReading.Timestamp;
                }

                GyrometerSample gyrometerSample = new GyrometerSample();
                gyrometerSample.MeasurementTime = gyrometerReading.Timestamp.Subtract(_startDateTime);
                gyrometerSample.VelocityX = gyrometerReading.AngularVelocityX;
                gyrometerSample.VelocityY = gyrometerReading.AngularVelocityY;
                gyrometerSample.VelocityZ = gyrometerReading.AngularVelocityZ;
                this.AddGyrometerSample(gyrometerSample);
            }
        }
        /// <summary>
        /// Adds a new gyrometer sample into the active gyrometer list.
        /// </summary>
        /// <param name="gyrometerSample"></param>
        private void AddGyrometerSample(GyrometerSample gyrometerSample)
        {
            GetActivGyrometerList().Add(gyrometerSample);
            // Decides whether a list switch is necessary.
            if (GetActivGyrometerList().Count >= _processingListCount)
            {
                SwitchBetweenLists();
            }
        }

        /// <summary>
        /// Returns the gyrometer list which is currently used to store new gyrometer sample.
        /// </summary>
        /// <returns></returns>
        public List<GyrometerSample> GetActivGyrometerList()
        {
            return _listChangeCounter % 2 == 0 ? _gyrometerListEven : _gyrometerListOdd;
        }
        /// <summary>
        /// Returns the gyrometer list which is currently NOT used store new gyrometer sample.
        /// </summary>
        /// <returns></returns>
        public List<GyrometerSample> GetPassivGyrometerList()
        {
            return _listChangeCounter % 2 == 1 ? _gyrometerListEven : _gyrometerListOdd;
        }

        /// <summary>
        /// Creates a csv string of the gyrometer list.
        /// </summary>
        /// <param name="useActiveList">Decides whether active or passive list is used in csv creation.</param>
        /// <returns></returns>
        public string ToGyrometerCSVString(bool useActiveList)
        {
            StringBuilder stringbuilder = new StringBuilder();
            IList<GyrometerSample> gyrometerList = useActiveList ? this.GetActivGyrometerList() : this.GetPassivGyrometerList();
            var enumerator = gyrometerList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GyrometerSample gyrometerSample = enumerator.Current;
                stringbuilder.Append(gyrometerSample.ToCSVString());
            }
            return stringbuilder.ToString();
        }

        //################################################## Quaternion #####################################################
        /// <summary>
        /// Adds a new orientation sensor reading into the active orientation sensor reading list.
        /// </summary>
        /// <param name="orientationSensorReading"></param>
        public void AddOrientationSensorReading(OrientationSensorReading orientationSensorReading)
        {
            if (orientationSensorReading != null)
            {
                if (_startDateTime.CompareTo(DateTimeOffset.MinValue) == 0)
                {
                    _startDateTime = orientationSensorReading.Timestamp;
                }

                QuaternionSample quaternionSample = new QuaternionSample();
                quaternionSample.MeasurementTime = orientationSensorReading.Timestamp.Subtract(_startDateTime);
                quaternionSample.AngleW = orientationSensorReading.Quaternion.W;
                quaternionSample.CoordinateX = orientationSensorReading.Quaternion.X;
                quaternionSample.CoordinateY = orientationSensorReading.Quaternion.Y;
                quaternionSample.CoordinateZ = orientationSensorReading.Quaternion.Z;
                this.AddQuaternionSample(quaternionSample);
            }
        }
        /// <summary>
        /// Adds a new quaternion sample into the active quaternion list.
        /// </summary>
        /// <param name="quaternionSample"></param>
        private void AddQuaternionSample(QuaternionSample quaternionSample)
        {
            GetActivQuaternionList().Add(quaternionSample);
            // Decides whether a list switch is necessary.
            if (GetActivQuaternionList().Count >= _processingListCount)
            {
                SwitchBetweenLists();
            }
        }

        /// <summary>
        /// Returns the quaternion list which is currently used to store new quaternion samples.
        /// </summary>
        /// <returns></returns>
        public List<QuaternionSample> GetActivQuaternionList()
        {
            return _listChangeCounter % 2 == 0 ? _quaternionListEven : _quaternionListOdd;
        }
        /// <summary>
        /// Returns the quaternion list which is currently NOT used store new quaternion samples.
        /// </summary>
        /// <returns></returns>
        public List<QuaternionSample> GetPassivQuaternionList()
        {
            return _listChangeCounter % 2 == 1 ? _quaternionListEven : _quaternionListOdd;
        }
        
        /// <summary>
        /// Creates a csv string of the quaternion list.
        /// </summary>
        /// <param name="useActiveList">Decides whether active or passive list is used in csv creation.</param>
        /// <returns></returns>
        public string ToQuaternionCSVString(bool useActiveList)
        {
            StringBuilder stringbuilder = new StringBuilder();
            IList<QuaternionSample> quaternionList = useActiveList ? this.GetActivQuaternionList() : this.GetPassivQuaternionList();
            var enumerator = quaternionList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QuaternionSample quaternionSample = enumerator.Current;
                stringbuilder.Append(quaternionSample.ToCSVString());
            }
            return stringbuilder.ToString();
        }

        //###################################################################################################################
        //################################################## Event handler ##################################################
        //###################################################################################################################

        protected virtual void OnListsHasSwitched(EventArgs e)
        {
            EventHandler handler = ListsHasSwitched;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler ListsHasSwitched;
    }
}
