using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class EvaluationDataModel
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public EvaluationDataModel()
        {
            this.AccelerometerAnalysisList = new List<object[]>();
            this.GyrometerAnalysisList = new List<object[]>();
            this.QuaternionAnalysisList = new List<object[]>();

            this.AccelerometerSampleAnalysisList = new List<AccelerometerSample>();
            this.GyrometerSampleAnalysisList = new List<GyrometerSample>();
            this.QuaternionSampleAnalysisList = new List<QuaternionSample>();
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        /// <summary>
        /// List of accelerometer tuples which will analysed.
        /// 
        /// object[0]: timespan since the start of measurement. (TimeSpan)
        /// object[1]: X coordinate of the accelerometer tuple. (double)
        /// object[2]: Y coorfinate of the accelerometer tuple. (double)
        /// object[3]: Z coordinate of the accelerometer tuple. (double)
        /// object[4]: is accelerometer tuple already processed by analysis. (bool)
        /// </summary>
        public List<object[]> AccelerometerAnalysisList;
        public List<AccelerometerSample> AccelerometerSampleAnalysisList;

        /// <summary>
        /// List of gyrometer tuples which will analysed.
        /// 
        /// object[0]: timespan since the start of measurement. (TimeSpan)
        /// object[1]: X angle velocity of the gyrometer tuple. (double)
        /// object[2]: Y angle velocity of the gyrometer tuple. (double)
        /// object[3]: Z angle velocity of the gyrometer tuple. (double)
        /// object[4]: is gyrometer tuple already processed by analysis. (bool)
        /// </summary>
        public List<object[]> GyrometerAnalysisList;
        public List<GyrometerSample> GyrometerSampleAnalysisList;

        /// <summary>
        /// List of quaternion tuples which will analysed.
        /// 
        /// object[0]: timespan since the start of measurement. (TimeSpan)
        /// object[1]: W rptation angle of the quaternion tuple. (double)
        /// object[2]: X vector coordinate of the quaternion tuple. (double)
        /// object[3]: Y vector coordinate of the quaternion tuple. (double)
        /// object[4]: Z vector coordinate of the quaternion tuple. (double)
        /// object[5]: is quaternion tuple already processed by analysis. (bool)
        /// </summary>
        public List<object[]> QuaternionAnalysisList;
        public List<QuaternionSample> QuaternionSampleAnalysisList;


        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################
        
        public void AddAllAccelerometerAnalysisFromSampleList(List<AccelerometerSample> accelerometerSampleList)
        {
            //TODO think about to clean accelerometerAnalysisList at first.
            this.AccelerometerAnalysisList.Clear();

            var enumerator = accelerometerSampleList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccelerometerSample currentAccelerometerSample = enumerator.Current;
                TimeSpan timeSpan = currentAccelerometerSample.MeasurementTime;
                double accelerometerX = currentAccelerometerSample.CoordinateX;
                double accelerometerY = currentAccelerometerSample.CoordinateY;
                double accelerometerZ = currentAccelerometerSample.CoordinateZ;
                bool isAnalysed = false;
                this.AccelerometerAnalysisList.Add(new object[5] { timeSpan, accelerometerX, accelerometerY, accelerometerZ, isAnalysed });
            }

            this.AccelerometerSampleAnalysisList.Clear();

            var enumerator2 = accelerometerSampleList.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                AccelerometerSample currentAccelerometerSample = enumerator2.Current;
                TimeSpan timeSpan = currentAccelerometerSample.MeasurementTime;
                double accelerometerX = currentAccelerometerSample.CoordinateX;
                double accelerometerY = currentAccelerometerSample.CoordinateY;
                double accelerometerZ = currentAccelerometerSample.CoordinateZ;
                this.AccelerometerSampleAnalysisList.Add(new AccelerometerSample(timeSpan, accelerometerX, accelerometerY, accelerometerZ));
            }
        }

        public void AddAllGyrometerAnalysisFromSampleList(List<GyrometerSample> gyrometerSampleList)
        {
            //TODO think about to clean gyrometerAnalysisList at first.
            this.GyrometerAnalysisList.Clear();

            var enumerator = gyrometerSampleList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GyrometerSample currentGyrometerSample = enumerator.Current;
                TimeSpan timeSpan = currentGyrometerSample.MeasurementTime;
                double gyrometerX = currentGyrometerSample.VelocityX;
                double gyrometerY = currentGyrometerSample.VelocityY;
                double gyrometerZ = currentGyrometerSample.VelocityZ;
                bool isAnalysed = false;
                this.GyrometerAnalysisList.Add(new object[5] { timeSpan, gyrometerX, gyrometerY, gyrometerZ, isAnalysed });
            }

            this.GyrometerSampleAnalysisList.Clear();

            var enumerator2 = gyrometerSampleList.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                GyrometerSample currentGyrometerSample = enumerator2.Current;
                TimeSpan timeSpan = currentGyrometerSample.MeasurementTime;
                double velocityX = currentGyrometerSample.VelocityX;
                double velocityY = currentGyrometerSample.VelocityY;
                double velocityZ = currentGyrometerSample.VelocityZ;
                this.GyrometerSampleAnalysisList.Add(new GyrometerSample(timeSpan, velocityX, velocityY, velocityZ));
            }
        }

        public void AddAllQuaternionAnalysisFromSampleList(List<QuaternionSample> quaternionSampleList)
        {
            //TODO think about to clean gyrometerAnalysisList at first.
            this.QuaternionAnalysisList.Clear();

            var enumerator = quaternionSampleList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QuaternionSample currentQuaternionSample = enumerator.Current;
                TimeSpan timeSpan = currentQuaternionSample.MeasurementTime;
                double quaternionW = currentQuaternionSample.AngleW;
                double quaternionX = currentQuaternionSample.CoordinateX;
                double quaternionY = currentQuaternionSample.CoordinateY;
                double quaternionZ = currentQuaternionSample.CoordinateZ;
                bool isAnalysed = false;
                this.QuaternionAnalysisList.Add(new object[6] { timeSpan, quaternionW, quaternionX, quaternionY, quaternionZ, isAnalysed });
            } 
            
            this.QuaternionSampleAnalysisList.Clear();

            var enumerator2 = quaternionSampleList.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                QuaternionSample currentQuaternionSample = enumerator2.Current;
                TimeSpan timeSpan = currentQuaternionSample.MeasurementTime;
                double angleW = currentQuaternionSample.AngleW;
                double coordinateX = currentQuaternionSample.CoordinateX;
                double coordinateY = currentQuaternionSample.CoordinateY;
                double coordinateZ = currentQuaternionSample.CoordinateZ;
                this.QuaternionSampleAnalysisList.Add(new QuaternionSample(timeSpan, angleW, coordinateX, coordinateY, coordinateZ));
            }
        }
    }
}
