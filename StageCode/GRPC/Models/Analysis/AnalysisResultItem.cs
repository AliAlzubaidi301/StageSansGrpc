using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{

    public class AnalysisResultItem
    {
        public long Id { get; set; }

        public string SequenceName { get; set; }

        private Dictionary<string, AnalysisSignalItem> bufferedSignals = new Dictionary<string, AnalysisSignalItem>();
        private Dictionary<long, AnalysisResultMetadataItem> impuritiesMetadatas = new Dictionary<long, AnalysisResultMetadataItem>();
        private List<TimeEventItem> timeEvents = new List<TimeEventItem>();
        private Dictionary<long, AnalysisResultVariableItem> resultVariables = new Dictionary<long, AnalysisResultVariableItem>();
        
        public Dictionary<string, AnalysisSignalItem> BufferedSignals { get { return bufferedSignals; } }
        public Dictionary<long, AnalysisResultMetadataItem> ImpuritiesMetadatas { get { return impuritiesMetadatas; } }
        public List<TimeEventItem> TimeEvents { get { return this.timeEvents; } }
        public Dictionary<long, AnalysisResultVariableItem> ResultVariables { get { return resultVariables; } }

        public DateTime AnalysisDate { get; set; }
        public long SequenceId { get; set; }
       
        public AnalysisResultItem CopyItem() 
        {
            AnalysisResultItem copy = new AnalysisResultItem()
            {
                Id = this.Id,
                AnalysisDate = this.AnalysisDate,
                SequenceId = this.SequenceId
            };
            foreach (KeyValuePair<string, AnalysisSignalItem> usedSignal in this.bufferedSignals) copy.bufferedSignals.Add(usedSignal.Key, usedSignal.Value.CopyItem());
            foreach (TimeEventItem timeEvent in this.timeEvents) copy.timeEvents.Add(timeEvent.CopyItem());
            foreach (KeyValuePair<long, AnalysisResultVariableItem> resultVariable in this.resultVariables) copy.resultVariables.Add(resultVariable.Key, resultVariable.Value.CopyItem());
            return copy;
        }
    }
}
