using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using Orthodyne.CoreCommunicationLayer.Services;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class GeneralController
    {
        private ModuleAlarmRemoteMethodInvocationService alarmRemoteMethodInvocationService = null;
        private ModuleAccessRemoteMethodInvocationService accessRemoteMethodInvocationService = null;
        private ModuleAnalysisRemoteMethodInvocationService analysisRemoteMethodInvocationService = null;
        private ModuleIoRemoteMethodInvocationService ioRemoteMethodInvocationService = null;
        private ModuleJobRemoteMethodInvocationService jobRemoteMethodInvocationService = null;
        private ModuleTranslationRemoteMethodInvocationService translationRemoteMethodInvocationService = null;
        private ModuleHistoryRemoteMethodInvocationService historyRemoteMethodInvocationService = null;
        private ModuleRequestRemoteMethodInvocationService requestRemoteMethodInvocationService = null;
        private ModuleAuditTrailRemoteMethodInvocationService auditTrailRemoteMethodInvocationService = null;
        private ModuleSchedulerRemoteMethodInvocationService schedulerRemoteMethodInvocationService = null;
        private ModuleGeneralConfigurationRevocationService generalConfigurationRemoteInvocationService = null;

        private ModuleIoControllerOrthoDesigner moduleIoController;
        private ModuleJobController moduleJobController;
        private ModuleAccessController moduleAccessController;
        private ModuleAnalysisController moduleAnalysisController;
        private ModuleAlarmController moduleAlarmController;
        private ModuleTranslationController moduleTranslationController;
        private ModuleHistoryController moduleHistoryController;
        private ModuleRequestController moduleRequestController;
        private ModuleAuditTrailController moduleAuditTrailController;
        private ModuleSchedulerController moduleSchedulerController;
        private ModuleGeneralConfigurationControllerOrthoDesigner moduleGeneralConfigurationController;

        public bool IsCoreAvalaible { get; set; }

        public ModuleGeneralConfigurationControllerOrthoDesigner ModuleGeneralConfigurationController
        {
            get
            {
                return moduleGeneralConfigurationController;
            }
        }
        public ModuleSchedulerController ModuleSchedulerController
        {
            get
            {
                return moduleSchedulerController;
            }
        }

        public ModuleJobController ModuleJobController
        {
            get
            {
                return this.moduleJobController;
            }
        }
        public ModuleIoControllerOrthoDesigner ModuleIoController
        {
            get
            {
                return this.moduleIoController;
            }
        }
        public ModuleAnalysisController ModuleAnalysisController
        {
            get { return this.moduleAnalysisController; }
        }
        public ModuleAlarmController ModuleAlarmController
        {
            get
            {
                return this.moduleAlarmController;
            }
        }
        public ModuleAccessController ModuleAccessController
        {
            get
            {
                return this.moduleAccessController;
            }
        }
        public ModuleTranslationController ModuleTranslationController
        {
            get
            {
                return this.moduleTranslationController;
            }
        }
        public ModuleHistoryController ModuleHistoryController
        {
            get
            {
                return this.moduleHistoryController;
            }
        }
        public ModuleRequestController ModuleRequestController
        {
            get
            {
                return this.moduleRequestController;
            }
        }
        public ModuleAuditTrailController ModuleAuditTrailController
        {
            get
            {
                return this.moduleAuditTrailController;
            }
        }
        private string userLangage = "English";
        public string UserLangage
        {
            get
            {
                return userLangage;
            }
            set
            {
                userLangage = value;
            }
        }

        public void Dispose()
        {
            moduleAlarmController = null;
            moduleAnalysisController = null;
            moduleIoController = null;
            moduleJobController = null;
            moduleAccessController = null;
            moduleTranslationController = null;
            moduleHistoryController = null;
            moduleRequestController = null;
            moduleAuditTrailController = null;
            accessRemoteMethodInvocationService = null;
            alarmRemoteMethodInvocationService = null;
            analysisRemoteMethodInvocationService = null;
            jobRemoteMethodInvocationService = null;
            ioRemoteMethodInvocationService = null;
            translationRemoteMethodInvocationService = null;
            historyRemoteMethodInvocationService = null;
            requestRemoteMethodInvocationService = null;
            auditTrailRemoteMethodInvocationService = null;
            schedulerRemoteMethodInvocationService = null;
            moduleGeneralConfigurationController = null;
        }

        private void Init()
        {
            try
            {
                moduleGeneralConfigurationController = new ModuleGeneralConfigurationControllerOrthoDesigner(generalConfigurationRemoteInvocationService, this);
                moduleAlarmController = new ModuleAlarmController(alarmRemoteMethodInvocationService, this);
                moduleAnalysisController = new ModuleAnalysisController(analysisRemoteMethodInvocationService, this);
                moduleIoController = new ModuleIoControllerOrthoDesigner(ioRemoteMethodInvocationService, this);
                moduleJobController = new ModuleJobController(jobRemoteMethodInvocationService, this);
                moduleAccessController = new ModuleAccessController(accessRemoteMethodInvocationService, this);
                moduleTranslationController = new ModuleTranslationController(translationRemoteMethodInvocationService, this);
                moduleHistoryController = new ModuleHistoryController(historyRemoteMethodInvocationService, this);
                moduleRequestController = new ModuleRequestController(requestRemoteMethodInvocationService, this);
                moduleAuditTrailController = new ModuleAuditTrailController(auditTrailRemoteMethodInvocationService, this);
                moduleSchedulerController = new ModuleSchedulerController(schedulerRemoteMethodInvocationService, this);
                //IsCoreAvalaible = true;
            }
            catch (RpcException rex)
            {
                //IsCoreAvalaible = false;
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}
