using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundTask.DataModel
{
    class MeasurementEvaluation
    {
        public string Source { get; set; }
        public long SamplesCount { get; set;}
        public int StepsDetected { get; set; }
        public int FallsDetected { get; set; }
        
        //DOTO list of step and fall detections    
    }
}
