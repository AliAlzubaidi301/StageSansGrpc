using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.Model.Objects.QueryParameters
{
    public class InsertValuesQueryParameters: QueryParameter
    {
        public string ColumnName;
        public Object ColumnValue;
        public InsertValuesQueryParameters() : base() {
            this.ParameterType = Enum.QueryParametersEnum.insertValues;
        }
    }
}
