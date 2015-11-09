using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace SensorDataEvaluation.DataModel
{
    public class GeolocationSample
    {
        /// <summary>
        /// long + double + double + double = 8 + 8 + 8 + 8 + 8 + 8 = 48
        /// </summary>
        public const int AmountOfBytes = 8 + 8 + 8 + 8 + 8 + 8;
        public static readonly int BytesOfHeaderString = HeaderString.Length * 2;
        public const string HeaderString = "Geolocation:TimeInTicks(8b),Latitude(8b),Longitude(8b),Altitude(8b),AccuracyInMeter(8b),SpeedInMeterPerSecond(8b)";

        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public GeolocationSample (DateTimeOffset _startDateTime, Geocoordinate geocoordinate) 
        {
            this.MeasurementTime = geocoordinate.Timestamp.Subtract(_startDateTime);
            this.Latitude = geocoordinate.Point.Position.Latitude;
            this.Longitude = geocoordinate.Point.Position.Longitude;
            this.Altitude = geocoordinate.Point.Position.Altitude;
            this.Accuracy = geocoordinate.Accuracy;
            this.Speed = geocoordinate.Speed.HasValue ? geocoordinate.Speed.Value : 0d;
        }

        public GeolocationSample(byte[] byteArray)
        {
            int i = 0;
            this.MeasurementTime = TimeSpan.FromTicks(BitConverter.ToInt64(byteArray, i));
            i += 8;
            this.Latitude = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.Longitude = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.Altitude = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.Accuracy = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.Speed = BitConverter.ToDouble(byteArray, i);
            i += 8;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        private TimeSpan MeasurementTime { get; set; }
        private double Latitude { get; set; }
        private double Longitude { get; set; }
        private double Altitude { get; set; }
        public double Accuracy { get; set; }
        public double Speed { get; set; }

        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        /// <summary>
        /// Is used to create a byte array which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.Latitude));
            listOfArrays.Add(BitConverter.GetBytes(this.Longitude));
            listOfArrays.Add(BitConverter.GetBytes(this.Altitude));
            listOfArrays.Add(BitConverter.GetBytes(this.Accuracy));
            listOfArrays.Add(BitConverter.GetBytes(this.Speed));
            return listOfArrays.SelectMany(a => a).ToArray();
        }

        public static string GetExportHeader()
        {
            return HeaderString;
        }

        public static byte[] GetExportDataDescription(int sampleCount)
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes(AmountOfBytes));
            listOfArrays.Add(BitConverter.GetBytes(sampleCount));
            listOfArrays.Add(BitConverter.GetBytes(BytesOfHeaderString));
            return listOfArrays.SelectMany(a => a).ToArray();
        }
    }
}
