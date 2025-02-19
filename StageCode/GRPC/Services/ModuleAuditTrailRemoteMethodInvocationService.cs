using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Services
{
    public class ModuleAuditTrailRemoteMethodInvocationService
    {
        private const string PORT_NUMBER = "50059";
        private const string DEFAULT_CORE_IP = "127.0.0.1";
        private string applicationGuid;
        private Channel grpcChannel;
      
    }
}
