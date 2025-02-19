using CodeExceptionManager.Model.Objects;
using Orthodyne.CoreCommunicationLayer.Models.AuditTrail;
using Orthodyne.CoreCommunicationLayer.Models.History;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleAuditTrailController
    {
        private ModuleAuditTrailRemoteMethodInvocationService remoteMethods;
        private GeneralController generalController;

        internal ModuleAuditTrailController(ModuleAuditTrailRemoteMethodInvocationService remoteMethods, GeneralController generalController)
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
    }
}
