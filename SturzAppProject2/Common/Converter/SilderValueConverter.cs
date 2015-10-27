using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BackgroundTask.Common.Converter
{
    class SilderValueConverter : IValueConverter
    {
        private const string reportIntervalParam = "ReportInterval";
        private const string gpsReportIntervalParam = "GPSReportInterval";
        private const string processedSampleCountParam = "ProcessedSampleCount";
        private const string AccelerometerThresholdParam = "AccelerometerThreshold";
        private const string GyrometerThresholdParam = "GyrometerThreshold";
        private const string StepDistanceParam = "StepDistance";
        private const string PeakJoinDistanceParam = "PeakJoinDistance";

        private readonly uint[] reportInterval = new uint[6] { 10, 16, 20, 40, 50, 100 };
        private readonly uint[] gpsReportInterval = new uint[7] { 1, 2, 5, 10, 15, 20, 30 };
        private readonly uint[] processedSampleCount = new uint[7] { 100, 200, 250, 500, 1000, 2000, 5000 };
        private readonly double[] accelerometerThreshold = new double[13] { 1.2d, 1.3d, 1.35d, 1.4d, 1.45d, 1.5d, 1.55d, 1.6d, 1.65d, 1.7d, 1.75d, 1.8d, 1.9d };
        private readonly double[] gyrometerThreshold = new double[13] { 50d, 75d, 100d, 125d, 150d, 175d, 200d, 225d, 250d, 275d, 300d, 325d, 350d };
        private readonly uint[] stepDistance = new uint[9] { 100, 150, 200, 250, 300, 350, 400, 450, 500};
        private readonly uint[] peakJoinDistance = new uint[6] { 50, 100, 150, 200, 250, 300 };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                if (value.GetType().Equals(typeof(uint)))
                {
                    uint convertValue = (uint)value;
                    double resultValue = 0d;

                    if (parameter != null)
                    {
                        string parameterString = parameter as string;

                        switch (parameterString)
                        {
                            case reportIntervalParam:
                                resultValue = (double) Array.IndexOf(reportInterval, convertValue);
                                break;
                            case gpsReportIntervalParam:
                                resultValue = (double)Array.IndexOf(gpsReportInterval, convertValue);
                                break;
                            case processedSampleCountParam:
                                resultValue = (double) Array.IndexOf(processedSampleCount, convertValue);
                                break;
                            case StepDistanceParam:
                                resultValue = (double) Array.IndexOf(stepDistance, convertValue);
                                break;
                            case PeakJoinDistanceParam:
                                resultValue = (double)Array.IndexOf(peakJoinDistance, convertValue);
                                break;
                        }
                    }
                    return resultValue;
                } 
                else if (value.GetType().Equals(typeof(double)))
                {
                    double convertValue = (double)value;
                    double resultValue = 0d;

                    if (parameter != null)
                    {
                        string parameterString = parameter as string;

                        switch (parameterString)
                        {
                            case AccelerometerThresholdParam:
                                resultValue = (double) Array.IndexOf(accelerometerThreshold, convertValue);
                                break;
                            case GyrometerThresholdParam:
                                resultValue = (double)Array.IndexOf(gyrometerThreshold, convertValue);
                                break;
                        }
                    }
                    return resultValue;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                if (value.GetType().Equals(typeof(double)))
                {
                    int convertValue = System.Convert.ToInt32(value);
                    object resultValue = 0;

                    if (parameter != null)
                    {
                        string parameterString = parameter as string;

                        switch (parameterString)
                        {
                            case reportIntervalParam:
                                if (convertValue >= 0 && convertValue < reportInterval.Length)
                                {
                                    resultValue = reportInterval.ElementAt(convertValue);
                                }
                                break;
                            case gpsReportIntervalParam:
                                if (convertValue >= 0 && convertValue < gpsReportInterval.Length)
                                {
                                    resultValue = gpsReportInterval.ElementAt(convertValue);
                                }
                                break;
                            case processedSampleCountParam:
                                if (convertValue >= 0 && convertValue < processedSampleCount.Length)
                                {
                                    resultValue = processedSampleCount.ElementAt(convertValue);
                                }
                                break;
                            case AccelerometerThresholdParam:
                                if (convertValue >= 0 && convertValue < accelerometerThreshold.Length)
                                {
                                    resultValue = accelerometerThreshold.ElementAt(convertValue);
                                }
                                break;
                            case GyrometerThresholdParam:
                                if (convertValue >= 0 && convertValue < gyrometerThreshold.Length)
                                {
                                    resultValue = gyrometerThreshold.ElementAt(convertValue);
                                }
                                break;
                            case StepDistanceParam:
                                if (convertValue >= 0 && convertValue < stepDistance.Length)
                                {
                                    resultValue = stepDistance.ElementAt(convertValue);
                                }
                                break;
                            case PeakJoinDistanceParam:
                                if (convertValue >= 0 && convertValue < peakJoinDistance.Length)
                                {
                                    resultValue = peakJoinDistance.ElementAt(convertValue);
                                }
                                break;
                        }
                    }
                    return resultValue;
                }
            }
            return value;
        }
    }
}
