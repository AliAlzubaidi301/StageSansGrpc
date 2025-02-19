using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Analysis
{
    [Serializable]
    public class ContinuousMetaDataItem
    {
        public long NbPoint { get; set; }
        public double MeanSignalValue { get; set; }
        public double StdDeviationSignalValue { get; set; }
        public double MinSignalValue { get; set; }
        public double MaxSignalValue { get; set; }

    }
}
