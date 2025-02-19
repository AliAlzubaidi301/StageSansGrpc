using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using Orthodyne.CoreCommunicationLayer.Models.Alarms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Services
{
    public class ModuleAlarmRemoteMethodInvocationService
    {
        private string applicationGuid;
        private const string PORT_NUMBER = "50057";
        private const string DEFAULT_CORE_IP = "127.0.0.1";
        private Channel grpcChannel;

        public ModuleAlarmRemoteMethodInvocationService(string applicationGuid)
        {

            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
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

        public ModuleAlarmRemoteMethodInvocationService(string applicationGuid, string ip)
        {

            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(ip + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
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
    }
}
