using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public class ExportData
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public ExportData()
        {
            this.AccelerometerSamples = new List<AccelerometerSample>();
            this.GyrometerSamples = new List<GyrometerSample>();
            this.QuaternionSamples = new List<QuaternionSample>();
            this.EvaluationSamples = new List<EvaluationSample>();
        }

        public ExportData(List<AccelerometerSample> accelerometerSamples, List<GyrometerSample> gyrometerSamples, List<QuaternionSample> quaternionSamples, List<EvaluationSample> evaluationSamples)
        {
            this.AccelerometerSamples = accelerometerSamples;
            this.GyrometerSamples = gyrometerSamples;
            this.QuaternionSamples = quaternionSamples;
            this.EvaluationSamples = evaluationSamples;
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

        #endregion

        //###################################################################################
        //################################### Methods #######################################
        //###################################################################################

        #region Methods

        public string ToExportCSVString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Append accelerometer header and accelerometer samples
            if (this.AccelerometerSamples != null && this.AccelerometerSamples.Count > 0)
            {
                stringBuilder.Append(AccelerometerSamples.ElementAt(0).GetExportHeader());

                var enumerator = AccelerometerSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AccelerometerSample accelerometerSample = enumerator.Current;
                    stringBuilder.Append(accelerometerSample.ToExportCSVString());
                }
            }

            // Append gyrometer header and gyrometer samples
            if (this.GyrometerSamples != null && this.GyrometerSamples.Count > 0)
            {
                stringBuilder.Append(GyrometerSamples.ElementAt(0).GetExportHeader());

                var enumerator = GyrometerSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GyrometerSample gyrometerSample = enumerator.Current;
                    stringBuilder.Append(gyrometerSample.ToExportCSVString());
                }
            }

            // Append quaternion header and quaternion samples
            if (this.QuaternionSamples != null && this.QuaternionSamples.Count > 0)
            {
                stringBuilder.Append(QuaternionSamples.ElementAt(0).GetExportHeader());

                var enumerator = QuaternionSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    QuaternionSample quaternionSample = enumerator.Current;
                    stringBuilder.Append(quaternionSample.ToExportCSVString());
                }
            }

            // Append evaluation header and evaluation samples
            if (this.EvaluationSamples != null && this.EvaluationSamples.Count > 0)
            {
                stringBuilder.Append(EvaluationSamples.ElementAt(0).GetExportHeader());

                var enumerator = EvaluationSamples.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    EvaluationSample evaluationSample = enumerator.Current;
                    stringBuilder.Append(evaluationSample.ToExportCSVString());
                }
            }
            return stringBuilder.ToString();
        }

        #endregion
    }
}
