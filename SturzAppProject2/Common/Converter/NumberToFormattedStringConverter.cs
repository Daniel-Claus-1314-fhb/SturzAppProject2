using System;
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
                            case "PeakJoinDistanceFull":
                                return String.Format("Peaks innerhalb {0:G} ms als Schritte erkennen", convertUnsignedInteger);
                            case "PeakJoinDistanceSimple":
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
                            case "AccelerometerThresholdFull":
                                return String.Format("Accelerometerschwellwert {0:f2} G", convertDouble);
                            case "AccelerometerThresholdSimple":
                                return String.Format("{0:f2} G", convertDouble);
                            case "GyrometerThresholdFull":
                                return String.Format("Gyrometerschwellwert {0:G} rad/s", convertDouble);
                            case "GyrometerThresholdSimple":
                                return String.Format("{0:G} rad/s", convertDouble);
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
