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
            this.AccelerometerSampleAnalysisList = new List<AccelerometerSample>();
            this.GyrometerSampleAnalysisList = new List<GyrometerSample>();
            this.QuaternionSampleAnalysisList = new List<QuaternionSample>();
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        /// <summary>
        /// List of accelerometer tuples which will analysed.
        /// </summary>
        public List<AccelerometerSample> AccelerometerSampleAnalysisList;

        /// <summary>
        /// List of gyrometer tuples which will analysed.
        /// </summary>
        public List<GyrometerSample> GyrometerSampleAnalysisList;

        /// <summary>
        /// List of quaternion tuples which will analysed.
        /// </summary>
        public List<QuaternionSample> QuaternionSampleAnalysisList;


        //###################################################################################################################
        //################################################## Methods ########################################################
        //###################################################################################################################
        
        public void AddAllAccelerometerAnalysisFromSampleList(List<AccelerometerSample> accelerometerSampleList)
        {
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
