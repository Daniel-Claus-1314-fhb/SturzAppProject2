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


        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################

        public void AddAllAccelerometerDataFromTupleList(IList<AccelerometerSample> accelerometerDataTupleList)
        {
            //TODO think about to clean accelerometerAnalysisList at first.
            this.AccelerometerAnalysisList.Clear();

            var enumerator = accelerometerDataTupleList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccelerometerSample currentAccelerometerSample = enumerator.Current;
                TimeSpan timeSpan = currentAccelerometerSample.MeasurementTime;
                double accelerometerX = currentAccelerometerSample.CoordianteX;
                double accelerometerY = currentAccelerometerSample.CoordianteY;
                double accelerometerZ = currentAccelerometerSample.CoordianteZ;
                bool isAnalysed = false;
                this.AccelerometerAnalysisList.Add(new object[5] { timeSpan, accelerometerX, accelerometerY, accelerometerZ, isAnalysed });
            }
        }

        public void AddAllGyrometerDataFromTupleList(IList<GyrometerSample> gyrometerDataTupleList)
        {
            //TODO think about to clean gyrometerAnalysisList at first.
            this.GyrometerAnalysisList.Clear();

            var enumerator = gyrometerDataTupleList.GetEnumerator();
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
        }

        public void AddAllQuaternionDataFromTupleList(IList<QuaternionSample> quaternionDataTupleList)
        {
            //TODO think about to clean gyrometerAnalysisList at first.
            this.QuaternionAnalysisList.Clear();

            var enumerator = quaternionDataTupleList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QuaternionSample currentQuaternionSample = enumerator.Current;
                TimeSpan timeSpan = currentQuaternionSample.MeasurementTime;
                double quaternionW = currentQuaternionSample.AngleW;
                double quaternionX = currentQuaternionSample.CoordianteX;
                double quaternionY = currentQuaternionSample.CoordianteY;
                double quaternionZ = currentQuaternionSample.CoordianteZ;
                bool isAnalysed = false;
                this.QuaternionAnalysisList.Add(new object[6] { timeSpan, quaternionW, quaternionX, quaternionY, quaternionZ, isAnalysed });
            }
        }
    }
}
