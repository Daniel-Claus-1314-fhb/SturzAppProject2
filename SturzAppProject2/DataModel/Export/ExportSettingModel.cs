using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel.Export
{
    public class ExportSettingModel
    {
        public ExportSettingModel() { }

        public ExportSettingModel(bool isAccelerometer, bool isGyrometer, bool isQuaternion, bool isGeolocation, bool isEvaluation) 
        {
            this.IsAccelerometer = isAccelerometer;
            this.IsGyrometer = isGyrometer;
            this.IsQuaternion = isQuaternion;
            this.IsGeolocation = isGeolocation;
            this.IsEvaluation = isEvaluation;
        }

        public bool IsAccelerometer { get; set; }
        public bool IsGyrometer { get; set; }
        public bool IsQuaternion { get; set; }
        public bool IsGeolocation { get; set; }
        public bool IsEvaluation { get; set; }
    }
}
