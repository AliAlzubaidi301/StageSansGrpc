using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class IoItemFeature
    {
        private List<string> parametersTypeInString;
        private List<string> returnTypesInString;

        public string Parameters
        {
            get { return String.Join(", ", this.parametersTypeInString.ToArray()); }
            private set { }
        }

        public string ReturnTypes
        {
            get { return String.Join(", ", this.returnTypesInString.ToArray()); }
            private set { }
        }

        public IoItemFeature()
        {
            parametersTypeInString = new List<string>();
            returnTypesInString = new List<string>();
        }

        public string FeatureName { get; set; }
        public bool VisibleFromUI { get; set; }


        public List<string> GetParametersTypesInString()
        {
            return this.parametersTypeInString;
        }

        public List<string> GetReturnTypesInString()
        {
            return this.returnTypesInString;
        }
        public IoItemFeature CopyItem() {
            IoItemFeature copy = new IoItemFeature() { 
                FeatureName = this.FeatureName
            };
            copy.parametersTypeInString.AddRange(this.parametersTypeInString);
            copy.returnTypesInString.AddRange(this.returnTypesInString);
            return copy;
        }
    }
}
