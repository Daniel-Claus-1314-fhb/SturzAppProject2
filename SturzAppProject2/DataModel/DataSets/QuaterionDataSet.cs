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
    public class QuaterionDataSet
    {
        //###################################################################################
        //################################### Construtors ###################################
        //###################################################################################

        #region Construtors

        public QuaterionDataSet(bool isAvailable, int totalcount) 
            : this()
        {
            this.IsAvailable = isAvailable;
            this.TotalCount = totalcount;
        }

        public QuaterionDataSet()
        {
            this._currentDataSetOffset = 0;
            this._currentDataSetCount = 0;
            this._dataSamples = new List<QuaternionSample>();
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
        private List<QuaternionSample> _dataSamples;
        
        #endregion

        //###################################################################################
        //##################################### Methods #####################################
        //###################################################################################

        #region Methods

        public static QuaterionDataSet NewDefaultDataSet()
        {
            QuaterionDataSet result = new QuaterionDataSet();
            result.IsAvailable = false;
            result.TotalCount = 0;
            return result;
        }

        public async Task AnalyseDataSetAsync(string filename) 
        {
            this.IsAvailable = await FileService.IsQuaterionSamplesAvailable(filename);
            Debug.WriteLine("Find Quaterion file: {0}", IsAvailable);
            if (this.IsAvailable)
            {
                this.TotalCount = await FileService.GetQuaterionSamplesCount(filename);
                Debug.WriteLine("Quaterion Sample Count: {0}", TotalCount);
            }
        }

        public async Task<List<QuaternionSample>> GetDataSamples(string filename, int dataSetOffset, int dataSetCount)
        {
            bool isUpdateSamples = false;
            List<QuaternionSample> resultList = new List<QuaternionSample>();

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
                    _dataSamples = await FileService.LoadQuaternionSamplesFromFileAsync(filename, dataSetOffset, dataSetCount);
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
