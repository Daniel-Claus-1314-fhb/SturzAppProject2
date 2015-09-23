using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;

namespace BackgroundTask.DataModel
{
    public class OxyplotData
    {
        
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public OxyplotData()
        {
            this.AccelerometerSamples = new List<AccelerometerSample>();
            this.EvaluationSamples = new List<EvaluationSample>();
            this.GyrometerSamples = new List<GyrometerSample>();
            this.QuaternionSamples = new List<QuaternionSample>();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public List<AccelerometerSample> AccelerometerSamples { get; set; }
        public List<GyrometerSample> GyrometerSamples { get; set; }
        public List<QuaternionSample> QuaternionSamples { get; set; }
        public List<EvaluationSample> EvaluationSamples { get; set; }

        public bool HasAccelerometerSamples
        {
            get { return this.AccelerometerSamples != null && this.AccelerometerSamples.Count > 0 ? true : false; }
        }
        public bool HasGyrometerSamples
        {
            get { return this.GyrometerSamples != null && this.GyrometerSamples.Count > 0 ? true : false; }
        }
        public bool HasQuaternionSamples
        {
            get { return this.QuaternionSamples != null && this.QuaternionSamples.Count > 0 ? true : false; }
        }
        public bool HasEvaluationSamples
        {
            get { return this.EvaluationSamples != null && this.EvaluationSamples.Count > 0 ? true : false; }
        }

        #endregion

        //###################################################################################
        //################################### Methods #######################################
        //###################################################################################

        #region Methods       

        public LineSeries GetAccelerometerXLineSeries()
        {
            LineSeries acclerometerXLineSeries = new LineSeries();
            acclerometerXLineSeries.Title = "AccelerometerX";

            var enumerator = this.AccelerometerSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccelerometerSample currentAccelerometerSample = enumerator.Current;
                acclerometerXLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentAccelerometerSample.MeasurementTime), currentAccelerometerSample.CoordinateX));
            }
            return acclerometerXLineSeries;
        }

        public LineSeries GetAccelerometerYLineSeries()
        {
            LineSeries acclerometerYLineSeries = new LineSeries();
            acclerometerYLineSeries.Title = "AccelerometerY";

            var enumerator = this.AccelerometerSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccelerometerSample currentAccelerometerSample = enumerator.Current;
                acclerometerYLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentAccelerometerSample.MeasurementTime), currentAccelerometerSample.CoordinateY));
            }
            return acclerometerYLineSeries;
        }

        public LineSeries GetAccelerometerZLineSeries()
        {
            LineSeries acclerometerZLineSeries = new LineSeries();
            acclerometerZLineSeries.Title = "AccelerometerZ";

            var enumerator = this.AccelerometerSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccelerometerSample currentAccelerometerSample = enumerator.Current;
                acclerometerZLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentAccelerometerSample.MeasurementTime), currentAccelerometerSample.CoordinateZ));
            }
            return acclerometerZLineSeries;
        }

        public LineSeries GetAccelerometerVectorLengthLineSeries()
        {
            LineSeries accelerometerVectorLengthLineSeries = new LineSeries();
            accelerometerVectorLengthLineSeries.Title = "Vektorlänge";

            var enumerator = this.EvaluationSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EvaluationSample currentEvaluationSample = enumerator.Current;
                accelerometerVectorLengthLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentEvaluationSample.MeasurementTime), currentEvaluationSample.AccelerometerVectorLength));
            }
            return accelerometerVectorLengthLineSeries;
        }

        public LineSeries GetAccelerometerStepLineSeries()
        {
            LineSeries accelerometerStepLineSeries = new LineSeries();
            accelerometerStepLineSeries.Title = "Schritte";

            var enumerator = this.EvaluationSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EvaluationSample currentEvaluationSample = enumerator.Current;
                accelerometerStepLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentEvaluationSample.MeasurementTime), currentEvaluationSample.IsStepDetected ? 1 : 0));
            }
            return accelerometerStepLineSeries;
        }

        #endregion
    }
}
