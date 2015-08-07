using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BackgroundTask.Common.Selector
{
    class MeasurementTypeSelector : DataTemplateSelector
    {
        public DataTemplate InitializedMeasurementTemplate { get; set; }
        public DataTemplate StartedMeasurementTemplate { get; set; }
        public DataTemplate StoppedMeasurementTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {

            if (item.GetType() == typeof(MeasurementViewModel))
            {
                MeasurementViewModel measurementViewModel = (MeasurementViewModel)item;

                switch (measurementViewModel.MeasurementState)
                {
                    case MeasurementState.Initialized:
                        return InitializedMeasurementTemplate;

                    case MeasurementState.Started:
                        return StartedMeasurementTemplate;

                    case MeasurementState.Stopped:
                        return StoppedMeasurementTemplate;
                }
            }
            return base.SelectTemplateCore(item);
        }
    }
}
