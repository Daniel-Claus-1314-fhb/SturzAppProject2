﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class EvaluationSample
    {
        /// <summary>
        /// long + double + double + bool + bool + bool = 8 + 8 + 8 + 1 + 1 + 1 = 27
        /// </summary>
        public const int AmountOfBytes = 8 + 8 + 8 + 1 + 1 + 1;

        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public EvaluationSample(TimeSpan measurementTime, double accelerometerVectorLength, double gyrometerVectorLength)
        {
            this.MeasurementTime = measurementTime;
            this.AccelerometerVectorLength = accelerometerVectorLength;
            this.GyrometerVectorLength = gyrometerVectorLength;
            this.IsDetectedStep = false;
        }

        public EvaluationSample(TimeSpan measurementTime, double accelerometerVectorLength, double gyrometerVectorLength, 
            bool isAssumedAccelerometerStep, bool isAssumedGyrometerStep, bool isDetectedStep)
            : this(measurementTime, accelerometerVectorLength, gyrometerVectorLength)
        {
            this.IsAssumedAccelerometerStep = isAssumedAccelerometerStep;
            this.IsAssumedGyrometerStep = isAssumedGyrometerStep;
            this.IsDetectedStep = isDetectedStep;
        }

        public EvaluationSample(byte[] byteArray)
        {
            int i = 0;
            this.MeasurementTime = TimeSpan.FromTicks(BitConverter.ToInt64(byteArray, i));
            i += 8;
            this.AccelerometerVectorLength = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.GyrometerVectorLength = BitConverter.ToDouble(byteArray, i);
            i += 8;
            this.IsAssumedAccelerometerStep = BitConverter.ToBoolean(byteArray, i);
            i += 1;
            this.IsAssumedGyrometerStep = BitConverter.ToBoolean(byteArray, i);
            i += 1;
            this.IsDetectedStep = BitConverter.ToBoolean(byteArray, i);
            i += 1;
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        public TimeSpan MeasurementTime { get; set; }
        public double AccelerometerVectorLength { get; set; }
        public double GyrometerVectorLength { get; set; }
        public bool IsAssumedAccelerometerStep { get; set; }
        public bool IsAssumedGyrometerStep { get; set; }
        public bool IsDetectedStep { get; set; }

        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        public byte[] ToByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.AccelerometerVectorLength));
            listOfArrays.Add(BitConverter.GetBytes(this.GyrometerVectorLength));
            listOfArrays.Add(BitConverter.GetBytes(this.IsAssumedAccelerometerStep));
            listOfArrays.Add(BitConverter.GetBytes(this.IsAssumedGyrometerStep));
            listOfArrays.Add(BitConverter.GetBytes(this.IsDetectedStep));
            return listOfArrays.SelectMany(a => a).ToArray();
        }

        public string GetExportHeader()
        {
            return String.Format(new CultureInfo("en-US"), "Evaluation,MeasurementTimeInMilliseconds,AccelerometerVectorLength,GyrometerVectorLength,AccelerometerPeak,GyrometerPeak,DetectedStep\n");
        }

        /// <summary>
        /// Is used to create a csv string which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public string ToExportCSVString()
        {
            return String.Format(String.Format(new CultureInfo("en-US"), "3,{0},{1:f3},{2:f3},{3:g},{4:g},{5:g}\n",
                    this.MeasurementTime.TotalMilliseconds, this.AccelerometerVectorLength, this.GyrometerVectorLength,
                    this.IsAssumedAccelerometerStep ? 1 : 0, this.IsAssumedGyrometerStep ? 1 : 0, this.IsDetectedStep ? 1 : 0));
        }

        /// <summary>
        /// Is used to create a byte array which represents the current EvaluationSample.
        /// </summary>
        /// <returns></returns>
        public byte[] ToExportByteArray()
        {
            List<byte[]> listOfArrays = new List<byte[]>();
            listOfArrays.Add(BitConverter.GetBytes((short) 3));
            listOfArrays.Add(BitConverter.GetBytes(this.MeasurementTime.Ticks));
            listOfArrays.Add(BitConverter.GetBytes(this.AccelerometerVectorLength));
            listOfArrays.Add(BitConverter.GetBytes(this.GyrometerVectorLength));
            listOfArrays.Add(BitConverter.GetBytes(this.IsAssumedAccelerometerStep));
            listOfArrays.Add(BitConverter.GetBytes(this.IsAssumedGyrometerStep));
            listOfArrays.Add(BitConverter.GetBytes(this.IsDetectedStep));
            listOfArrays.Add(BitConverter.GetBytes((char)13));
            return listOfArrays.SelectMany(a => a).ToArray();
        }
    }
}
