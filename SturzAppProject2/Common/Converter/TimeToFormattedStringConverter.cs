using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BackgroundTask.Common.Converter
{
    class TimeToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                //#############################################################################
                //################################### DateTime ################################
                //#############################################################################
                if (value.GetType().Equals(typeof(DateTime)))
                {
                    DateTime convertDateTime = (DateTime)value;

                    if (convertDateTime == DateTime.MinValue)
                    {
                        return String.Format("-");
                    }

                    if (parameter != null)
                    {
                        string parameterString = parameter as string;

                        switch (parameterString)
                        {
                            case "CreateFull":
                                return String.Format("Erstelltdatum: {0:G}", convertDateTime);
                            case "CreateSimple":
                                return String.Format("{0:G}", convertDateTime);

                            case "StartFull":
                                return String.Format("Startdatum: {0:G}", convertDateTime);
                            case "StartSimple":
                                return String.Format("{0:G}", convertDateTime);

                            case "StopFull":
                                return String.Format("Stopdatum: {0:G}", convertDateTime);
                            case "StopSimple":
                                return String.Format("{0:G}", convertDateTime);
                            default:
                                return String.Format("{0:G}", convertDateTime);
                        }
                    }
                    else
                    {
                        return String.Format("{0:G}", convertDateTime);
                    }
                }

                //#############################################################################
                //################################### TimeSpan ################################
                //#############################################################################
                else if (value.GetType().Equals(typeof(TimeSpan)))
                {
                    TimeSpan convertTimeSpan = (TimeSpan)value;

                    if (convertTimeSpan == TimeSpan.Zero)
                    {
                        return String.Format("-");
                    }

                    if (parameter != null)
                    {
                        string parameterString = parameter as string;

                        switch (parameterString)
                        {
                            case "DurationFull":
                                return String.Format("Dauer: {0:hh\\:mm\\:ss}", convertTimeSpan);
                            case "DurationSimple":
                                return String.Format("{0:hh\\:mm\\:ss}", convertTimeSpan);
                            default:
                                return String.Format("{0:hh\\:mm\\:ss}", convertTimeSpan);
                        }
                    }
                    else
                    {
                        return String.Format("{0:hh\\:mm\\:ss}", convertTimeSpan);
                    }

                }
            }
            return "Falscher Datentype für Zeitconvertierung.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
