using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace SensorDataEvaluation.DataModel
{
    public class GyrometerSample
    {
        /// <summary>
        /// long + float + float + float = 8 + 4 + 4 + 4 = 20
        /// </summary>
        public const int AmountOfBytes = 8 + 4 + 4 + 4;
        public static readonly int BytesOfHeaderString = HeaderString.Length * 2;
        public const string HeaderString = "Gyrometer:TimeInTicks(8b),VelocityX(4b),VelocityY(4b),VelocityZ(4b)";

        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public GyrometerSample(TimeSpan measurementTime, float velocityX, float velocityY, float velocityZ) 
        {
            this.MeasurementTime = measurementTime;
            this.VelocityX = velocityX;
            this.VelocityY = velocityY;
            this.VelocityZ = velocityZ;
        }

        public GyrometerSample(GyrometerReading gyrometerReading, DateTimeOffset _startDateTime)
        {
            this.MeasurementTime = gyrometerReading.Timestamp.Subtract(_startDateTime);
            this.VelocityX = Convert.ToSingle(gyrometerReading.AngularVelocityX);
            this.VelocityY = Convert.ToSingle(gyrometerReading.AngularVelocityY);
            this.VelocityZ = Convert.ToSingle(gyrometerReading.AngularVelocityZ);
        }

        public GyrometerSample(byte[] byteArray)
        {
            int i = 0;
            this.MeasurementTime = TimeSpan.FromTicks(BitConverter.ToInt64(byteArray, i));
            i += 8;
            this.VelocityX = BitConverter.ToSingle(byteArray, i);
            i += 4;
            this.VelocityY = BitConverter.ToSingle(byteArray, i);
            i += 4;
            this.VelocityY = BitConverter.ToSingle(byteArray, i);
            i += 4;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        public TimeSpan MeasurementTime { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float VelocityZ { get; set; }

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
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityX));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityY));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityZ));
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
