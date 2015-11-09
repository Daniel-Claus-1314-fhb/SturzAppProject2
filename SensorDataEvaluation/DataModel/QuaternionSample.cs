
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace SensorDataEvaluation.DataModel
{
    public class QuaternionSample
    {
        /// <summary>
        /// long + float + float + float + float = 8 + 4 + 4 + 4 + 4 = 24
        /// </summary>
        public const int AmountOfBytes = 8 + 4 + 4 + 4 + 4;
        public static readonly int BytesOfHeaderString = HeaderString.Length * 2;
        public const string HeaderString = "Quaternion:TimeInTicks(8b),AngleW(4b),CoordinateX(4b),CoordinateY(4b),CoordinateZ(4b)";

        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public QuaternionSample(TimeSpan measurementTime, float angleW, float coordinateX, float coordinateY, float coordinateZ) 
        {
            this.MeasurementTime = measurementTime;
            this.AngleW = angleW;
            this.CoordinateX = coordinateX;
            this.CoordinateY = coordinateY;
            this.CoordinateZ = coordinateZ;
        }

        public QuaternionSample(OrientationSensorReading orientationSensorReading, DateTimeOffset _startDateTime)
        {
            this.MeasurementTime = orientationSensorReading.Timestamp.Subtract(_startDateTime);
            this.AngleW = orientationSensorReading.Quaternion.W;
            this.CoordinateX = orientationSensorReading.Quaternion.X;
            this.CoordinateY = orientationSensorReading.Quaternion.Y;
            this.CoordinateZ = orientationSensorReading.Quaternion.Z;
        }

        public QuaternionSample(byte[] byteArray)
        {
            int i = 0;
            this.MeasurementTime = TimeSpan.FromTicks(BitConverter.ToInt64(byteArray, i));
            i += 8;
            this.AngleW = BitConverter.ToSingle(byteArray, i);
            i += 4;
            this.CoordinateX = BitConverter.ToSingle(byteArray, i);
            i += 4;
            this.CoordinateY = BitConverter.ToSingle(byteArray, i);
            i += 4;
            this.CoordinateZ = BitConverter.ToSingle(byteArray, i);
            i += 4;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        public TimeSpan MeasurementTime { get; set; }
        public float AngleW { get; set; }
        public float CoordinateX { get; set; }
        public float CoordinateY { get; set; }
        public float CoordinateZ { get; set; }

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
            listOfArrays.Add(BitConverter.GetBytes(this.AngleW));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateX));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateY));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateZ));
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
