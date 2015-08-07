using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    class MeasurementSetting
    {
        public uint ReportInterval { get; set; }
        public uint ProcessSamplesCount { get; set; }
        public bool UseAccelerometer { get; set; }
        public bool UseGyrometer { get; set; }
    }
}
