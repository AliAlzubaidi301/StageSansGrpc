using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using IIOManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Services
{
    public class ModuleGeneralConfigurationRevocationService
    {
        private const string PORT_NUMBER = "50063";
        private const string DEFAULT_CORE_IP = "127.0.0.1";
        private string applicationGuid;
        private Channel grpcChannel;
        private Methods.MethodsClient clientInterface;

        public ModuleGeneralConfigurationRevocationService(string applicationGuid)
        {
            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
                clientInterface = new Methods.MethodsClient(grpcChannel);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public ModuleGeneralConfigurationRevocationService(string applicationGuid, string ip)
        {
            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(ip + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
                clientInterface = new Methods.MethodsClient(grpcChannel);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //public LoadFlagsElementsOutput LoadFlags(LoadFlagsElementInput input)
        //{
        //    LoadFlagsElementsOutput output = new LoadFlagsElementsOutput();
        //    try
        //    {
        //        output = clientInterface.LoadFlagsElements(input);
        //    }
        //    catch (Exception ex)
        //    {
        //        new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }
        //    return output;
        //}

    }
}
