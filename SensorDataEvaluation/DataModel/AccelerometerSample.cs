﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class AccelerometerSample
    {
        /// <summary>
        /// long + double + double + double = 8 + 8 + 8 + 8 = 32
        /// </summary>
        public const int AmountOfBytes = 8 + 8 + 8 + 8;

        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################
        
        public AccelerometerSample(TimeSpan measurementTime, double coordinateX, double coordinateY, double coordinateZ)
        {
            this.MeasurementTime = measurementTime;
            this.CoordinateX = coordinateX;
            this.CoordinateY = coordinateY;
            this.CoordinateZ = coordinateZ;
        }

        public AccelerometerSample(byte[] byteArray)
        {
            int i = 0;
            this.MeasurementTime = TimeSpan.FromTicks(BitConverter.ToInt64(byteArray, i));
            i += 8;
            this.CoordinateX = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.CoordinateY = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.CoordinateZ = BitConverter.ToDouble(byteArray, i);
            i += 8;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        public TimeSpan MeasurementTime { get; set; }
        public double CoordinateX { get; set; }
        public double CoordinateY { get; set; }
        public double CoordinateZ { get; set; }

        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        public byte[] ToByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateX));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateY));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateZ));
            return listOfArrays.SelectMany(a => a).ToArray();
        }

        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Accelerometer,MeasurementTimeInMilliseconds,CoordinateX,CoordinateY,CoordinateZ\n");
        }

        /// <summary>
        /// Is used to create a csv string which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public string ToExportCSVString()
        {
            return String.Format(new CultureInfo("en-US"), "0,{0},{1:f3},{2:f3},{3:f3}\n", this.MeasurementTime.TotalMilliseconds, this.CoordinateX, this.CoordinateY, this.CoordinateZ);
        }

        /// <summary>
        /// Is used to create a byte array which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public byte[] ToExportByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes((short) 0));
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateX));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateY));
            listOfArrays.Add(BitConverter.GetBytes(this.CoordinateZ));
            listOfArrays.Add(BitConverter.GetBytes((char)13));
            return listOfArrays.SelectMany(a => a).ToArray();
        }
    }
}
