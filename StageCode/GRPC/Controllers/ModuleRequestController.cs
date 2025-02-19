using CodeExceptionManager.Model.Objects;
using Orthodyne.CoreCommunicationLayer.Models.Common;
using Orthodyne.CoreCommunicationLayer.Models.Request;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleRequestController
    {
       
        private Dictionary<long, RequestSourceItem> definedRequestsInCache = new Dictionary<long, RequestSourceItem>();
        private GeneralController generalController;
        private ModuleRequestRemoteMethodInvocationService remoteMethods;
        internal ModuleRequestController(ModuleRequestRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {
            try
            {
                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
                //LoadRequests(true);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public bool HasUnsavedChanged { get; set; }
      
        public Dictionary<long, RequestSourceItem> DefinedRequestsInCache { get { return definedRequestsInCache; } }
    }
}