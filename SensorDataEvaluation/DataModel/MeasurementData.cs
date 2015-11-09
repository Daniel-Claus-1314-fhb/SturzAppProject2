using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
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
            this._sampleBufferSize = 1000;
            this._accelerometerListEven = new List<AccelerometerSample>();
            this._accelerometerListOdd = new List<AccelerometerSample>();

            this._gyrometerListEven = new List<GyrometerSample>();
            this._gyrometerListOdd = new List<GyrometerSample>();

            this._quaternionListEven = new List<QuaternionSample>();
            this._quaternionListOdd = new List<QuaternionSample>();

            this._geolocationListEven = new List<GeolocationSample>();
            this._geolocationListOdd = new List<GeolocationSample>();
        }

        public MeasurementData(string measurementFilename, uint processingListSize)
            : this(measurementFilename)
        {
            this._sampleBufferSize = processingListSize;
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
        private uint _sampleBufferSize;

        private List<AccelerometerSample> _accelerometerListEven { get; set; }
        private List<AccelerometerSample> _accelerometerListOdd { get; set; }

        private List<GyrometerSample> _gyrometerListEven { get; set; }
        private List<GyrometerSample> _gyrometerListOdd { get; set; }

        private List<QuaternionSample> _quaternionListEven { get; set; }
        private List<QuaternionSample> _quaternionListOdd { get; set; }

        private List<GeolocationSample> _geolocationListEven { get; set; }
        private List<GeolocationSample> _geolocationListOdd { get; set; }

        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        private void SwitchBetweenLists()
        {
            // clear passiv list before switch them to activ list.
            GetPassivAccelerometerList().Clear();
            GetPassivGyrometerList().Clear();
            GetPassivQuaternionList().Clear();
            GetPassivGeolocationList().Clear();
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
                this.AddAccelerometerSample(new AccelerometerSample(accelerometerReading, _startDateTime));
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
            if (GetActivAccelerometerList().Count >= _sampleBufferSize)
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

        public byte[] ToAccelerometerBytes(bool useActiveList)
        {
            IList<AccelerometerSample> accelerometerSampleList = useActiveList ? this.GetActivAccelerometerList() : this.GetPassivAccelerometerList();
            List<byte[]> resultByteArrays = new List<byte[]>();
            var enumerator = accelerometerSampleList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                resultByteArrays.Add(enumerator.Current.ToByteArray());
            }
            return resultByteArrays.SelectMany(a => a).ToArray();
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
                this.AddGyrometerSample(new GyrometerSample(gyrometerReading, _startDateTime));
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
            if (GetActivGyrometerList().Count >= _sampleBufferSize)
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
        
        public byte[] ToGyrometerBytes(bool useActiveList)
        {
            IList<GyrometerSample> gyrometerList = useActiveList ? this.GetActivGyrometerList() : this.GetPassivGyrometerList();
            List<byte[]> resultByteArrays = new List<byte[]>();
            var enumerator = gyrometerList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                resultByteArrays.Add(enumerator.Current.ToByteArray());
            }
            return resultByteArrays.SelectMany(a => a).ToArray();
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
                this.AddQuaternionSample(new QuaternionSample(orientationSensorReading, _startDateTime));
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
            if (GetActivQuaternionList().Count >= _sampleBufferSize)
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

        public byte[] ToQuaternionBytes(bool useActiveList)
        {
            IList<QuaternionSample> quaternionList = useActiveList ? this.GetActivQuaternionList() : this.GetPassivQuaternionList();
            List<byte[]> resultByteArrays = new List<byte[]>();
            var enumerator = quaternionList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                resultByteArrays.Add(enumerator.Current.ToByteArray());
            }
            return resultByteArrays.SelectMany(a => a).ToArray();
        }

        //################################################## Geolocation #####################################################
        /// <summary>
        /// Adds a new location sensor reading into the active location sensor reading list.
        /// </summary>
        /// <param name="Geocoordinate"></param>
        public void AddGeolocationReading(Geocoordinate geocoordinateReading)
        {
            if (geocoordinateReading != null)
            {
                if (_startDateTime.CompareTo(DateTimeOffset.MinValue) == 0)
                {
                    _startDateTime = geocoordinateReading.Timestamp;
                }
                this.AddGeolocationSample(new GeolocationSample(_startDateTime, geocoordinateReading));
            }
        }
        /// <summary>
        /// Adds a new location sample into the active location list.
        /// </summary>
        /// <param name="LocationSample"></param>
        private void AddGeolocationSample(GeolocationSample locationSample)
        {
            GetActivGeolocationList().Add(locationSample);
            // Decides whether a list switch is necessary.
            if (GetActivGeolocationList().Count >= _sampleBufferSize)
            {
                SwitchBetweenLists();
            }
        }

        /// <summary>
        /// Returns the location list which is currently used to store new location samples.
        /// </summary>
        /// <returns></returns>
        public List<GeolocationSample> GetActivGeolocationList()
        {
            return _listChangeCounter % 2 == 0 ? _geolocationListEven : _geolocationListOdd;
        }
        /// <summary>
        /// Returns the location list which is currently NOT used store new location samples.
        /// </summary>
        /// <returns></returns>
        public List<GeolocationSample> GetPassivGeolocationList()
        {
            return _listChangeCounter % 2 == 1 ? _geolocationListEven : _geolocationListOdd;
        }

        /// <summary>
        /// Creates a csv string of the location list.
        /// </summary>
        /// <param name="useActiveList">Decides whether active or passive list is used in csv creation.</param>
        /// <returns></returns>
        public byte[] ToGeolocationBytes(bool useActiveList)
        {
            IList<GeolocationSample> locationList = useActiveList ? this.GetActivGeolocationList() : this.GetPassivGeolocationList();
            List<byte[]> resultByteArrays = new List<byte[]>();
            var enumerator = locationList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                resultByteArrays.Add(enumerator.Current.ToByteArray());
            }
            return resultByteArrays.SelectMany(a => a).ToArray();
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
