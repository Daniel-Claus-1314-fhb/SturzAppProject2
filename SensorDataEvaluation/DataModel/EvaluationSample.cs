using System;
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
        /// long + double + double + bool + bool + bool = 8 + 4 + 4 + 1 + 1 + 1 = 19
        /// </summary>
        public const int AmountOfBytes = 8 + 4 + 4 + 1 + 1 + 1;
        public static readonly int BytesOfHeaderString = HeaderString.Length * 2;
        public const string HeaderString = "Evaluation:TimeInTicks(8b),AccelerometerVectorLength(4b),GyrometerVectorLength(4b),AccelerometerPeak(1b),GyrometerPeak(1b),DetectedStep(1b)";

        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public EvaluationSample(TimeSpan measurementTime, float accelerometerVectorLength, float gyrometerVectorLength)
        {
            this.MeasurementTime = measurementTime;
            this.AccelerometerVectorLength = accelerometerVectorLength;
            this.GyrometerVectorLength = gyrometerVectorLength;
            this.IsDetectedStep = false;
        }

        public EvaluationSample(TimeSpan measurementTime, float accelerometerVectorLength, float gyrometerVectorLength, 
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
            this.AccelerometerVectorLength = BitConverter.ToSingle(byteArray, i);
            i += 4;
            this.GyrometerVectorLength = BitConverter.ToSingle(byteArray, i);
            i += 4;
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
        public float AccelerometerVectorLength { get; set; }
        public float GyrometerVectorLength { get; set; }
        public bool IsAssumedAccelerometerStep { get; set; }
        public bool IsAssumedGyrometerStep { get; set; }
        public bool IsDetectedStep { get; set; }

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
            listOfArrays.Add(BitConverter.GetBytes(this.AccelerometerVectorLength));
            listOfArrays.Add(BitConverter.GetBytes(this.GyrometerVectorLength));
            listOfArrays.Add(BitConverter.GetBytes(this.IsAssumedAccelerometerStep));
            listOfArrays.Add(BitConverter.GetBytes(this.IsAssumedGyrometerStep));
            listOfArrays.Add(BitConverter.GetBytes(this.IsDetectedStep));
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
