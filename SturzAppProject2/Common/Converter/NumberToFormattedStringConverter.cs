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
                            case "ReportInterval":
                                return String.Format("Abtastrate: {0:G} ms", convertUnsignedInteger);
                            case "ProcessedSampleCount":
                                return String.Format("{0:G} Messwerte werden gemeinsam ausgewertet.", convertUnsignedInteger);
                            default:
                                return String.Format("{0:G}", convertUnsignedInteger);
                        }
                    }
                    else
                    {
                        return String.Format("{0:G}", convertUnsignedInteger);
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
