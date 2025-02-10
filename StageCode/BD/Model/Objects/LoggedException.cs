using CodeExceptionManager.DAO;
using CodeExceptionManager.Model.Entities.ErrorLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeExceptionManager.Model.Objects
{
    public class LoggedException: Exception
    {
        public LoggedException(string sourceAssemblyName, string sourceAssemblyVersion, string sourceClassName, string sourceMethodName, string errorMessage = "", string errorStackTrace= "") : base() {
            BaseDAO<LoggedExceptionEntity>.Create(new LoggedExceptionEntity() { 
                 AssemblyName = sourceAssemblyName,
                 AssemblyVersion = sourceAssemblyVersion,
                 ClassName = sourceClassName,
                 MethodName = sourceMethodName,
                 ErrorMessage = errorMessage,
                 ErrorStackTrace = errorStackTrace,
                 Date = DateTime.Now
            }, "Exceptions");
        }
    }
}
