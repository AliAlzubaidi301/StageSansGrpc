using Orthodyne.CoreCommunicationLayer.Services;
using Orthodyne.CoreCommunicationLayer.Models.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeExceptionManager.Model.Objects;
using System.Reflection;
using Orthodyne.CoreCommunicationLayer.Models.AuditTrail;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleHistoryController
    {
        private ModuleHistoryRemoteMethodInvocationService remoteMethods;
        private GeneralController generalController;

        //private List<HistoryMessageElement> listHistoryMessageItem = new List<HistoryMessageElement>();

        internal ModuleHistoryController(ModuleHistoryRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {

            try
            {
                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public List<string> LoadDinctinctContent(string property)
        {
            List<string> returnValue = new List<string>();
            try
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return null;
            }
            return returnValue;
        }
    }
}
