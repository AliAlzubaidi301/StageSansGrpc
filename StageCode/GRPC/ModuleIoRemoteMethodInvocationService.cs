using Grpc.Core;
using IIOManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrthoDesigner.GRPC
{
    public class ModuleIoRemoteMethodInvocationService
    {
        private const string PORT_NUMBER = "50052";
        private const string DEFAULT_CORE_IP = "https://10.1.6.11";
        private string applicationGuid;
        private Channel grpcChannel;
        private Methods.MethodsClient clientInterface;

        public ModuleIoRemoteMethodInvocationService(string applicationGuid)
        {

            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
                clientInterface = new Methods.MethodsClient(grpcChannel);
            }
            catch (Exception ex)
            {

            }

        }

        public GetAutoModeOutput GetAutoMode()
        {
            GetAutoModeOutput output = new GetAutoModeOutput();

            try
            {
                GetAutoModeInput input = new GetAutoModeInput() { };
                output = clientInterface.GetAutoMode(input);
            }
            catch (Exception ex)
            {

            }
            return output;
        }


        public ModuleIoRemoteMethodInvocationService(string applicationGuid, string ip)
        {
            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(ip + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
                clientInterface = new Methods.MethodsClient(grpcChannel);
            }
            catch (Exception ex)
            {

            }

        }

       
        public GetDefinedControllerOutput GetDefinedIoController()
        {
            GetDefinedControllerOutput output = new GetDefinedControllerOutput();

            try
            {
                GetDefinedControllersInput input = new GetDefinedControllersInput()
                {
                    Version = "1"
                };
                output = clientInterface.GetDefinedControllers(input);

                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public GetElementPropertiesOutput GetElementProperties(int idComponent)
        {
            GetElementPropertiesOutput output = new GetElementPropertiesOutput();

            try
            {
                GetElementPropertiesInput input = new GetElementPropertiesInput()
                {
                    ElementIdentifier = idComponent
                };
                output = clientInterface.GetElementProperties(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public GetControllerFeaturesOutput GetControllerFeatures(int idComponent)
        {
            GetControllerFeaturesOutput output = new GetControllerFeaturesOutput();

            try
            {
                GetControllerFeaturesInput input = new GetControllerFeaturesInput()
                {
                    ElementIdentifier = idComponent
                };
                output = clientInterface.GetControllerFeatures(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public UndoChangesOutput UndoChanges()
        {
            UndoChangesOutput output = new UndoChangesOutput();
            try
            {
                UndoChangesInput input = new UndoChangesInput();
                output = clientInterface.UndoChanges(input);
            }
            catch (Exception ex)
            {

            }
            return output;
        }

        public GetDefinedStreamsOutput GetDefinedStreams()
        {
            GetDefinedStreamsOutput output = new GetDefinedStreamsOutput();

            try
            {
                GetDefinedStreamsInput input = new GetDefinedStreamsInput();
                output = clientInterface.GetDefinedStreams(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public GetStreamDataTableOutput GetStreamDataTable(int idStreamComponent)
        {
            GetStreamDataTableOutput output = new GetStreamDataTableOutput();

            try
            {
                GetStreamDataTableInput input = new GetStreamDataTableInput() { ItemId = idStreamComponent };
                output = clientInterface.GetStreamDataTable(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }
        public GetStreamGroupsOutput GetStreamGroups(long idStream)
        {
            GetStreamGroupsOutput output = new GetStreamGroupsOutput();

            try
            {
                GetStreamGroupsInput input = new GetStreamGroupsInput();
                input.ItemId = idStream;
                output = clientInterface.GetStreamGroups(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }


        
        public GetRequiredPropertiesToConnectStreamOutput GetRequiredPropertiesToConnectStream()
        {
            GetRequiredPropertiesToConnectStreamOutput output = new GetRequiredPropertiesToConnectStreamOutput();

            try
            {
                GetRequiredPropertiesToConnectStreamInput input = new GetRequiredPropertiesToConnectStreamInput();
                output = clientInterface.GetRequiredPropertiesToConnectStream(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public SaveOutput Save(string userName, string actionName)
        {
            SaveOutput output = new SaveOutput();

            try
            {
                SaveInput input = new SaveInput()
                {
                    ComputerName = Environment.MachineName,
                    UserName = userName,
                    ActionName = actionName,
                };
                output = clientInterface.Save(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public RemoveLinkOutput RemoveLink(long idController, long idStream, long idDatatableItem, string userName, long oldControllerId = 0)
        {
            RemoveLinkOutput output = new RemoveLinkOutput();

            try
            {
                RemoveLinkInput input = new RemoveLinkInput()
                {
                    IdController = idController,
                    IdTargetStream = idStream,
                    IdDatatableItem = idDatatableItem,
                    ComputerName = Environment.MachineName,
                    UserName = userName,
                    IdOldController = oldControllerId,
                };
                output = clientInterface.RemoveLink(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public RemoveIoComponentOutput RemoveIoComponent(long idComponent, string userName)
        {
            RemoveIoComponentOutput output = new RemoveIoComponentOutput();

            try
            {
                RemoveIoComponentInput input = new RemoveIoComponentInput()
                {
                    IdComponent = idComponent,
                    ComputerName = Environment.MachineName,
                    UserName = userName
                };
                output = clientInterface.RemoveIoComponent(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        //Append unit Lbo public IIOManager.AddControllerOutput AddController(string name, string shortTypeName)
        //Append for alarm public IIOManager.AddControllerOutput AddController(string name, string shortTypeName, string unit)

        public AddControllerOutput AddController(string name, string shortTypeName, string unit, string errormsg, int priority, int type, int mainItemId, string streamSimpleName, double minvalue, double maxvalue, double idController, string componentName, long scaleMin = 0, long scaleMax = 0)
        {
            AddControllerOutput output = new AddControllerOutput();
            try
            {
                AddControllerInput input = new AddControllerInput()
                {
                    ItemName = name,
                    ShortTypeName = shortTypeName,
                    Unit = unit,
                    Errormsg = errormsg,
                    Priority = priority,
                    Type = type,
                    MainItemId = mainItemId,
                    StreamSimpleName = streamSimpleName,
                    Minvalue = (long)minvalue,
                    Maxvalue = (long)maxvalue,
                    IdController = (long)idController,
                    ComponentName = componentName,
                    ScaleMin = scaleMin,
                    ScaleMax = scaleMax
                };
                output = clientInterface.AddController(input);

                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public SearchAvailableStreamsOutput SearchAvailableStreams(bool StmLine, bool Beck)
        {
            SearchAvailableStreamsOutput output = new SearchAvailableStreamsOutput();

            try
            {
                SearchAvailableStreamsInput input = new SearchAvailableStreamsInput();
                input.Stmline = StmLine;
                input.Beck = Beck;
                output = clientInterface.SearchAvailableStreams(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }
        // For Icp Tcp/Ip
        public SearchAvailableStreamsOutput SearchAvailableStreamsIcp()
        {
            SearchAvailableStreamsOutput output = new SearchAvailableStreamsOutput();

            try
            {
                SearchAvailableStreamsInput input = new SearchAvailableStreamsInput();
                output = clientInterface.SearchAvailableStreamsIcp(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public SearchAvailableStreamsOutput SearchAvailableIndCommunication()
        {
            SearchAvailableStreamsOutput output = new SearchAvailableStreamsOutput();

            try
            {
                SearchAvailableStreamsInput input = new SearchAvailableStreamsInput();
                output = clientInterface.SearchAvailableStreamsIndCommunication(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public LoadEquipmentOutput LoadEquipment()
        {
            LoadEquipmentOutput output = new LoadEquipmentOutput();
            try
            {
                LoadEquipmentInput input = new LoadEquipmentInput();
                output = clientInterface.LoadEquipment(input);
            }
            catch (Exception ex)
            {

            }
            return output;
        }

        public CheckDllExitsOutput CheckIfDllExists(string name)
        {
            CheckDllExitsOutput output = new CheckDllExitsOutput();
            try
            {
                CheckDllExitsInput input = new CheckDllExitsInput
                {
                    Name = name,
                };
                output = clientInterface.CheckDllExits(input);
            }
            catch (Exception ex)
            {

            }
            return output;
        }

        public GetAvailableDllStreamListOutput GetAvailableDllStreamList()
        {
            GetAvailableDllStreamListOutput output = new GetAvailableDllStreamListOutput();

            try
            {
                GetAvailableDllStreamListInput input = new GetAvailableDllStreamListInput();
                output = clientInterface.GetAvailableDllStreamList(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public RequestAlarmCancellationOutput RequestAlarmCancellation(int componentId)
        {
            RequestAlarmCancellationOutput output = new RequestAlarmCancellationOutput();

            try
            {
                RequestAlarmCancellationInput input = new RequestAlarmCancellationInput
                {
                    ControllerId = Convert.ToInt64(componentId)
                };
                output = clientInterface.RequestAlarmCancellation(input);
            }
            catch (Exception ex)
            {

            }

            return output;
        }

        public RequestAlarmCreationOutput RequestAlarmCreation(string alarmType, int componentId, string userName)
        {
            RequestAlarmCreationOutput output = new RequestAlarmCreationOutput();

            try
            {
                RequestAlarmCreationInput input = new RequestAlarmCreationInput()
                {
                    AlarmType = alarmType,
                    ComponentId = Convert.ToInt64(componentId),
                    ComputerName = Environment.MachineName,
                    UserName = userName
                };
                output = clientInterface.RequestAlarmCreation(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }
        public GetTechnologyListOutput GetTechnologyList()
        {
            GetTechnologyListOutput output = new GetTechnologyListOutput();

            try
            {
                GetTechnologyListInput input = new GetTechnologyListInput();

                output = clientInterface.GetTechnologyList(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }

        }

        public AlarmAckOutput AlarmAck()
        {
            AlarmAckOutput output = new AlarmAckOutput();
            try
            {
                AlarmAckInput input = new AlarmAckInput();

                output = clientInterface.AlarmAck(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }
        }
        public GetHighAlarmsOutput GetHighAlarms()
        {
            GetHighAlarmsOutput output = new GetHighAlarmsOutput();
            try
            {
                GetHighAlarmsInput input = new GetHighAlarmsInput();

                output = clientInterface.GetHighAlarms(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }
        }
        public GetStatusIoStreamListOutput GetStatusIoStreamList()
        {
            GetStatusIoStreamListOutput output = new GetStatusIoStreamListOutput();
            try
            {
                GetStatusIoStreamListInput input = new GetStatusIoStreamListInput();

                output = clientInterface.GetStatusIoStreamList(input);
                return output;
            }
            catch (Exception ex)
            {

                return output;
            }
        }

    }
}
