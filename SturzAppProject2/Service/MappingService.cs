using BackgroundTask.DataModel;
using BackgroundTask.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.Service
{
    public class MappingService
    {
        public ObservableCollection<MeasurementViewModel> mapTo(List<Measurement> measurements)
        {
            ObservableCollection<MeasurementViewModel> resultMeasurementViewModels = new ObservableCollection<MeasurementViewModel>();

            if (measurements != null)
            {
                foreach (Measurement measurement in measurements)
                {
                    resultMeasurementViewModels.Add(new MeasurementViewModel(measurement));
                }
            }
            return resultMeasurementViewModels;
        }
    }
}
