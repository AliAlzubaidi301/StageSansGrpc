using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class Technology
    {
            public long Id { get; set; }
            public string Name { get; set; }
            //only used when edit, won't be stored client side
    }
}
