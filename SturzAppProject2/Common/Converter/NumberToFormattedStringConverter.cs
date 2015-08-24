﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BackgroundTask.Common.Converter
{
    class NumberToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                if (value.GetType().Equals(typeof(uint)))
                {
                    uint convertUnsignedInteger = (uint)value;

                    if (parameter != null)
                    {
                        string parameterString = parameter as string;

                        switch (parameterString)
                        {
                            case "ReportIntervalFull":
                                return String.Format("Abtastrate {0:G} ms", convertUnsignedInteger);
                            case "ReportIntervalSimple":
                                return String.Format("{0:G} ms", convertUnsignedInteger);
                            case "ProcessedSampleCountFull":
                                return String.Format("Auswertung für {0:G} Messwerte", convertUnsignedInteger);
                            case "ProcessedSampleCountSimple":
                                return String.Format("{0:G} Messwerte", convertUnsignedInteger);
                            case "StepDistanceFull":
                                return String.Format("Schrittabstand {0:G} ms", convertUnsignedInteger);
                            case "StepDistanceSimple":
                                return String.Format("{0:G} ms", convertUnsignedInteger);
                            default:
                                return String.Format("{0:G}", convertUnsignedInteger);
                        }
                    }
                    else
                    {
                        return String.Format("{0:G}", convertUnsignedInteger);
                    }
                }
                if (value.GetType().Equals(typeof(double)))
                {
                    double convertDouble = (double)value;

                    if (parameter != null)
                    {
                        string parameterString = parameter as string;

                        switch (parameterString)
                        {
                            case "PeakThresholdFull":
                                return String.Format("Amplitudenschwellwert {0:f2}", convertDouble);
                            case "PeakThresholdSimple":
                                return String.Format("{0:f2}", convertDouble);
                            default:
                                return String.Format("{0:G}", convertDouble);
                        }
                    }
                    else
                    {
                        return String.Format("{0:G}", convertDouble);
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
