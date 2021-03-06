﻿using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BackgroundTask.Common.Converter
{
    class NotifiedLevelToColorBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string colorCode = "{x:Null}";

            if (value != null)
            {
                NotifyLevel level = (NotifyLevel) value;
                
                switch (level)
                {
                    case NotifyLevel.Info:
                        // Green
                        colorCode = "#FF096E37";
                        break;
                    case NotifyLevel.Warn:
                        // Yellow
                        colorCode = "#FFDE9E13";
                        break;
                    case NotifyLevel.Error:
                        // Red
                        colorCode = "#FF9C2525";
                        break;
                }
            }

            return colorCode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
