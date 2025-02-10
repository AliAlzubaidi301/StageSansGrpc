using CodeExceptionManager.Model.Enum;
using CodeExceptionManager.Model.Objects.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.Model.Objects
{
    public class QueryObject
    {
        private Dictionary<QueryParametersEnum, List<QueryParameter>> parameters = new Dictionary<QueryParametersEnum, List<QueryParameter>>();
        public RequestTypeEnum RequestType { get; set; }
        public String TableName { get; set; }
        public Dictionary<QueryParametersEnum, List<QueryParameter>> Parameters {  
            get { return parameters; }
        }

    }
}
