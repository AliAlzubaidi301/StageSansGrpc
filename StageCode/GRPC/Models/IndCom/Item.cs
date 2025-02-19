using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IndCom
{
    public class Item
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public VarTypeComEnum VarTypeCom { get; set; }
        //public string Adress { get; set; }
        public double Adress { get; set; }
        public IndCommTypeComponentEnum VarTypeOrtho { get; set; }
        public long IdEqu { get; set; }
    }
}
