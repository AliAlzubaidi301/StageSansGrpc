using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeExceptionManager.Model.Objects;
using System.Reflection;

namespace Orthodyne.CoreCommunicationLayer.Services
{
    public class ModuleTranslationRemoteMethodInvocationService
    {
        private const string PORT_NUMBER = "50051";
        private const string DEFAULT_CORE_IP = "127.0.0.1";
        private string applicationGuid = "";
        private Channel grpcChannel;
    }
}
