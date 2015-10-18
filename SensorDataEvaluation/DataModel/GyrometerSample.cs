using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class GyrometerSample
    {
        /// <summary>
        /// long + double + double + double = 8 + 8 + 8 + 8 = 32
        /// </summary>
        public const int AmountOfBytes = 8 + 8 + 8 + 8;

        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public GyrometerSample(TimeSpan measurementTime, double velocityX, double velocityY, double velocityZ) 
        {
            this.MeasurementTime = measurementTime;
            this.VelocityX = velocityX;
            this.VelocityY = velocityY;
            this.VelocityZ = velocityZ;
        }

        public GyrometerSample(byte[] byteArray)
        {
            int i = 0;
            this.MeasurementTime = TimeSpan.FromTicks(BitConverter.ToInt64(byteArray, i));
            i += 8;
            this.VelocityX = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.VelocityY = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.VelocityY = BitConverter.ToDouble(byteArray, i);
            i += 8;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        public TimeSpan MeasurementTime { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }
        public double VelocityZ { get; set; }

        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        public byte[] ToByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityX));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityY));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityZ));
            return listOfArrays.SelectMany(a => a).ToArray();
        }

        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Gyrometer,MeasurementTimeInMilliseconds,VelocityX,VelocityY,VelocityZ\n");
        }

        /// <summary>
        /// Is used to create a csv string which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public string ToExportCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "1,{0},{1:f3},{2:f3},{3:f3}\n", this.MeasurementTime.TotalMilliseconds, this.VelocityX, this.VelocityY, this.VelocityZ);
        }

        /// <summary>
        /// Is used to create a byte array which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public byte[] ToExportByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes((short) 1));
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityX));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityY));
            listOfArrays.Add(BitConverter.GetBytes(this.VelocityZ));
            listOfArrays.Add(BitConverter.GetBytes((char)13));
            return listOfArrays.SelectMany(a => a).ToArray();
        }
    }
}
