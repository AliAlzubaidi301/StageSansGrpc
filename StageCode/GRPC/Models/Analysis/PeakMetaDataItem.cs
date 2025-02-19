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
	public class PeakMetaDataItem
	{
		public double AreaValue { get; set; }
		public double Height { get; set; }
		public long StartPointIndex { get; set; }
		public long TopPointIndex { get; set; }
		public long InflexionPointIndex { get; set; }
		public long EndPointIndex { get; set; }
		public List<PeakPoint> PeakPoints { get; set; }

		public PeakMetaDataItem CopyItem() 
		{
			PeakMetaDataItem copy = new PeakMetaDataItem() { 
				AreaValue = this.AreaValue,
				EndPointIndex = this.EndPointIndex,
				Height = this.Height,
				InflexionPointIndex = this.InflexionPointIndex,
				StartPointIndex = this.StartPointIndex,
				TopPointIndex = this.TopPointIndex
			};
			return copy;
		}
    }
}
