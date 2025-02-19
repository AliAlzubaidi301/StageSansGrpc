using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class AnalysisResultMetadataItem
    {
        private string specAlarmLevel;
        private double concentration;
        public long Id { get; set; }
        public ImpurityItem Impurity { get; set; }
        public double Concentration { get { return Math.Round(concentration, 3); } set { concentration = value; } }
        public PeakMetaDataItem Peak { get; set; }
        public ContinuousMetaDataItem ContinuousData { get; set; }
        public SpecificationItem ArchivedSpecification { get; set; }
    }
}