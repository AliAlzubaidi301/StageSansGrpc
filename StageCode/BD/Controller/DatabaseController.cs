using CodeExceptionManager.Controller.DatabaseEngine;
using CodeExceptionManager.Controller.DatabaseEngine.Implementation;
using CodeExceptionManager.Model.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.Controller
{
    public static class DatabaseController
    {       
        private static IDatabaseEngine database = new SQLite();

        public static List<Object> ExecuteQuery(QueryObject query)
        {
            List<object> returnValue = new List<object>() { };
            try
            {
                if (database != null) {
                    if (!database.ConnexionStatus) database.Connect();
                    if (database.ConnexionStatus)
                    {
                        if (query.RequestType == Model.Enum.RequestTypeEnum.read)
                            returnValue.AddRange(database.ExecuteSqlQueryWithReturnedData(database.ParseQuery(query)));
                        else returnValue.Add(database.ExecuteSqlQueryWithoutReturnedData(database.ParseQuery(query)));
                    }
                    else returnValue.Add(false);             
                }
            } catch (Exception ex) {
                new List<object>() { ex.Message };
            }
            return returnValue;
        }
    }
}
