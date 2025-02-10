using CodeExceptionManager.Model.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.Controller.DatabaseEngine
{
    public interface IDatabaseEngine
    {
        void Connect();
        void Disconnect();
        Boolean ExecuteSqlQueryWithoutReturnedData(string queryString);
        List<Object> ExecuteSqlQueryWithReturnedData(string queryString);
        bool ConnexionStatus { get; set; }
        string ParseQuery(QueryObject query);
    }
}
