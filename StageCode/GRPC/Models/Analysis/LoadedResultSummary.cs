using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    public class LoadedResultSummary
    {
        public long Id { get; set; }
        public string AnalysisDate { get; set; }
        public string MethodName { get; set; }

        public string Status { get; set; }
    }
}
