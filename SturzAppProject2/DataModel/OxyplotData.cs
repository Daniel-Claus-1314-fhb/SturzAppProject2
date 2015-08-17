using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
            AccelerometerReadings = new List<Tuple<TimeSpan, double, double, double>>();
            GyrometerReadings = new List<Tuple<TimeSpan, double, double, double>>();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public List<Tuple<TimeSpan, double, double, double>> AccelerometerReadings { get; set; }
        public List<Tuple<TimeSpan, double, double, double>> GyrometerReadings { get; set; }

        public bool HasAccelerometerReadings
        {
            get { return this.AccelerometerReadings != null && this.AccelerometerReadings.Count > 0 ? true : false; }
        }
        public bool HasGyrometerReadings
        {
            get { return this.GyrometerReadings != null && this.GyrometerReadings.Count > 0 ? true : false; }
        }

        public LineSeries AccelerometerXLineSeries
        {
            get
            {
                LineSeries acclerometerXLineSeries = new LineSeries();
                acclerometerXLineSeries.Title = "AccX";

                if (this.AccelerometerReadings != null && this.AccelerometerReadings.Count > 0)
                {
                    TimeSpan startTimeStamp = this.AccelerometerReadings.ElementAt(0).Item1;
                    foreach (Tuple<TimeSpan, double, double, double> accelerometerReading in this.AccelerometerReadings)
                    {
                        acclerometerXLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(accelerometerReading.Item1 - startTimeStamp), accelerometerReading.Item2));
                    }
                }
                return acclerometerXLineSeries;
            }
        }

        public LineSeries AccelerometerYLineSeries
        {
            get
            {
                LineSeries acclerometerYLineSeries = new LineSeries();
                acclerometerYLineSeries.Title = "AccY";

                if (this.AccelerometerReadings != null && this.AccelerometerReadings.Count > 0)
                {
                    TimeSpan startTimeStamp = this.AccelerometerReadings.ElementAt(0).Item1;
                    foreach (Tuple<TimeSpan, double, double, double> accelerometerReading in this.AccelerometerReadings)
                    {
                        acclerometerYLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(accelerometerReading.Item1 - startTimeStamp), accelerometerReading.Item3));
                    }
                }
                return acclerometerYLineSeries;
            }
        }

        public LineSeries AccelerometerZLineSeries
        {
            get
            {
                LineSeries acclerometerZLineSeries = new LineSeries();
                acclerometerZLineSeries.Title = "AccZ";

                if (this.AccelerometerReadings != null && this.AccelerometerReadings.Count > 0)
                {
                    TimeSpan startTimeStamp = this.AccelerometerReadings.ElementAt(0).Item1;
                    foreach (Tuple<TimeSpan, double, double, double> accelerometerReading in this.AccelerometerReadings)
                    {
                        acclerometerZLineSeries.Points.Add(new DataPoint(TimeSpanAxis.ToDouble(accelerometerReading.Item1 - startTimeStamp), accelerometerReading.Item4));
                    }
                }
                return acclerometerZLineSeries;
            }
        }

        #endregion
    }
}
