using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    public sealed class TaskArguments
    {
        public TaskArguments()
        {
            ReportInterval = 16;
        }

        public TaskArguments(string accelerometerDataId, string gyrometerDataId, uint reportInterval)
        {
            this.AccelerometerDataId = accelerometerDataId;
            this.GyrometerDataId = gyrometerDataId;
            this.ReportInterval = reportInterval;
            this.ProcessingListSize = 1000;
        }

        public TaskArguments(string accelerometerDataId, string gyrometerDataId, uint reportInterval, int processingListSize)
            : this(accelerometerDataId, gyrometerDataId, reportInterval)
        {
            this.ProcessingListSize = processingListSize;
        }

        public string AccelerometerDataId{ get; set; }
        public string GyrometerDataId { get; set; }
        public uint ReportInterval{ get; set; }
        public int ProcessingListSize { get; set; }
    }
}
