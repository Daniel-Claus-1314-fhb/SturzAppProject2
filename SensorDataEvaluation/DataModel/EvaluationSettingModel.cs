using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataEvaluation.DataModel
{
    public class EvaluationSettingModel
    {
        //###################################################################################################################
        //################################################## Constructor ####################################################
        //###################################################################################################################

        public EvaluationSettingModel(uint processingListCount, double accelerometerThreshold, double gyrometerThreshold, uint stepDistance, uint peakJoinDistance)
        {
            this.ProcessingListCount = processingListCount;
            this.AccelermeterVectorLengthThreshold = accelerometerThreshold;
            this.GyrometerVectorLengthThreshold = gyrometerThreshold;
            this.StepDistance = new TimeSpan(TimeSpan.TicksPerMillisecond * stepDistance);
            this.PeakJoinDistance = new TimeSpan(TimeSpan.TicksPerMillisecond * peakJoinDistance);
        }

        //###################################################################################################################
        //################################################## Properties #####################################################
        //###################################################################################################################

        public uint ProcessingListCount { get; private set; }
        public double AccelermeterVectorLengthThreshold { get; private set; }
        public double GyrometerVectorLengthThreshold { get; private set; }
        public TimeSpan StepDistance { get; private set; }
        public TimeSpan PeakJoinDistance { get; private set; }
    }
}
