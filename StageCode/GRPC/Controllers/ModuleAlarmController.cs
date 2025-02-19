using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using Orthodyne.CoreCommunicationLayer.Models.Alarms;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleAlarmController
    {
        private Dictionary<long, AlarmItem> alarms = new Dictionary<long, AlarmItem>();
        private Dictionary<long, AlarmItem> usedAlarms = new Dictionary<long, AlarmItem>();
        private Dictionary<string, AlarmFormatDefinitionItem> existingAlarmsPattern = new Dictionary<string, AlarmFormatDefinitionItem>(); 
        private ModuleAlarmRemoteMethodInvocationService remoteMethods;
        private GeneralController generalController;

        internal ModuleAlarmController(ModuleAlarmRemoteMethodInvocationService remoteMethods, GeneralController generalController) {

            try
            {
                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public void AckAlarms(List<long> alarmIds = null) {

            try
            {
                if (alarmIds == null)
                {
                    alarmIds.AddRange(this.usedAlarms.Keys);
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public void EditAlarm(AlarmItem toSaveItem) {

            try
            {
               
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public Dictionary<long, AlarmItem> Alarms { get { return alarms; } }
        public Dictionary<long, AlarmItem> UsedAlarms { get { return usedAlarms; } }
        public Dictionary<string, AlarmFormatDefinitionItem> ExistingAlarmsPattern { get { return existingAlarmsPattern; } }
    }
}
