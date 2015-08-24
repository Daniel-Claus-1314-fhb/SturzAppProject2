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
        private const string processedSampleCountParam = "ProcessedSampleCount";
        private const string peakThresholdParam = "PeakThreshold";
        private const string stepDistanceParam = "StepDistance";

        private readonly uint[] reportInterval = new uint[6] { 10, 16, 20, 50, 100, 200 };
        private readonly uint[] processedSampleCount = new uint[7] { 50, 100, 200, 250, 500, 1000, 2000 };
        private readonly double[] peakThreshold = new double[13] { 0.2d, 0.25d, 0.3d, 0.35d, 0.4d, 0.45d, 0.5d, 0.55d, 0.6d, 0.65d, 0.7d, 0.75d, 0.8d };
        private readonly uint[] stepDistance = new uint[13] { 200, 250, 300, 350, 400, 450, 500, 550, 600, 650, 700, 750, 800 };

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
                            case processedSampleCountParam:
                                resultValue = (double) Array.IndexOf(processedSampleCount, convertValue);
                                break;
                            case stepDistanceParam:
                                resultValue = (double) Array.IndexOf(stepDistance, convertValue);
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
                            case peakThresholdParam:
                                resultValue = (double) Array.IndexOf(peakThreshold, convertValue);
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
                            case processedSampleCountParam:
                                if (convertValue >= 0 && convertValue < processedSampleCount.Length)
                                {
                                    resultValue = processedSampleCount.ElementAt(convertValue);
                                }
                                break;
                            case peakThresholdParam:
                                if (convertValue >= 0 && convertValue < peakThreshold.Length)
                                {
                                    resultValue = peakThreshold.ElementAt(convertValue);
                                }
                                break;
                            case stepDistanceParam:
                                if (convertValue >= 0 && convertValue < stepDistance.Length)
                                {
                                    resultValue = stepDistance.ElementAt(convertValue);
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
