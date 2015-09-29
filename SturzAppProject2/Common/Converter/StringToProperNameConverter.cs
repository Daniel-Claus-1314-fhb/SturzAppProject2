using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BackgroundTask.Common.Converter
{
    class StringToProperNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value.GetType().Equals(typeof(string)))
            {
                string nameValue = value as string;

                if (nameValue != null && nameValue != String.Empty)
                {
                    nameValue = nameValue.Trim();
                    nameValue = Regex.Replace(nameValue, @"\s+", "_");
                    nameValue = Regex.Replace(nameValue, @"[^\w\.@-]", ""); 
                }

                if (nameValue != null && nameValue == String.Empty)
                {
                    nameValue = "Neue_Messung";
                }
                return nameValue;
            }
            return value;
        }
    }
}
