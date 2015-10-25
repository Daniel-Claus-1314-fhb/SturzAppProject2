
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
        
        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Quaternion(2byte),MeasurementTimeInTicks(8byte),AngleW(4byte),CoordinateX(4byte),CoordinateY(4byte),CoordinateZ(4byte)\n");
        }

        /// <summary>
        /// Is used to create a csv string which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public string ToExportCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "2,{0},{1:f3},{2:f3},{3:f3},{4:f3}\n", this.MeasurementTime.Ticks, this.AngleW, this.CoordinateX, this.CoordinateY, this.CoordinateZ);
        }

        /// <summary>
        /// Is used to create a byte array which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public byte[] ToExportByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes((short) 2));
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.AngleW));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateX));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateY));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateZ));
            listOfArrays.Add(BitConverter.GetBytes((char)13));
            return listOfArrays.SelectMany(a => a).ToArray();
        }
    }
}
