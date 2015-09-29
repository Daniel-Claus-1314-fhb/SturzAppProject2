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

        //###################################################################################
        //################################### Accelerometer LineSeries ######################
        //###################################################################################

        public LineSeries GetAccelerometerXLineSeries()
        {
            LineSeries acclerometerXLineSeries = new LineSeries();
            acclerometerXLineSeries.Title = "Accelerometer X";

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
            acclerometerYLineSeries.Title = "Accelerometer Y";

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
            acclerometerZLineSeries.Title = "Accelerometer Z";

            var enumerator = this.AccelerometerSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AccelerometerSample currentAccelerometerSample = enumerator.Current;
                acclerometerZLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentAccelerometerSample.MeasurementTime), currentAccelerometerSample.CoordinateZ));
            }
            return acclerometerZLineSeries;
        }

        //###################################################################################
        //################################### Gyrometer LineSeries ##########################
        //###################################################################################

        public LineSeries GetGyrometerXLineSeries()
        {
            LineSeries gyrometerXLineSeries = new LineSeries();
            gyrometerXLineSeries.Title = "Gyrometer X";

            var enumerator = this.GyrometerSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GyrometerSample currentGyrometerSample = enumerator.Current;
                gyrometerXLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentGyrometerSample.MeasurementTime), currentGyrometerSample.VelocityX));
            }
            return gyrometerXLineSeries;
        }

        public LineSeries GetGyrometerYLineSeries()
        {
            LineSeries gyrometerYLineSeries = new LineSeries();
            gyrometerYLineSeries.Title = "Gyrometer Y";

            var enumerator = this.GyrometerSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GyrometerSample currentGyrometerSample = enumerator.Current;
                gyrometerYLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentGyrometerSample.MeasurementTime), currentGyrometerSample.VelocityY));
            }
            return gyrometerYLineSeries;
        }

        public LineSeries GetGyrometerZLineSeries()
        {
            LineSeries gyrometerZLineSeries = new LineSeries();
            gyrometerZLineSeries.Title = "Gyrometer Z";

            var enumerator = this.GyrometerSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GyrometerSample currentGyrometerSample = enumerator.Current;
                gyrometerZLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentGyrometerSample.MeasurementTime), currentGyrometerSample.VelocityZ));
            }
            return gyrometerZLineSeries;
        }

        //###################################################################################
        //################################### Quaternion LineSeries #########################
        //###################################################################################

        public LineSeries GetQuaterionWLineSeries()
        {
            LineSeries QuaterionWLineSeries = new LineSeries();
            QuaterionWLineSeries.Title = "Quaterion angle W";

            var enumerator = this.QuaternionSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QuaternionSample currentQuaternionSample = enumerator.Current;
                QuaterionWLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentQuaternionSample.MeasurementTime), currentQuaternionSample.AngleW));
            }
            return QuaterionWLineSeries;
        }

        public LineSeries GetQuaterionXLineSeries()
        {
            LineSeries QuaterionXLineSeries = new LineSeries();
            QuaterionXLineSeries.Title = "Quaterion X";

            var enumerator = this.QuaternionSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QuaternionSample currentQuaternionSample = enumerator.Current;
                QuaterionXLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentQuaternionSample.MeasurementTime), currentQuaternionSample.CoordinateX));
            }
            return QuaterionXLineSeries;
        }

        public LineSeries GetQuaterionYLineSeries()
        {
            LineSeries QuaterionYLineSeries = new LineSeries();
            QuaterionYLineSeries.Title = "Quaterion Y";

            var enumerator = this.QuaternionSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QuaternionSample currentQuaternionSample = enumerator.Current;
                QuaterionYLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentQuaternionSample.MeasurementTime), currentQuaternionSample.CoordinateY));
            }
            return QuaterionYLineSeries;
        }

        public LineSeries GetQuaterionZLineSeries()
        {
            LineSeries QuaterionZLineSeries = new LineSeries();
            QuaterionZLineSeries.Title = "Quaterion Z";

            var enumerator = this.QuaternionSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                QuaternionSample currentQuaternionSample = enumerator.Current;
                QuaterionZLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentQuaternionSample.MeasurementTime), currentQuaternionSample.CoordinateZ));
            }
            return QuaterionZLineSeries;
        }

        //###################################################################################
        //################################### Evaluation LineSeries #########################
        //###################################################################################

        public LineSeries GetAccelerometerVectorLengthLineSeries()
        {
            LineSeries accelerometerVectorLengthLineSeries = new LineSeries();
            accelerometerVectorLengthLineSeries.Title = "Accelerometer Vektorlänge";

            var enumerator = this.EvaluationSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EvaluationSample currentEvaluationSample = enumerator.Current;
                accelerometerVectorLengthLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentEvaluationSample.MeasurementTime), currentEvaluationSample.AccelerometerVectorLength));
            }
            return accelerometerVectorLengthLineSeries;
        }

        public LineSeries GetGyrometerVectorLengthLineSeries()
        {
            LineSeries gyrometerVectorLengthLineSeries = new LineSeries();
            gyrometerVectorLengthLineSeries.Title = "Gyrometer Vektorlänge";

            var enumerator = this.EvaluationSamples.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EvaluationSample currentEvaluationSample = enumerator.Current;
                gyrometerVectorLengthLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(currentEvaluationSample.MeasurementTime), currentEvaluationSample.GyrometerVectorLength));
            }
            return gyrometerVectorLengthLineSeries;
        }

        public LineSeries GetAssumedAccelerometerStepLineSeries()
        {
            LineSeries accelerometerStepLineSeries = new LineSeries();
            accelerometerStepLineSeries.Title = "Accelerometer Peaks";

            for (int i = 0; i < this.EvaluationSamples.Count; i++)
            {
                bool setPointToLineSeries = false;

                // Tritt ein, wenn das erste oder das letzte Evaluierungssample ausgewählt ist.
                if (i == 0 || i == this.EvaluationSamples.Count - 1)
                {
                    setPointToLineSeries = true;
                }
                // Tritt ein, wenn das aktuelle Evaluierungssample einen vermuteten Schritt besitzt.
                else if (this.EvaluationSamples.ElementAt(i).IsAssumedAccelerometerStep)
                {
                    setPointToLineSeries = true;
                }
                // Tritt ein, wenn das nachfolgende Evaluierungssample einen vermuteten Schritte besitzen wird.
                // oder
                // Tritt ein, wenn das vorherige Evaluierungssample einen vermuteten Schritte besäßen hat.
                else if ((i < this.EvaluationSamples.Count - 1 && this.EvaluationSamples.ElementAt(i + 1).IsAssumedAccelerometerStep) ||
                        (i > 0 && this.EvaluationSamples.ElementAt(i - 1).IsAssumedAccelerometerStep))
                {
                    setPointToLineSeries = true;
                }

                if (setPointToLineSeries)
                {
                    accelerometerStepLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(this.EvaluationSamples.ElementAt(i).MeasurementTime), this.EvaluationSamples.ElementAt(i).IsAssumedAccelerometerStep ? 1d : 0d));
                }
            }
            return accelerometerStepLineSeries;
        }

        public LineSeries GetAssumedGyrometerStepLineSeries()
        {
            LineSeries gyrometerStepLineSeries = new LineSeries();
            gyrometerStepLineSeries.Title = "Gyrometer Peaks";

            for (int i = 0; i < this.EvaluationSamples.Count; i++)
            {
                bool setPointToLineSeries = false;

                // Tritt ein, wenn das erste oder das letzte Evaluierungssample ausgewählt ist.
                if (i == 0 || i == this.EvaluationSamples.Count - 1)
                {
                    setPointToLineSeries = true;
                }
                // Tritt ein, wenn das aktuelle Evaluierungssample einen vermuteten Schritt besitzt.
                else if (this.EvaluationSamples.ElementAt(i).IsAssumedGyrometerStep)
                {
                    setPointToLineSeries = true;
                }
                // Tritt ein, wenn das nachfolgende Evaluierungssample einen vermuteten Schritte besitzen wird.
                // oder
                // Tritt ein, wenn das vorherige Evaluierungssample einen vermuteten Schritte besäßen hat.
                else if ((i < this.EvaluationSamples.Count - 1 && this.EvaluationSamples.ElementAt(i + 1).IsAssumedGyrometerStep) ||
                        (i > 0 && this.EvaluationSamples.ElementAt(i - 1).IsAssumedGyrometerStep))
                {
                    setPointToLineSeries = true;
                }

                if (setPointToLineSeries)
                {
                    gyrometerStepLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(this.EvaluationSamples.ElementAt(i).MeasurementTime), this.EvaluationSamples.ElementAt(i).IsAssumedGyrometerStep ? -1d : 0d));
                }
            }
            return gyrometerStepLineSeries;
        }

        public LineSeries GetDetectedStepLineSeries()
        {
            LineSeries stepLineSeries = new LineSeries();
            stepLineSeries.Title = "Als Schritte erkannte Peaks";

            for (int i = 0; i < this.EvaluationSamples.Count; i++)
            {
                bool setPointToLineSeries = false;

                // Tritt ein, wenn das erste oder das letzte Evaluierungssample ausgewählt ist.
                if (i == 0 || i == this.EvaluationSamples.Count - 1)
                {
                    setPointToLineSeries = true;
                }
                // Tritt ein, wenn das aktuelle Evaluierungssample einen Schritt besitzt.
                else if (this.EvaluationSamples.ElementAt(i).IsDetectedStep)
                {
                    setPointToLineSeries = true;
                }
                // Tritt ein, wenn das nachfolgende Evaluierungssample einen Schritte besitzen wird.
                // oder
                // Tritt ein, wenn das vorherige Evaluierungssample einen Schritte besäßen hat.
                else if ((i < this.EvaluationSamples.Count - 1 && this.EvaluationSamples.ElementAt(i + 1).IsDetectedStep) ||
                        (i > 0 && this.EvaluationSamples.ElementAt(i - 1).IsDetectedStep))
                {
                    setPointToLineSeries = true;
                }

                if (setPointToLineSeries)
                {
                    stepLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(this.EvaluationSamples.ElementAt(i).MeasurementTime), this.EvaluationSamples.ElementAt(i).IsDetectedStep ? 0.7d : 0d));
                }
            }
            return stepLineSeries;
        }

        #endregion
    }
}
