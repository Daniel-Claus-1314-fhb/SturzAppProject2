using BackgroundTask.Service;
using Newtonsoft.Json;
using SensorDataEvaluation.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel.DataSets
{
    public class GyrometerDataSet
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public GyrometerDataSet(bool isAvailable, int totalcount)
            : this()
        {
            this.IsAvailable = isAvailable;
            this.TotalCount = totalcount;
        }

        public GyrometerDataSet()
        {
            this._currentDataSetOffset = 0;
            this._currentDataSetCount = 0;
            this._dataSamples = new List<GyrometerSample>();
        }

        #endregion

        //###################################################################################
        //################################### Properties ####################################
        //###################################################################################

        #region Properties

        public bool IsAvailable { get; set; }
        public int TotalCount { get; set; }

        [JsonIgnore]
        private int _currentDataSetOffset;

        [JsonIgnore]
        private int _currentDataSetCount;

        [JsonIgnore]
        private List<GyrometerSample> _dataSamples;
        
        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        public static GyrometerDataSet NewDefaultDataSet()
        {
            GyrometerDataSet result = new GyrometerDataSet();
            result.IsAvailable = false;
            result.TotalCount = 0;
            return result;
        }

        public async Task AnalyseDataSetAsync(string filename) 
        {
            this.IsAvailable = await FileService.IsGyrometerSamplesAvailable(filename);
            Debug.WriteLine("Find Gyrometer file: {0}", IsAvailable);
            if (this.IsAvailable)
            {
                this.TotalCount = await FileService.GetGyrometerSamplesCount(filename);
                Debug.WriteLine("Gyrometer Sample Count: {0}", TotalCount);
            }
        }

        public async Task<List<GyrometerSample>> GetDataSamples(string filename, int dataSetOffset, int dataSetCount)
        {
            bool isUpdateSamples = false;
            List<GyrometerSample> resultList = new List<GyrometerSample>();

            if (IsAvailable)
            {
                if (_dataSamples != null && _dataSamples.Count > 0)
                {
                    if (this._currentDataSetOffset == dataSetOffset && 
                        this._currentDataSetCount == dataSetCount)
                    {
                        resultList = this._dataSamples;
                    }
                    else
                    {
                        isUpdateSamples = true;
                    }
                }
                else if (_dataSamples == null || _dataSamples.Count == 0)
                {
                    isUpdateSamples = true;
                }

                if (isUpdateSamples)
                {
                    _dataSamples = await FileService.LoadGyrometerSamplesFromFileAsync(filename, dataSetOffset, dataSetCount);
                    this._currentDataSetOffset = dataSetOffset;
                    this._currentDataSetCount = dataSetCount;
                    resultList = this._dataSamples;
                }
            }
            return resultList;
        }

        #endregion
    }
}
