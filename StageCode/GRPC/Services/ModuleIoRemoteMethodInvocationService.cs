using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using IIOManager;
using Orthodyne.CoreCommunicationLayer.Models.IndCom;
using Orthodyne.CoreCommunicationLayer.Models.IO;
using Orthodyne.CoreCommunicationLayer.Models.Technology;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Orthodyne.CoreCommunicationLayer.Services
{
    public class ModuleIoRemoteMethodInvocationService
    {
        private const string PORT_NUMBER = "50052";
        private const string DEFAULT_CORE_IP = "10.1.6.11";
        private string applicationGuid;
        private Channel grpcChannel;
        private IIOManager.Methods.MethodsClient clientInterface;

        public ModuleIoRemoteMethodInvocationService(string applicationGuid)
        {

            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(DEFAULT_CORE_IP + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
                clientInterface = new IIOManager.Methods.MethodsClient(grpcChannel);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return output;
        }


        public ModuleIoRemoteMethodInvocationService(string applicationGuid, string ip)
        {

            try
            {
                this.applicationGuid = applicationGuid;
                grpcChannel = new Channel(ip + ":" + PORT_NUMBER, ChannelCredentials.Insecure);
                clientInterface = new IIOManager.Methods.MethodsClient(grpcChannel);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

        }

        public IIOManager.DeleteTechnologyOutput DeleteTechnology(TechnologyItem item, string userName)
        {
            IIOManager.DeleteTechnologyOutput output = new DeleteTechnologyOutput();
            DeleteTechnologyInput input = new DeleteTechnologyInput()
            {
                ComputerName = Environment.MachineName,
                UserName = userName,
                ToDelete = new TechnologyElement
                {
                    Id = item.Id,
                    Name = item.Name,
                }
            };

            output = clientInterface.DeleteTechnology(input);

            return output;
        }

        public DeleteItemOutput DeleteItem(Item toDelete, string userName)
        {
            DeleteItemOutput output = new DeleteItemOutput();
            DeleteItemInput input = new DeleteItemInput
            {
                ComputerName = Environment.MachineName,
                UserName = userName,
                ToDelete = new ItemElement
                {
                    Adress = toDelete.Adress.ToString(),
                    VarTypeOrtho = (long)toDelete.VarTypeOrtho,
                    Id = toDelete.Id,
                    IdEqu = toDelete.IdEqu,
                    Name = toDelete.Name,
                    VarTypeCom = (long)toDelete.VarTypeCom,
                },
            };
            output = clientInterface.DeleteItem(input);
            return output;
        }

        public EditItemOutput EditItem(Item toSave, string userName)
        {
            EditItemOutput output = new EditItemOutput();
            EditItemInput input = new EditItemInput
            {
                ComputerName = Environment.MachineName,
                UserName = userName,
                ToSave = new ItemElement
                {
                    Adress = toSave.Adress.ToString(),
                    VarTypeOrtho = (long)toSave.VarTypeOrtho,
                    Id = toSave.Id,
                    IdEqu = toSave.IdEqu,
                    Name = toSave.Name,
                    VarTypeCom = (long)toSave.VarTypeCom,
                },
            };
            output = clientInterface.EditItem(input);
            return output;
        }

        public DeleteParametersOutput DeleteParameter(ParamItem toDelete, string userName)
        {
            DeleteParametersOutput output = new DeleteParametersOutput();
            DeleteParametersInput input = new DeleteParametersInput
            {
                ComputerName = Environment.MachineName,
                ToDelete = new ParamElement
                {
                    Id = toDelete.Id,
                    IdEqu = toDelete.IdEqu,
                    Name = toDelete.Name,
                    Value = toDelete.Value,
                },
                UserName = userName,
            };
            output = clientInterface.DeleteParameters(input);
            return output;
        }

        public EditParametersOutput EditParameter(List<ParamItem> toSave, string userName)
        {
            EditParametersOutput output = new EditParametersOutput();
            EditParametersInput input = new EditParametersInput()
            {
                ComputerName = Environment.MachineName,
                UserName = userName,
            };
            foreach (var item in toSave)
            {
                input.ToSave.Add(new ParamElement
                {
                    Id = item.Id,
                    IdEqu = item.IdEqu,
                    Name = item.Name,
                    Value = item.Value,
                });
            }
            output = clientInterface.EditParameters(input);
            return output;
        }

        public DeleteEquipmentOutput DeleteEquipment(EquipmentItem toDelete, string userName)
        {
            DeleteEquipmentOutput output = new DeleteEquipmentOutput();
            DeleteEquipmentInput input = new DeleteEquipmentInput
            {
                ComputerName = Environment.MachineName,
                ToDelete = new EquipmentElement
                {
                    Id = toDelete.Id,
                    Name = toDelete.Name,
                    Type = (long)toDelete.Type,
                },
                UserName = userName,
            };
            output = clientInterface.DeleteEquipment(input);
            return output;
        }

        public EditEquipmentOutput EditEquipment(EquipmentItem toSave, string userName)
        {
            EditEquipmentOutput output = new EditEquipmentOutput();
            EditEquipmentInput input = new EditEquipmentInput
            {
                ComputerName = Environment.MachineName,
                ToSave = new EquipmentElement
                {
                    Id = toSave.Id,
                    Name = toSave.Name,
                    Type = (long)toSave.Type,
                },
                UserName = userName,
            };

            if (toSave.Items != null)
            {
                foreach (var item in toSave.Items)
                {
                    input.ToSave.Items.Add(new ItemElement
                    {
                        Adress = item.Adress.ToString(),
                        Id = item.Id,
                        Name = item.Name,
                        VarTypeCom = (long)item.VarTypeCom,
                        VarTypeOrtho = (long)item.VarTypeOrtho,
                    });
                }
            }

            if (toSave.Params != null)
            {
                foreach (var item in toSave.Params)
                {
                    input.ToSave.Params.Add(new ParamElement
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Value = item.Value,
                    });
                }
            }
            output = clientInterface.EditEquipment(input);
            return output;
        }

        public IIOManager.EditTechnologyOutput EditTechnology(List<TechnologyItem> items, string userName)
        {
            IIOManager.EditTechnologyOutput output = new EditTechnologyOutput();
            EditTechnologyInput input = new EditTechnologyInput()
            {
                ComputerName = Environment.MachineName,
                UserName = userName,
            };

            foreach (var item in items)
            {
                TechnologyElement tmp = new TechnologyElement
                {
                    Id = item.Id,
                    Name = item.Name,
                };
                input.Technologies.Add(tmp);
            }

            output = clientInterface.EditTechnology(input);

            return output;
        }

        public IIOManager.GetDefinedControllerOutput GetDefinedIoController()
        {
            IIOManager.GetDefinedControllerOutput output = new IIOManager.GetDefinedControllerOutput();

            try
            {
                IIOManager.GetDefinedControllersInput input = new IIOManager.GetDefinedControllersInput()
                {
                    Version = "1"
                };
                output = clientInterface.GetDefinedControllers(input);

                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.GetElementPropertiesOutput GetElementProperties(int idComponent)
        {
            IIOManager.GetElementPropertiesOutput output = new IIOManager.GetElementPropertiesOutput();

            try
            {
                IIOManager.GetElementPropertiesInput input = new IIOManager.GetElementPropertiesInput()
                {
                    ElementIdentifier = idComponent
                };
                output = clientInterface.GetElementProperties(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.GetControllerFeaturesOutput GetControllerFeatures(int idComponent)
        {
            IIOManager.GetControllerFeaturesOutput output = new IIOManager.GetControllerFeaturesOutput();

            try
            {
                IIOManager.GetControllerFeaturesInput input = new IIOManager.GetControllerFeaturesInput()
                {
                    ElementIdentifier = idComponent
                };
                output = clientInterface.GetControllerFeatures(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return output;
        }

        public IIOManager.GetDefinedStreamsOutput GetDefinedStreams()
        {
            IIOManager.GetDefinedStreamsOutput output = new IIOManager.GetDefinedStreamsOutput();

            try
            {
                IIOManager.GetDefinedStreamsInput input = new IIOManager.GetDefinedStreamsInput();
                output = clientInterface.GetDefinedStreams(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.GetStreamDataTableOutput GetStreamDataTable(int idStreamComponent)
        {
            IIOManager.GetStreamDataTableOutput output = new IIOManager.GetStreamDataTableOutput();

            try
            {
                IIOManager.GetStreamDataTableInput input = new IIOManager.GetStreamDataTableInput() { ItemId = idStreamComponent };
                output = clientInterface.GetStreamDataTable(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }
        public IIOManager.GetStreamGroupsOutput GetStreamGroups(long idStream)
        {
            IIOManager.GetStreamGroupsOutput output = new IIOManager.GetStreamGroupsOutput();

            try
            {
                IIOManager.GetStreamGroupsInput input = new IIOManager.GetStreamGroupsInput();
                input.ItemId = idStream;
                output = clientInterface.GetStreamGroups(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }


        public IIOManager.ExecuteControllerFeatureOutput ExecuteControllerFeature(int idComponent, IoItemFeature feature, object[] parameters = null)
        {
            IIOManager.ExecuteControllerFeatureOutput output = new IIOManager.ExecuteControllerFeatureOutput();
            if (feature == null) { return output; }
            try
            {
                IIOManager.ExecuteControllerFeatureInput input = new IIOManager.ExecuteControllerFeatureInput()
                {
                    ElementIdentifier = idComponent
                };
                IIOManager.ComponentFeatureMetadata featureMetadata = new IIOManager.ComponentFeatureMetadata();
                featureMetadata.FeatureName = feature.FeatureName;
                featureMetadata.ParametersTypeInString.AddRange(feature.GetParametersTypesInString());
                featureMetadata.ReturnTypesInString.AddRange(feature.GetReturnTypesInString());
                input.Feature = featureMetadata;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        input.ParametersInString.Add(parameters[i].ToString());
                    }
                }
                output = clientInterface.ExecuteControllerFeature(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.GetRequiredPropertiesToConnectStreamOutput GetRequiredPropertiesToConnectStream()
        {
            IIOManager.GetRequiredPropertiesToConnectStreamOutput output = new IIOManager.GetRequiredPropertiesToConnectStreamOutput();

            try
            {
                IIOManager.GetRequiredPropertiesToConnectStreamInput input = new IIOManager.GetRequiredPropertiesToConnectStreamInput();
                output = clientInterface.GetRequiredPropertiesToConnectStream(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.AddStreamOutput AddStream(string name, StreamMetadataItem streamMetadatas, string userName, string simpleNameToDisplay)
        {
            IIOManager.AddStreamOutput output = new IIOManager.AddStreamOutput();

            try
            {
                IIOManager.AddStreamInput input = new IIOManager.AddStreamInput()
                {
                    ItemName = name,
                    StreamMetadata = new IIOManager.StreamMetadataElement(),
                    ComputerName = Environment.MachineName,
                    UserName = userName,
                    SimpleNameToDisplay = simpleNameToDisplay,
                };
                input.StreamMetadata.StreamShortTypeName = streamMetadatas.StreamShortType;
                foreach (StreamMetadataPropertyItem metadataProperty in streamMetadatas.MetadataProperties)
                {
                    input.StreamMetadata.Properties.Add(new IIOManager.PropertyElement()
                    {
                        PropertyName = metadataProperty.PropertyName,
                        PropertyTypeInString = metadataProperty.PropertyTypeInString.ToString(),
                        PropertyValueInString = metadataProperty.PropertyValueInString
                    });
                }
                output = clientInterface.AddStream(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.SaveOutput Save(string userName, string actionName)
        {
            IIOManager.SaveOutput output = new IIOManager.SaveOutput();

            try
            {
                IIOManager.SaveInput input = new IIOManager.SaveInput()
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.RemoveLinkOutput RemoveLink(long idController, long idStream, long idDatatableItem, string userName, long oldControllerId=0)
        {
            IIOManager.RemoveLinkOutput output = new IIOManager.RemoveLinkOutput();

            try
            {
                IIOManager.RemoveLinkInput input = new IIOManager.RemoveLinkInput()
                {
                    IdController = idController,
                    IdTargetStream = idStream,
                    IdDatatableItem = idDatatableItem,
                    ComputerName = Environment.MachineName,
                    UserName = userName,
                    IdOldController=oldControllerId,
                };
                output = clientInterface.RemoveLink(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.RemoveIoComponentOutput RemoveIoComponent(long idComponent, string userName)
        {
            IIOManager.RemoveIoComponentOutput output = new IIOManager.RemoveIoComponentOutput();

            try
            {
                IIOManager.RemoveIoComponentInput input = new IIOManager.RemoveIoComponentInput()
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        //Append unit Lbo public IIOManager.AddControllerOutput AddController(string name, string shortTypeName)
        //Append for alarm public IIOManager.AddControllerOutput AddController(string name, string shortTypeName, string unit)
     
        public IIOManager.AddControllerOutput AddController(string name, string shortTypeName, string unit, string errormsg, int priority, int type,int mainItemId,string streamSimpleName, double minvalue, double maxvalue,double idController,string componentName,long scaleMin=0,long scaleMax=0) 
        {
            IIOManager.AddControllerOutput output = new IIOManager.AddControllerOutput();
            try
            {
                IIOManager.AddControllerInput input = new IIOManager.AddControllerInput()
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
                    IdController=(long)idController,
                    ComponentName=componentName,
                    ScaleMin = scaleMin,
                    ScaleMax = scaleMax
                };
                output = clientInterface.AddController(input);

                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.EditControllerOutput EditController(long idController, List<IoItemPropertyItem> propertiesToUpdate, List<long> alarmsId, string actionName, string userName,string newSimpleName)
        {
            IIOManager.EditControllerOutput output = new IIOManager.EditControllerOutput();
            try
            {
                IIOManager.EditControllerInput input = new IIOManager.EditControllerInput()
                {
                    ControllerId = idController,
                    ActionName = actionName,
                    UserName = userName,
                    ComputerName = Environment.MachineName,
                    NewSimpleName= newSimpleName,
                };
                foreach (IoItemPropertyItem property in propertiesToUpdate)
                {
                    input.PropertiesToUpdate.Add(new IIOManager.ElementProperty()
                    {
                        PropertyName = property.PropertyName,
                        ParsedToStringPropertyValue = property.PropertyValueInString,
                        IsValue = false
                    });
                }
                input.AlarmsIds.AddRange(alarmsId);
                output = clientInterface.EditController(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }
        }

        public IIOManager.EditStreamOutput EditStream(long idStream, List<IoItemPropertyItem> propertiesToUpdate, string actionName, string userName)
        {
            IIOManager.EditStreamOutput output = new IIOManager.EditStreamOutput();

            try
            {
                IIOManager.EditStreamInput input = new IIOManager.EditStreamInput()
                {
                    StreamId = idStream,
                    ActionName = actionName,
                    ComputerName = Environment.MachineName,
                    UserName = userName
                };
                foreach (IoItemPropertyItem property in propertiesToUpdate)
                {
                    input.PropertiesToUpdate.Add(new IIOManager.ElementProperty()
                    {
                        PropertyName = property.PropertyName,
                        ParsedToStringPropertyValue = property.PropertyValueInString,
                        IsValue = false
                    });
                }
                output = clientInterface.EditStream(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        // For StmLine and Beck
        public IIOManager.SearchAvailableStreamsOutput SearchAvailableStreams(bool StmLine, bool Beck)
        {
            IIOManager.SearchAvailableStreamsOutput output = new IIOManager.SearchAvailableStreamsOutput();

            try
            {
                IIOManager.SearchAvailableStreamsInput input = new IIOManager.SearchAvailableStreamsInput();
                input.Stmline = StmLine;
                input.Beck = Beck;
                output = clientInterface.SearchAvailableStreams(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }
        // For Icp Tcp/Ip
        public IIOManager.SearchAvailableStreamsOutput SearchAvailableStreamsIcp()
        {
            IIOManager.SearchAvailableStreamsOutput output = new IIOManager.SearchAvailableStreamsOutput();

            try
            {
                IIOManager.SearchAvailableStreamsInput input = new IIOManager.SearchAvailableStreamsInput();
                output = clientInterface.SearchAvailableStreamsIcp(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.SearchAvailableStreamsOutput SearchAvailableIndCommunication()
        {
            IIOManager.SearchAvailableStreamsOutput output = new IIOManager.SearchAvailableStreamsOutput();

            try
            {
                IIOManager.SearchAvailableStreamsInput input = new IIOManager.SearchAvailableStreamsInput();
                output = clientInterface.SearchAvailableStreamsIndCommunication(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return output;
        }

        public IIOManager.CheckDllExitsOutput CheckIfDllExists(string name)
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return output;
        }

        public IIOManager.GetAvailableDllStreamListOutput GetAvailableDllStreamList()
        {
            IIOManager.GetAvailableDllStreamListOutput output = new IIOManager.GetAvailableDllStreamListOutput();

            try
            {
                IIOManager.GetAvailableDllStreamListInput input = new IIOManager.GetAvailableDllStreamListInput();
                output = clientInterface.GetAvailableDllStreamList(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return output;
        }

        public IIOManager.RequestAlarmCreationOutput RequestAlarmCreation(string alarmType, int componentId, string userName)
        {
            IIOManager.RequestAlarmCreationOutput output = new IIOManager.RequestAlarmCreationOutput();

            try
            {
                IIOManager.RequestAlarmCreationInput input = new IIOManager.RequestAlarmCreationInput()
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        // Append Technology Table
        public IIOManager.GetTechnologyListOutput GetTechnologyList()
        {
            IIOManager.GetTechnologyListOutput output = new IIOManager.GetTechnologyListOutput();

            try
            {
                IIOManager.GetTechnologyListInput input = new IIOManager.GetTechnologyListInput();

                output = clientInterface.GetTechnologyList(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }

        }

        public IIOManager.AlarmAckOutput AlarmAck()
        {
            IIOManager.AlarmAckOutput output = new IIOManager.AlarmAckOutput();
            try
            {
                IIOManager.AlarmAckInput input = new IIOManager.AlarmAckInput();

                output = clientInterface.AlarmAck(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }
        }
        public IIOManager.GetHighAlarmsOutput GetHighAlarms()
        {
            IIOManager.GetHighAlarmsOutput output = new IIOManager.GetHighAlarmsOutput();
            try
            {
                IIOManager.GetHighAlarmsInput input = new IIOManager.GetHighAlarmsInput();

                output = clientInterface.GetHighAlarms(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }
        }
        public IIOManager.GetStatusIoStreamListOutput GetStatusIoStreamList()
        {
            IIOManager.GetStatusIoStreamListOutput output = new IIOManager.GetStatusIoStreamListOutput();
            try
            {
                IIOManager.GetStatusIoStreamListInput input = new IIOManager.GetStatusIoStreamListInput();

                output = clientInterface.GetStatusIoStreamList(input);
                return output;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return output;
            }
        }

    }
}
