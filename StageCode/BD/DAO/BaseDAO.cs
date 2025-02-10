using CodeExceptionManager.Controller;
using CodeExceptionManager.Model.Objects;
using CodeExceptionManager.Model.Objects.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.DAO
{
    public static class BaseDAO<T>
    {
        public static void Create(T entity, string tableName = "") {
            QueryObject query = new QueryObject() { TableName = tableName, RequestType = Model.Enum.RequestTypeEnum.create };
            query.Parameters.Add(Model.Enum.QueryParametersEnum.insertValues, new List<Model.Objects.QueryParameters.QueryParameter>());
            foreach (var sourceProperty in typeof(T).GetProperties().Where(x => x.Name.ToLower() != "id")) {
                query.Parameters[Model.Enum.QueryParametersEnum.insertValues].Add(new InsertValuesQueryParameters() { 
                    ColumnName = sourceProperty.Name,
                    ColumnValue = sourceProperty.GetValue(entity)
                });
            }
            DatabaseController.ExecuteQuery(query);
        }
    }
}
