using BackgroundTask.DataModel.DataSets;
using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.Service
{
    public static class SensorDataSetService
    {
        public static List<T> loadDataSetFromFile<T>(DataSetType dataSetType, int Offset, int Count)
        {

            if (typeof(T).GetElementType().Equals(typeof(AccelerometerSample)))
            {

                return new List<T>();
            }


            return null;
        }
    }
}
