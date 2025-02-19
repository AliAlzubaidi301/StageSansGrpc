using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using Orthodyne.CoreCommunicationLayer.Models.Common;
using Orthodyne.CoreCommunicationLayer.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Services
{
    public class ModuleRequestRemoteMethodInvocationService
    {
        private const string PORT_NUMBER = "50058";
        private const string DEFAULT_CORE_IP = "127.0.0.1";
        private string applicationGuid;
        private Channel grpcChannel;

        public ModuleRequestRemoteMethodInvocationService(string applicationGuid)
        {
            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}