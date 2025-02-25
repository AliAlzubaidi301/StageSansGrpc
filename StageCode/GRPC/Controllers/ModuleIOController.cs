using CodeExceptionManager.Model.Objects;
using Google.Protobuf.Collections;
using IIOManager;
using Orthodyne.CoreCommunicationLayer.Models.IndCom;
using Orthodyne.CoreCommunicationLayer.Models.IO;
using Orthodyne.CoreCommunicationLayer.Models.Technology;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleIoController
    {
        public const string TYPE_PROPERTY_NAME = "ComponentTypeName";
        public const string COMPONENT_NAME_TYPE_NAME = "ComponentName";
        public const string COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY = "SimpleNameToDisplay";
        public const string COMPONENT_COMPUTE_METHOD_NAME = "authorized_compute_method";

        public Dictionary<long, IoController> ioControllers = new Dictionary<long, IoController>();
        public List<IoStream> ioStreams = new List<IoStream>();

        private Dictionary<string, StreamMetadataItem> ioStreamMetadatas = new Dictionary<string, StreamMetadataItem>();
        private ModuleIoRemoteMethodInvocationService remoteMethods;
        private GeneralController generalController;
        //private Dictionary<string, IIoTabController> _usedtabControllers = new Dictionary<string, IIoTabController>();
        //private Dictionary<string, Type> _availabletabControllers = new Dictionary<string, Type>();

        private List<string> usedTypes = new List<string>();
        // Append new Technology table private
        private Dictionary<long, Technology> technologies = new Dictionary<long, Technology>();
        private List<EquipmentItem> availableEquipment = new List<EquipmentItem>();
        //Append StatusIoStream LBO
        private List<StatusIoStream> statusiostreams = new List<StatusIoStream>();

        public bool UnsavedChanges = false;
        public bool SameSimpleNameForStmline { get; set; }

        public Dictionary<long, IoController> GetIoControllers() { return this.ioControllers; }
        public List<IoStream> GetIoStreams() {
            return this.ioStreams; 
        }
        public Dictionary<string, StreamMetadataItem> IoStreamMetadatas { get { return this.ioStreamMetadatas; } }
        public IoController GetIoController(long idComponent)
        {
            //TCO 24/01/2023 add ContainsKey
            if (this.ioControllers.ContainsKey(idComponent))
            {
                return this.ioControllers[idComponent];
            }
            else
            {
                return null;
            }
        }
        public List<string> GetUsedTypes() { return this.usedTypes; }
        // Append new Technology table public
        public Dictionary<long, Technology> Technologies { get { return technologies; } }
        //Append lbo StatusIoStream
        public List<StatusIoStream> StatusIoStreams { get { return statusiostreams; } }
        public List<EquipmentItem> AvailableEquipment { get { return availableEquipment; } }

        /*
        public Dictionary<IoStream, List<GroupSTMLine>> GetSTMLineWithGroups()
        {
            Dictionary<IoStream, List<GroupSTMLine>> connectedSTMLine = new Dictionary<IoStream, List<GroupSTMLine>>();
            
            List<IoStream> stmLines = new List<IoStream>();
            stmLines = ioStreams.Where(x => x.ShortType.ToLower() == "stmline").ToList();

            foreach (IoStream stmLine in stmLines)
            {
                GetStreamGroupsOutput output = remoteMethods.GetStreamGroups(stmLine.Id);

                List<GroupSTMLine> groups = new List<GroupSTMLine>();
                foreach (var item in output.Group)
                {
                    GroupSTMLine group = new GroupSTMLine();
                    group.Id = item.Id;
                    group.GroupSTMLine_Name = item.Name;
                    group.Type = item.Type;
                    group.ParentIds0 = item.ParentIds0;
                    group.ParentIds1 = item.ParentIds0;
                    group.ParentIds2 = item.ParentIds0;
                    group.ParentIds3 = item.ParentIds0;
                    groups.Add(group);
                }

                connectedSTMLine.Add(stmLine, groups);
            }

            return connectedSTMLine;
        }
        */

        internal ModuleIoController(ModuleIoRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {
            try
            {
                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
                this.LoadStreamMetadatas();
                // Load all records from Technology table
                this.LoadTechnologyItem();
                /*
                this.RefreshControllers();
                this.RefreshStreams();*/
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        /// <summary>
        /// Event that defines how to load and store the components from CORE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public List<ControllerElement> RefreshControllers()
        {
            try
            {
                this.ioControllers.Clear();

                IIOManager.GetDefinedControllerOutput output = remoteMethods.GetDefinedIoController();
                UnsavedChanges = output.UnsavedChanges;

                if (output.Controllers.Count > 0)
                {
                    List<ControllerElement> elements = output.Controllers.Where(x => x.ShortTypeName.ToLower() != "det").ToList();
                    int i = 0;

                    foreach (IIOManager.ControllerElement controllerElement in elements)
                    {
                        RegisterController(controllerElement);
                        i++;
                    }

                    return elements;
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return null;
        }

        public void DeleteTechnology(TechnologyItem toDelete)
        {
            try
            {
                remoteMethods.DeleteTechnology(toDelete, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditTechnology(List<TechnologyItem> technologies)
        {
            try
            {
                remoteMethods.EditTechnology(technologies, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void DeleteParameter(ParamItem toDelete)
        {
            try
            {
                remoteMethods.DeleteParameter(toDelete, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void DeleteItem(Item toDelete)
        {
            try
            {
                remoteMethods.DeleteItem(toDelete, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditItem(Item toSave)
        {
            try
            {
                remoteMethods.EditItem(toSave, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditParameters(List<ParamItem> toSave)
        {
            try
            {
                remoteMethods.EditParameter(toSave, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void DeleteEquipment(EquipmentItem toDelete)
        {
            try
            {
                remoteMethods.DeleteEquipment(toDelete, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditEquipment(EquipmentItem equipment)
        {
            try
            {
                remoteMethods.EditEquipment(equipment, generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        private void RegisterController(ControllerElement controller)
        {
            try
            {
                IoController newElement = new IoController()
                {
                    Id = Convert.ToInt32(controller.Identifier),
                    ComponentType = controller.ComponentType,
                    ShortType = controller.ShortTypeName,
                    IsArchive = controller.IsArchive,
                    IsSavedInDB = true,
                };

                foreach (long alarmId in controller.AlarmsId)
                {
                    if (generalController.ModuleAlarmController.Alarms.ContainsKey(alarmId))
                    {
                        newElement.GetAlarms().Add(generalController.ModuleAlarmController.Alarms[alarmId]);
                        if (!generalController.ModuleAlarmController.UsedAlarms.ContainsKey(alarmId))
                        {
                            lock (generalController.ModuleAlarmController.UsedAlarms)
                            {
                                generalController.ModuleAlarmController.UsedAlarms.Add(alarmId, generalController.ModuleAlarmController.Alarms[alarmId]);
                            }
                        }
                    }
                }
                newElement.GetProperties().AddRange(LoadProperties(newElement.Id,true));
                foreach (IIOManager.ComponentFeatureMetadata feature in remoteMethods.GetControllerFeatures(newElement.Id).Features)
                {
                    IoItemFeature newFeature = new IoItemFeature()
                    {
                        FeatureName = feature.FeatureName,
                        VisibleFromUI = feature.VisibleFromUI
                    };
                    newFeature.GetParametersTypesInString().AddRange(feature.ParametersTypeInString);
                    newFeature.GetReturnTypesInString().AddRange(feature.ReturnTypesInString);
                    newElement.GetFeatures().Add(newFeature);
                }
                if (!this.usedTypes.Contains(newElement.GetPropertyByName(TYPE_PROPERTY_NAME).PropertyValueInString))
                {
                    lock (usedTypes)
                    {
                        this.usedTypes.Add(newElement.GetPropertyByName(TYPE_PROPERTY_NAME).PropertyValueInString);
                    }
                }
                if (this.ioControllers.ContainsKey(newElement.Id))
                {
                    this.ioControllers[newElement.Id] = newElement;
                }
                else
                {
                    lock (ioControllers)
                    {
                        this.ioControllers.Add(Convert.ToInt64(newElement.Id), newElement);
                    }
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool GetAutoMode()
        {
            GetAutoModeOutput remoteAutoModeOutput = remoteMethods.GetAutoMode();
            return remoteAutoModeOutput.IsActif;
        }

        public List<IoItemPropertyItem> LoadProperties(int controllerId, bool isController)
        {
            List<IoItemPropertyItem> returnValue = new List<IoItemPropertyItem>();
            try
            {
                if (isController)
                {
                    foreach (IIOManager.ElementProperty property in remoteMethods.GetElementProperties(controllerId).Properties)
                    {
                        IoItemPropertyItem newProperty = new IoItemPropertyItem()
                        {
                            PropertyName = property.PropertyName,
                            PropertyValueInString = property.ParsedToStringPropertyValue,
                            IsEditable = property.IsEditable,
                            DisplayText = property.DisplayText,
                            PropertyType = Type.GetType(property.PropertyTypeInString)
                        };
                        newProperty.AuthorizedValues.AddRange(property.AuthorizedValuesInString);
                        returnValue.Add(newProperty);
                    }
                }
                else
                {
                    foreach (IIOManager.ElementProperty property in remoteMethods.GetElementProperties(controllerId).Properties)
                    {
                        IoItemPropertyItem newProperty = new IoItemPropertyItem()
                        {
                            PropertyName = property.PropertyName,
                            PropertyValueInString = property.ParsedToStringPropertyValue,
                            IsEditable = property.IsEditable,
                            DisplayText = property.DisplayText,
                            PropertyType = Type.GetType(property.PropertyTypeInString)
                        };
                        if(property.PropertyName.ToLower()=="type")
                        {
                            newProperty.IsEditable = false;
                        }
                        newProperty.AuthorizedValues.AddRange(property.AuthorizedValuesInString);
                        returnValue.Add(newProperty);
                    }
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return returnValue;
        }

        public void UpdateControllersValue()
        {
            try
            {
                foreach (IoController ioController in this.ioControllers.Values)
                {
                    ioController.GetValuesInString().Clear();
                    ioController.GetValuesInString().AddRange(remoteMethods.ExecuteControllerFeature(ioController.Id, ioController.GetFeatureByName("GetValue")).ReturnValueInString);
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public string[] ExecuteIoControllerFeature(int idController, string featureName, object[] parameters = null)
        {
            try
            {
                string[] returnValue = this.remoteMethods.ExecuteControllerFeature(idController, this.GetIoController(idController).GetFeatureByName(featureName), parameters).ReturnValueInString.ToArray();
                return returnValue;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return new string[] { ex.ToString() };
            }
        }

        public List<IoController> GetIoControllersByType(string typeName)
        {
            try
            {
                return this.ioControllers.Where(x => x.Value.ComponentType == typeName || x.Value.ShortType == typeName).Select(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return null;
            }

        }

        public void LoadTechnologyItem()
        {
            try
            {
                Technologies.Clear();

                GetTechnologyListOutput output = remoteMethods.GetTechnologyList();
                foreach (TechnologyElement technology in output.Technologies)
                {
                    Technology newItem = new Technology()
                    {
                        Id = technology.Id,
                        Name = technology.Name,
                    };

                    // here we can choose to put as key the Id or the name in the dictionary
                    // technologies.Add(technology.Name, newItem);
                    technologies.Add(technology.Id, newItem);
                }

            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void LoadStreamMetadatas()
        {
            try
            {
                this.IoStreamMetadatas.Clear();
                GetRequiredPropertiesToConnectStreamOutput requiredPropertiesToConnectStreamOutput = remoteMethods.GetRequiredPropertiesToConnectStream();

                foreach (StreamMetadataElement metadatasByType in requiredPropertiesToConnectStreamOutput.RequiredPropertiesByStreamType)
                {
                    StreamMetadataItem newItem = new StreamMetadataItem()
                    {
                        StreamShortType = metadatasByType.StreamShortTypeName
                    };
                    foreach (PropertyElement metadataProperty in metadatasByType.Properties)
                    {
                        newItem.MetadataProperties.Add(new StreamMetadataPropertyItem()
                        {
                            PropertyName = metadataProperty.PropertyName,
                            PropertyTypeInString = metadataProperty.PropertyTypeInString,
                            PropertyValueInString = metadataProperty.PropertyValueInString,
                            LabelForUi = metadataProperty.LabelForUi
                        });
                    }
                    this.IoStreamMetadatas.Add(newItem.StreamShortType, newItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void AddStream(string streamName, StreamMetadataItem metadatas, string simpleNameToDisplay)
        {
            try
            {
                AddStreamOutput addStreamOutput = remoteMethods.AddStream(streamName, metadatas, generalController.ModuleAccessController.UserLogin, simpleNameToDisplay);
                UnsavedChanges = addStreamOutput.UnsavedChanges;
                //SameSimpleNameForStmline=addStreamOutput.SameStmlineName;
                if (addStreamOutput.NewItem != null)
                {
                    RegisterStream(addStreamOutput.NewItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool AlreadyContainsStream(StreamMetadataItem streamMetadata)
        {
            bool returnValue = false;
            try
            {
                int iterations = 0;
                List<IoStream> potententialStreamToProcess = new List<IoStream>();
                List<IoStream> definitiveFoundStream = new List<IoStream>();
                List<IoStream> toRemove = new List<IoStream>();
                if (this.IoStreamMetadatas.ContainsKey(streamMetadata.StreamShortType))
                {
                    foreach (StreamMetadataPropertyItem metadata in streamMetadata.MetadataProperties)
                    {
                        if (this.IoStreamMetadatas[streamMetadata.StreamShortType].MetadataProperties.Where(x => x.PropertyName.ToLower() == metadata.PropertyName.ToLower()).Count() > 0)
                        {
                            potententialStreamToProcess.Clear();
                            toRemove.Clear();
                            //We get all the potential existing items for this metadata (we need to filter to match the expected metadata (for instance only IP Address, as we can get more than just this information)
                            if (this.ioStreams.Where(x => x.GetPropertyByName(metadata.PropertyName) != null &&
                                    x.GetPropertyByName(metadata.PropertyName).PropertyValueInString == metadata.PropertyValueInString &&
                                    x.ShortType == streamMetadata.StreamShortType).Count() > 0)
                            {
                                //potententialStreamToProcess.AddRange(this.ioStreams.Where(x => x.GetPropertyByName(metadata.PropertyName).PropertyValueInString == metadata.PropertyValueInString).ToList());
                                potententialStreamToProcess.AddRange(this.ioStreams.Where(x => x.GetPropertyByName(metadata.PropertyName) != null && x.GetPropertyByName(metadata.PropertyName).PropertyValueInString == metadata.PropertyValueInString).ToList());
                            }
                            //We add them to the consolidated ones if there aren't inside if this is the first iteration (because if this isn't, this means that this is not the same item as one parameter at least hasn(t match)
                            foreach (IoStream foundStream in potententialStreamToProcess)
                            {
                                if (!definitiveFoundStream.Contains(foundStream) && iterations == 0)
                                {
                                    definitiveFoundStream.Add(foundStream);
                                }
                            }
                            //Then we remove the ones that aren't found here from the consolidated items
                            foreach (IoStream alreadyStored in definitiveFoundStream)
                            {
                                if (!potententialStreamToProcess.Contains(alreadyStored))
                                {
                                    toRemove.Add(alreadyStored);
                                }
                            }
                            foreach (IoStream toRemoveItem in toRemove)
                            {
                                definitiveFoundStream.Remove(toRemoveItem);
                            }
                            iterations++;
                        }
                    }
                }
                if (definitiveFoundStream.Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                returnValue = false;
            }
            return returnValue;
        }

        public void Save(string actionName)
        {
            try
            {
                remoteMethods.Save(generalController.ModuleAccessController.UserLogin, actionName);
                UnsavedChanges = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void RemoveLink(long controllerId, long streamId, long datatableItemId, long oldControllerId = 0)
        {
            try
            {
                if (this.GetIoStreams().Where(x => x.Id == streamId).Count() > 0 &&
                    this.GetIoControllers().ContainsKey(controllerId))
                {
                    //GetIoStreams().Remove(GetIoControllers().[controllerId]);
                    //IoStream targetStream = this.GetIoStreams().FirstOrDefault(x => x.Id == streamId);
                    //IoController controller = this.GetIoControllers()[controllerId];
                    //if (targetStream != null
                    //    && GetIoControllers()[controllerId] != null
                    //    && targetStream.GetIoStreamDataTableItems().Where(x => x.Id == datatableItemId).Count() > 0)
                    //{
                    //    IoStreamDataTableItem dataTableItem = targetStream.GetIoStreamDataTableItems().FirstOrDefault(x => x.Id == datatableItemId);
                    //    dataTableItem.GetLinkedControllers().Remove(dataTableItem.GetLinkedControllers().FirstOrDefault(x => x.IdController == controllerId));
                    //    controller.GetLinkWithStreams().Remove(controller.GetLinkWithStreams().FirstOrDefault(x => x.IdStream == streamId && x.IdDataTableItem == datatableItemId));
                    //}
                    IoStream targetStream = this.GetIoStreams().Where(x => x.Id == streamId).FirstOrDefault();
                    IoController controller = this.GetIoControllers()[controllerId];
                    if (targetStream != null && controller != null && targetStream.GetIoStreamDataTableItems().Where(x => x.Id == datatableItemId).Count() > 0)
                    {
                        remoteMethods.RemoveLink(controllerId, streamId, datatableItemId, generalController.ModuleAccessController.UserLogin, oldControllerId);
                        IoStreamDataTableItem dataTableItem = targetStream.GetIoStreamDataTableItems().Where(x => x.Id == datatableItemId).FirstOrDefault();
                        //dataTableItem.GetLinkedControllers().Remove(dataTableItem.GetLinkedControllers().Where(x => x.IdController == controllerId).FirstOrDefault());
                        //controller.GetLinkWithStreams().Remove(controller.GetLinkWithStreams().Where(x => x.IdStream == streamId && x.IdDataTableItem == datatableItemId).FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void RemoveIoComponent(long componentId)
        {
            try
            {
                //Stream ou Controller ?
                if (ioControllers.ContainsKey(componentId))
                {
                    IoController ioController = ioControllers[componentId];
                    List<LinkItem> links = ioController.GetLinkWithStreams();
                    foreach (LinkItem link in links)
                    {
                        foreach (IoStreamDataTableItem ioStreamDataTableItem in ioStreams.Where(x => x.Id == link.IdStream).FirstOrDefault().GetIoStreamDataTableItems())
                        {
                            LinkItem linkItem = ioStreamDataTableItem.GetLinkedControllers().Where(x => x.IdController == componentId).FirstOrDefault();
                            if (linkItem != null)
                            {
                                ioStreamDataTableItem.GetLinkedControllers().Remove(linkItem);
                            }
                        }
                    }
                    ioControllers.Remove(componentId);
                }
                else
                {
                    if (ioStreams.Where(x => x.Id == componentId).FirstOrDefault() != null)
                    {
                        //Avant de remove le Stream on en supprime d'abord tous les controllers liés
                        IoStream ioStream = ioStreams.Where(x => x.Id == componentId).FirstOrDefault();
                        List<IoStreamDataTableItem> ioStreamDataTableItems = ioStream.GetIoStreamDataTableItems().Where(x => x.GetLinkedControllers().Count > 0).ToList();
                        foreach (IoStreamDataTableItem ioStreamDataTableItem in ioStreamDataTableItems)
                        {
                            List<LinkItem> linkItems = ioStreamDataTableItem.GetLinkedControllers().ToList();
                            foreach (LinkItem linkItem in linkItems)
                            {
                                //ioControllers[linkItem.IdController].GetLinkWithStreams().Clear();
                                if (ioControllers[linkItem.IdController].ComponentName.Contains($"{ioStreamDataTableItem.Name}("))
                                {
                                    ioControllers.Remove(linkItem.IdController);
                                    //remoteMethods.RemoveLink(linkItem.IdController, ioStream.Id, linkItem.IdDataTableItem, generalController.ModuleAccessController.UserLogin);
                                    remoteMethods.RemoveIoComponent(linkItem.IdController, generalController.ModuleAccessController.UserLogin);
                                }
                            }
                            //On suprime les links sur les controleurs
                            foreach (LinkItem linkItem in linkItems)
                            {
                                ioStreamDataTableItem.GetLinkedControllers().Remove(linkItem);
                            }

                        }
                        //Puis on supprime le Stream
                        ioStreams.Remove(ioStream);
                    }
                }

                remoteMethods.RemoveIoComponent(componentId, generalController.ModuleAccessController.UserLogin);

                UnsavedChanges = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //Lbo Append Unit if present
        //public void AddController(string name, string shortTypeName, int idTargetStream = -1, int idDataTableElement = -1, IODirectionEnum iODirection = IODirectionEnum.Output)
        //Lbo append alarms
        //public void AddController(string name, string shortTypeName, string unit = "", int idTargetStream = -1, int idDataTableElement = -1, IODirectionEnum iODirection = IODirectionEnum.Output)
        //Tco append MainItemId - 0 Main card OrthoProcess 1-4 for detector
        //public void AddController(string name, string shortTypeName, string unit = "", int idTargetStream = -1, int idDataTableElement = -1,
        //                          IODirectionEnum iODirection = IODirectionEnum.Output, string ErrorMsg = "", int Priority = -1, int Type = -1)
     

        private void RegisterStream(StreamElement stream)
        {
            try
            {
                IoStream tmpstream = new IoStream()
                {
                    Id = Convert.ToInt32(stream.Identifier),
                    ComponentType = stream.ComponentType,
                    ShortType = stream.ShortTypeName,
                    IsArchive = stream.IsArchive
                };
                tmpstream.GetProperties().AddRange(LoadProperties(tmpstream.Id,false));
                RepeatedField<StreamDataTableElement> streamDataTable = remoteMethods.GetStreamDataTable(Convert.ToInt32(stream.Identifier)).DataTable;
                foreach (IIOManager.StreamDataTableElement dataTableItem in streamDataTable)
                {
                    IoStreamDataTableItem ioStreamDataTable = new IoStreamDataTableItem()
                    {
                        Name = dataTableItem.DataName,
                        Unit = dataTableItem.Unit,
                        ErrorMsg = dataTableItem.Errormsg,
                        Priority = (int)dataTableItem.Priority,
                        Type = (int)dataTableItem.Type,
                        Id = Convert.ToInt32(dataTableItem.Id),
                        MinValue = Convert.ToDouble(dataTableItem.Minvalue),
                        MaxValue = Convert.ToDouble(dataTableItem.Maxvalue)
                    };
                    foreach (IIOManager.ElementProperty elementPropertyAsIdentifier in dataTableItem.PropertiesAsIdentifier)
                    {
                        ioStreamDataTable.GetIdentifierProperties().Add(new IoItemPropertyItem()
                        {
                            PropertyName = elementPropertyAsIdentifier.PropertyName,
                            PropertyValueInString = elementPropertyAsIdentifier.ParsedToStringPropertyValue
                        });
                    }
                    if (this.ioControllers.Count > 0)
                    {
                        foreach (LinkElement linkWithcontroller in dataTableItem.LinkedControllers)
                        {
                            if (this.ioControllers.Where(x => x.Key == linkWithcontroller.IdController).Count() == 1)
                            {
                                LinkItem newLink = new LinkItem()
                                {
                                    IdController = Convert.ToInt32(linkWithcontroller.IdController),
                                    IdStream = Convert.ToInt32(stream.Identifier),
                                    IdDataTableItem = Convert.ToInt32(dataTableItem.Id)
                                };
                                if (ioControllers != null && ioControllers.Where(x => x.Key == Convert.ToInt32(linkWithcontroller.IdController)).Count() > 0)
                                {
                                    //Test when we change Link of controller
                                    if (ioControllers.Where(x => x.Key == Convert.ToInt32(linkWithcontroller.IdController)).First().Value.GetLinkWithStreams().Where(x => x.IdStream == newLink.IdStream && x.IdDataTableItem == newLink.IdDataTableItem).Count() < 1)
                                    {
                                        ioControllers.Where(x => x.Key == Convert.ToInt32(linkWithcontroller.IdController)).First().Value.GetLinkWithStreams().Add(newLink);
                                    }
                                }
                                //Test when we change Lin of controller
                                if (ioStreamDataTable.GetLinkedControllers().Where(x => x.IdController == newLink.IdController).Count() < 1)
                                {
                                    //Only 1 link between stream and each controller
                                    ioStreamDataTable.GetLinkedControllers().Add(newLink);
                                }
                            }
                        }
                    }
                    foreach (string item in dataTableItem.AuthorizedControllerTypes)
                    {
                        ioStreamDataTable.GetauthorizedControllerTypes().Add(item);
                    }
                    tmpstream.GetIoStreamDataTableItems().Add(ioStreamDataTable);
                }
                this.ioStreams.Add(tmpstream);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public string EditController(IoController controller, string actionName)
        {
            string errorMessage = string.Empty;
            try
            {
                if(controller.SimpleNameNewValue==null)
                {
                    controller.SimpleNameNewValue = controller.SimpleNameToDisplay;
                }
                IIOManager.EditControllerOutput output = remoteMethods.EditController(controller.Id, controller.GetProperties().Where(x => x.IsEditable).ToList(), controller.GetAlarms().Select(x => x.Id).ToList(), actionName, generalController.ModuleAccessController.UserLogin, controller.SimpleNameNewValue);
                if (output != null && output.ErrorMessage != string.Empty)
                {

                    errorMessage = output.ErrorMessage;
                }
                return errorMessage;


                //TCO 23/03/2023 pourquoi recharger tout ? 
                //Cela faisait que le nom du IoController disparaissait
                /*
                this.ioControllers[Convert.ToInt64(controller.Id)].GetAlarms().Clear();
                this.ioControllers[Convert.ToInt64(controller.Id)].GetAlarms().AddRange(controller.GetAlarms());
                this.ioControllers[Convert.ToInt64(controller.Id)].GetProperties().Clear();
                this.ioControllers[Convert.ToInt64(controller.Id)].GetProperties().AddRange(controller.GetProperties());
                */
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return errorMessage;
            }
        }

        public Dictionary<string, IoItemPropertyItem> GetElementProperties(int stremId)
        {
            Dictionary<string, IoItemPropertyItem> keyValuePairs = new Dictionary<string, IoItemPropertyItem>();
            foreach (IIOManager.ElementProperty property in remoteMethods.GetElementProperties(stremId).Properties)
            {
                IoItemPropertyItem newProperty = new IoItemPropertyItem()
                {
                    PropertyName = property.PropertyName,
                    PropertyValueInString = property.ParsedToStringPropertyValue,
                    IsEditable = property.IsEditable,
                    DisplayText = property.DisplayText,
                    PropertyType = Type.GetType(property.PropertyTypeInString)
                };
                newProperty.AuthorizedValues.AddRange(property.AuthorizedValuesInString);
                keyValuePairs.Add(newProperty.PropertyName, newProperty);
            }

            return keyValuePairs;
        }

        public void LoadEquipment()
        {
            availableEquipment.Clear();
            foreach (var item in remoteMethods.LoadEquipment().Equipments)
            {
                EquipmentItem tmp = new EquipmentItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    //Type =  (Models.IndCom.IndCommTypeEnum)Enum.Parse(typeof(Models.IndCom.IndCommTypeEnum),item.Type),
                    Type = (Models.IndCom.IndCommTypeEnum)item.Type,
                    Items = item.Items.Select(x => new Item
                    {
                        Adress = double.Parse(x.Adress),
                        Name = x.Name,
                        Id = x.Id,
                        IdEqu = item.Id,
                        VarTypeCom = (VarTypeComEnum)x.VarTypeCom,
                        VarTypeOrtho = (Models.IndCom.IndCommTypeComponentEnum)x.VarTypeOrtho,
                    }).ToList(),
                    Params = item.Params.Select(x => new ParamItem
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Value = x.Value,
                        IdEqu = item.Id,
                    }).ToList()
                };
                availableEquipment.Add(tmp);
            }
        }

        public bool CheckIfDllExists(string name)
        {
            return remoteMethods.CheckIfDllExists(name).Exists;
        }

        public List<string> GetAvailableDllStream()
        {
            List<string> availableDllStream = new List<string>();
            foreach (IIOManager.DllStreamElement streamElement in remoteMethods.GetAvailableDllStreamList().DllStreams.ToList())
            {
                availableDllStream.Add(streamElement.Name);
            }

            return availableDllStream;
        }

        public void EditStream(IoStream stream, string actionName)
        {
            try
            {
                EditStreamOutput output = remoteMethods.EditStream(stream.Id, stream.GetProperties().Where(x => x.IsEditable).ToList(), "Confirm", generalController.ModuleAccessController.UserLogin);
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        // Search for StmLine and Beck
        public List<StreamMetadataItem> SearchAvailableStreams(bool StmLine, bool Beck)
        {
            List<StreamMetadataItem> returnValue = new List<StreamMetadataItem>();
            try
            {
                foreach (StreamMetadataElement foundedElement in remoteMethods.SearchAvailableStreams(StmLine, Beck).FoundedStreams)
                {
                    StreamMetadataItem newItem = new StreamMetadataItem()
                    {
                        StreamShortType = foundedElement.StreamShortTypeName
                    };
                    foreach (PropertyElement metadataProperty in foundedElement.Properties)
                    {
                        newItem.MetadataProperties.Add(new StreamMetadataPropertyItem()
                        {
                            PropertyName = metadataProperty.PropertyName,
                            PropertyTypeInString = metadataProperty.PropertyTypeInString,
                            PropertyValueInString = metadataProperty.PropertyValueInString,
                            LabelForUi = metadataProperty.LabelForUi
                        });
                    }
                    returnValue.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return returnValue;
        }

        //Search for Icp Tcp/Ip
        public List<StreamMetadataItem> SearchAvailableStreamsIcp()
        {
            List<StreamMetadataItem> returnValue = new List<StreamMetadataItem>();

            try
            {
                foreach (StreamMetadataElement foundedElement in remoteMethods.SearchAvailableStreamsIcp().FoundedStreams)
                {
                    StreamMetadataItem newItem = new StreamMetadataItem()
                    {
                        StreamShortType = foundedElement.StreamShortTypeName
                    };
                    foreach (PropertyElement metadataProperty in foundedElement.Properties)
                    {
                        newItem.MetadataProperties.Add(new StreamMetadataPropertyItem()
                        {
                            PropertyName = metadataProperty.PropertyName,
                            PropertyTypeInString = metadataProperty.PropertyTypeInString,
                            PropertyValueInString = metadataProperty.PropertyValueInString,
                            LabelForUi = metadataProperty.LabelForUi
                        });
                    }
                    returnValue.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return returnValue;
        }

        public List<StreamMetadataItem> SearchAvailableIndCommunication()
        {
            List<StreamMetadataItem> returnValue = new List<StreamMetadataItem>();

            try
            {
                foreach (StreamMetadataElement foundedElement in remoteMethods.SearchAvailableIndCommunication().FoundedStreams)
                {
                    StreamMetadataItem newItem = new StreamMetadataItem()
                    {
                        StreamShortType = foundedElement.StreamShortTypeName
                    };
                    foreach (PropertyElement metadataProperty in foundedElement.Properties)
                    {
                        newItem.MetadataProperties.Add(new StreamMetadataPropertyItem()
                        {
                            PropertyName = metadataProperty.PropertyName,
                            PropertyTypeInString = metadataProperty.PropertyTypeInString,
                            PropertyValueInString = metadataProperty.PropertyValueInString,
                            LabelForUi = metadataProperty.LabelForUi
                        });
                    }
                    returnValue.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }

            return returnValue;
        }

        //à garder
        //public void RequestAlarmCancellation(int componentId)
        //{
        //    try
        //    {
        //        RequestAlarmCancellationOutput requestOutput= remoteMethods.RequestAlarmCancellation(componentId);
        //        //generalController.ModuleAlarmController.LoadAlarms();
        //    }
        //    catch (Exception ex)
        //    {
        //        new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }
        //}


        public void AlarmAck()
        {
            try
            {
                AlarmAckOutput requestOutput = remoteMethods.AlarmAck();
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public int GetHighAlarms()
        {
            int returnValue = 0;
            try
            {
                GetHighAlarmsOutput requestOutput = remoteMethods.GetHighAlarms();
                returnValue = (int)requestOutput.Level;
            }

            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
            return returnValue;
        }
        public void LoadStatusIoStreamItem()
        {
            try
            {
                statusiostreams.Clear();

                GetStatusIoStreamListOutput output = remoteMethods.GetStatusIoStreamList();
                foreach (StatusIoStreamElement statusiostream in output.Statusiostream)
                {
                    StatusIoStream newItem = new StatusIoStream()
                    {
                        IoStreamId = statusiostream.IoStreamId,
                        IsStatus = statusiostream.IsStatus,
                        SimpleNameToDisplay = statusiostream.SimpleNameToDisplay,
                    };

                    // here we can choose to put as key the Id or the name in the dictionary
                    // technologies.Add(technology.Name, newItem);
                    statusiostreams.Add(newItem);
                }
                //Pour mettre le General en position 1
                //On sélectionne le général
                StatusIoStream statusIoStream1 = statusiostreams.Where(x => x.IoStreamId == -1).FirstOrDefault();
                statusiostreams = statusiostreams
                    .OrderBy(x => x == statusIoStream1 ? 0 : 1)
                    .ThenBy(x => x.SimpleNameToDisplay)
                    .ToList();
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

    }
    // Append for Hardware detection for Stm/Beck or Icp Tcp/Ip
    public enum EnumTypeStream
    {
        StmLine_Beck,
        Icp_Tcp
    }

}
