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

        private readonly uint[] reportInterval = new uint[6] { 10, 16, 20, 50, 100, 200 };
        private readonly uint[] processedSampleCount = new uint[7] { 50, 100, 200, 250, 500, 1000, 2000 };

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
                    uint resultValue = 0;

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
                        }
                    }
                    return resultValue;
                }
            }
            return value;
        }
    }
}
