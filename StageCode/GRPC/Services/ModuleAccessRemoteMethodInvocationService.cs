using Grpc.Core;
using System.Collections.Generic;
using System.Linq;
using Orthodyne.CoreCommunicationLayer.Models.Access;
using CodeExceptionManager.Model.Objects;
using System.Reflection;
using System;

namespace Orthodyne.CoreCommunicationLayer.Services
{
    public class ModuleAccessRemoteMethodInvocationService
    {
        private const string PORT_NUMBER = "50055";
        private const string DEFAULT_CORE_IP = "127.0.0.1";
        private string applicationGuid = "";
        private Channel grpcChannel;
      
    }
}
