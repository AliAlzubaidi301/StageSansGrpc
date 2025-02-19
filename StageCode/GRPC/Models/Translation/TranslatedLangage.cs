using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Translation
{
    public class TranslatedLangage
    {
        public long Id { get; set; }
        public string Label { get; set; }
        public long QuantityOfTranlatedText { get; set; }
        public bool IsUsed { get; set; }
    }
}
