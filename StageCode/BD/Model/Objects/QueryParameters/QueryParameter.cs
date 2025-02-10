using CodeExceptionManager.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.Model.Objects.QueryParameters
{
    public abstract class QueryParameter
    {
        public QueryParametersEnum ParameterType { get; set; }
    }
}
