#region assembly Orthodyne.CoreCommunicationLayer, Version=0.1.0.1, Culture=neutral, PublicKeyToken=null
// Z:\CommunicationLayer\Orthodyne.CoreCommunicationLayer.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CodeExceptionManager.Model.Objects;
using Google.Protobuf.Collections;
using IIOManager;
using Orthodyne.Common.Configuration.IO;
using Orthodyne.CoreCommunicationLayer.Models.Alarms;
using Orthodyne.CoreCommunicationLayer.Models.IndCom;
using Orthodyne.CoreCommunicationLayer.Models.IO;
using Orthodyne.CoreCommunicationLayer.Models.Technology;
using Orthodyne.CoreCommunicationLayer.Services;

namespace Orthodyne.CoreCommunicationLayer.Controllers;

public class ModuleIoControllerOrthoDesigner
{
    public const string TYPE_PROPERTY_NAME = "ComponentTypeName";

    public const string COMPONENT_NAME_TYPE_NAME = "ComponentName";

    public const string COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY = "SimpleNameToDisplay";

    public const string COMPONENT_COMPUTE_METHOD_NAME = "authorized_compute_method";

    public Dictionary<long, IoController> ioControllers = new Dictionary<long, IoController>();

    public  List<IoStream> ioStreams = new List<IoStream>();

    private Dictionary<string, StreamMetadataItem> ioStreamMetadatas = new Dictionary<string, StreamMetadataItem>();

    private ModuleIoRemoteMethodInvocationService remoteMethods;

    private GeneralController generalController;

    private List<string> usedTypes = new List<string>();

    private Dictionary<long, Technology> technologies = new Dictionary<long, Technology>();

    private List<EquipmentItem> availableEquipment = new List<EquipmentItem>();

    private List<StatusIoStream> statusiostreams = new List<StatusIoStream>();

    public bool UnsavedChanges = false;

    public bool SameSimpleNameForStmline { get; set; }

    public Dictionary<string, StreamMetadataItem> IoStreamMetadatas => ioStreamMetadatas;

    public Dictionary<long, Technology> Technologies => technologies;

    public List<StatusIoStream> StatusIoStreams => statusiostreams;

    public List<EquipmentItem> AvailableEquipment => availableEquipment;

    public Dictionary<long, IoController> GetIoControllers()
    {
        return ioControllers;
    }

    public List<IoStream> GetIoStreams()
    {
        return ioStreams;
    }

    public IoController GetIoController(long idComponent)
    {
        if (ioControllers.ContainsKey(idComponent))
        {
            return ioControllers[idComponent];
        }

        return null;
    }

    public List<string> GetUsedTypes()
    {
        return usedTypes;
    }

    public ModuleIoControllerOrthoDesigner(ModuleIoRemoteMethodInvocationService remoteMethods, GeneralController generalController)
    {
        try
        {
            this.generalController = generalController;
            this.remoteMethods = remoteMethods;
            LoadStreamMetadatas();
            LoadTechnologyItem();
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void RefreshControllers()
    {
        try
        {
            ioControllers.Clear();
            GetDefinedControllerOutput definedIoController = remoteMethods.GetDefinedIoController();
            UnsavedChanges = definedIoController.UnsavedChanges;
            if (definedIoController.Controllers.Count <= 0)
            {
                return;
            }

            List<ControllerElement> list = definedIoController.Controllers.Where((ControllerElement x) => x.ShortTypeName.ToLower() != "det").ToList();
            int num = 0;
            foreach (ControllerElement item in list)
            {
                RegisterController(item);
                num++;
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void DeleteTechnology(TechnologyItem toDelete)
    {
        try
        {
            remoteMethods.DeleteTechnology(toDelete, generalController.ModuleAccessController.UserLogin);
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    private void RegisterController(ControllerElement controller)
    {
        try
        {
            IoController ioController = new IoController
            {
                Id = Convert.ToInt32(controller.Identifier),
                ComponentType = controller.ComponentType,
                ShortType = controller.ShortTypeName,
                IsArchive = controller.IsArchive,
                IsSavedInDB = true
            };
            foreach (long item in controller.AlarmsId)
            {
                if (!generalController.ModuleAlarmController.Alarms.ContainsKey(item))
                {
                    continue;
                }

                ioController.GetAlarms().Add(generalController.ModuleAlarmController.Alarms[item]);
                if (!generalController.ModuleAlarmController.UsedAlarms.ContainsKey(item))
                {
                    lock (generalController.ModuleAlarmController.UsedAlarms)
                    {
                        generalController.ModuleAlarmController.UsedAlarms.Add(item, generalController.ModuleAlarmController.Alarms[item]);
                    }
                }
            }

            ioController.GetProperties().AddRange(LoadProperties(ioController.Id, isController: true));
            foreach (ComponentFeatureMetadata feature in remoteMethods.GetControllerFeatures(ioController.Id).Features)
            {
                IoItemFeature ioItemFeature = new IoItemFeature
                {
                    FeatureName = feature.FeatureName,
                    VisibleFromUI = feature.VisibleFromUI
                };
                ioItemFeature.GetParametersTypesInString().AddRange(feature.ParametersTypeInString);
                ioItemFeature.GetReturnTypesInString().AddRange(feature.ReturnTypesInString);
                ioController.GetFeatures().Add(ioItemFeature);
            }

            if (!usedTypes.Contains(ioController.GetPropertyByName("ComponentTypeName").PropertyValueInString))
            {
                lock (usedTypes)
                {
                    usedTypes.Add(ioController.GetPropertyByName("ComponentTypeName").PropertyValueInString);
                }
            }

            if (ioControllers.ContainsKey(ioController.Id))
            {
                ioControllers[ioController.Id] = ioController;
                return;
            }

            lock (ioControllers)
            {
                ioControllers.Add(Convert.ToInt64(ioController.Id), ioController);
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public bool GetAutoMode()
    {
        GetAutoModeOutput autoMode = remoteMethods.GetAutoMode();
        return autoMode.IsActif;
    }

    public List<IoItemPropertyItem> LoadProperties(int controllerId, bool isController)
    {
        List<IoItemPropertyItem> list = new List<IoItemPropertyItem>();
        try
        {
            if (isController)
            {
                foreach (ElementProperty property in remoteMethods.GetElementProperties(controllerId).Properties)
                {
                    IoItemPropertyItem ioItemPropertyItem = new IoItemPropertyItem
                    {
                        PropertyName = property.PropertyName,
                        PropertyValueInString = property.ParsedToStringPropertyValue,
                        IsEditable = property.IsEditable,
                        DisplayText = property.DisplayText,
                        PropertyType = Type.GetType(property.PropertyTypeInString)
                    };
                    ioItemPropertyItem.AuthorizedValues.AddRange(property.AuthorizedValuesInString);
                    list.Add(ioItemPropertyItem);
                }
            }
            else
            {
                foreach (ElementProperty property2 in remoteMethods.GetElementProperties(controllerId).Properties)
                {
                    IoItemPropertyItem ioItemPropertyItem2 = new IoItemPropertyItem
                    {
                        PropertyName = property2.PropertyName,
                        PropertyValueInString = property2.ParsedToStringPropertyValue,
                        IsEditable = property2.IsEditable,
                        DisplayText = property2.DisplayText,
                        PropertyType = Type.GetType(property2.PropertyTypeInString)
                    };
                    if (property2.PropertyName.ToLower() == "type")
                    {
                        ioItemPropertyItem2.IsEditable = false;
                    }

                    ioItemPropertyItem2.AuthorizedValues.AddRange(property2.AuthorizedValuesInString);
                    list.Add(ioItemPropertyItem2);
                }
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }

        return list;
    }

    public void UpdateControllersValue()
    {
        try
        {
            foreach (IoController value in ioControllers.Values)
            {
                value.GetValuesInString().Clear();
                value.GetValuesInString().AddRange(remoteMethods.ExecuteControllerFeature(value.Id, value.GetFeatureByName("GetValue")).ReturnValueInString);
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public string[] ExecuteIoControllerFeature(int idController, string featureName, object[] parameters = null)
    {
        try
        {
            return remoteMethods.ExecuteControllerFeature(idController, GetIoController(idController).GetFeatureByName(featureName), parameters).ReturnValueInString.ToArray();
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            return new string[1] { ex.ToString() };
        }
    }

    public List<IoController> GetIoControllersByType(string typeName)
    {
        try
        {
            return (from x in ioControllers
                    where x.Value.ComponentType == typeName || x.Value.ShortType == typeName
                    select x.Value).ToList();
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            return null;
        }
    }

    public void UndoChanges()
    {
        try
        {
            remoteMethods.UndoChanges();
            RefreshControllers();
            RefreshStreams();
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void RefreshStreams()
    {
        try
        {
            ioStreams.Clear();
            GetDefinedStreamsOutput definedStreams = remoteMethods.GetDefinedStreams();
            UnsavedChanges = definedStreams.UnsavedChanges;
            List<StreamElement> list = definedStreams.Streams.Where((StreamElement x) => !x.IsArchive).ToList();
            Task[] array = new Task[list.Count];
            int num = 0;
            foreach (StreamElement item in list)
            {
                num++;
                if (!item.IsArchive)
                {
                    RegisterStream(item);
                }
            }

            foreach (IoController value in ioControllers.Values)
            {
                if (value.GetLinkWithStreams() == null)
                {
                    continue;
                }

                Dictionary<IODirectionEnum, List<LinkItem>> source = (from x in value.GetLinkWithStreams()
                                                                      group x by x.IODirection).ToDictionary((IGrouping<IODirectionEnum, LinkItem> x) => x.Key, (IGrouping<IODirectionEnum, LinkItem> x) => x.Select((LinkItem y) => y).ToList());
                value.GetLinkWithStreams().Clear();
                switch (value.ShortType.ToLower())
                {
                    case "rel":
                        foreach (KeyValuePair<IODirectionEnum, List<LinkItem>> item2 in from x in source
                                                                                        where x.Value.Count == 1
                                                                                        where x.Value.Where((LinkItem y) => y.IODirection == IODirectionEnum.TwoWay).Any()
                                                                                        select x)
                        {
                            value.GetLinkWithStreams().AddRange(item2.Value);
                        }

                        foreach (KeyValuePair<IODirectionEnum, List<LinkItem>> item3 in from x in source
                                                                                        where x.Value.Count == 1
                                                                                        where x.Value.Where((LinkItem y) => y.IODirection != IODirectionEnum.TwoWay).Any()
                                                                                        select x)
                        {
                            value.GetLinkWithStreams().AddRange(item3.Value);
                        }

                        break;
                    case "di":
                    case "ad":
                    case "signal":
                    case "ala":
                        foreach (KeyValuePair<IODirectionEnum, List<LinkItem>> item4 in from x in source
                                                                                        where x.Value.Count == 1
                                                                                        where x.Value.Where((LinkItem y) => y.IODirection == IODirectionEnum.Input).Any()
                                                                                        select x)
                        {
                            value.GetLinkWithStreams().AddRange(item4.Value);
                        }

                        foreach (KeyValuePair<IODirectionEnum, List<LinkItem>> item5 in from x in source
                                                                                        where x.Value.Count == 1
                                                                                        where x.Value.Where((LinkItem y) => y.IODirection != IODirectionEnum.Input).Any()
                                                                                        select x)
                        {
                            value.GetLinkWithStreams().AddRange(item5.Value);
                        }

                        break;
                    default:
                        foreach (KeyValuePair<IODirectionEnum, List<LinkItem>> item6 in source.Where((KeyValuePair<IODirectionEnum, List<LinkItem>> x) => x.Value.Count == 1))
                        {
                            value.GetLinkWithStreams().AddRange(item6.Value);
                        }

                        break;
                }

                foreach (KeyValuePair<IODirectionEnum, List<LinkItem>> item7 in source.Where((KeyValuePair<IODirectionEnum, List<LinkItem>> x) => x.Value.Count != 1))
                {
                    value.GetLinkWithStreams().AddRange(item7.Value);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void LinkControllerToStream(int idTargetStream, int idController, int idDataTableElement, IODirectionEnum iODirection)
    {
        try
        {
            if ((from x in GetIoStreams()
                 where x.Id == idTargetStream
                 select x).Count() <= 0 || !GetIoControllers().ContainsKey(idController))
            {
                return;
            }

            LinkItem item = new LinkItem
            {
                IdController = idController,
                IODirection = iODirection,
                IdStream = idTargetStream,
                IdDataTableItem = idDataTableElement
            };
            EditLinkWithStreamOutput editLinkWithStreamOutput = remoteMethods.EditLinkWithStream(idTargetStream, idController, idDataTableElement, iODirection, generalController.ModuleAccessController.UserLogin);
            IoController ioController = GetIoControllers()[idController];
            ioController.GetLinkWithStreams().Add(item);
            IoStreamDataTableItem ioStreamDataTableItem = (from x in (from x in GetIoStreams()
                                                                      where x.Id == idTargetStream
                                                                      select x).First().GetIoStreamDataTableItems()
                                                           where x.Id == idDataTableElement
                                                           select x).FirstOrDefault();
            if (ioStreamDataTableItem == null)
            {
                return;
            }

            if (!ioStreamDataTableItem.GetLinkedControllers().Exists((LinkItem x) => x.IdController == idController))
            {
                ioStreamDataTableItem.GetLinkedControllers().Add(item);
                return;
            }

            (from x in ioStreamDataTableItem.GetLinkedControllers()
             where x.IdController == idController
             select x).FirstOrDefault().IODirection = iODirection;
            (from x in ioController.GetLinkWithStreams()
             where x.IdStream == idTargetStream && x.IdDataTableItem == idDataTableElement
             select x).FirstOrDefault().IODirection = iODirection;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void LoadTechnologyItem()
    {
        try
        {
            Technologies.Clear();
            GetTechnologyListOutput technologyList = remoteMethods.GetTechnologyList();
            foreach (TechnologyElement technology in technologyList.Technologies)
            {
                Technology value = new Technology
                {
                    Id = technology.Id,
                    Name = technology.Name
                };
                technologies.Add(technology.Id, value);
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void LoadStreamMetadatas()
    {
        try
        {
            IoStreamMetadatas.Clear();
            GetRequiredPropertiesToConnectStreamOutput requiredPropertiesToConnectStream = remoteMethods.GetRequiredPropertiesToConnectStream();
            foreach (StreamMetadataElement item in requiredPropertiesToConnectStream.RequiredPropertiesByStreamType)
            {
                StreamMetadataItem streamMetadataItem = new StreamMetadataItem
                {
                    StreamShortType = item.StreamShortTypeName
                };
                foreach (PropertyElement property in item.Properties)
                {
                    streamMetadataItem.MetadataProperties.Add(new StreamMetadataPropertyItem
                    {
                        PropertyName = property.PropertyName,
                        PropertyTypeInString = property.PropertyTypeInString,
                        PropertyValueInString = property.PropertyValueInString,
                        LabelForUi = property.LabelForUi
                    });
                }

                IoStreamMetadatas.Add(streamMetadataItem.StreamShortType, streamMetadataItem);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void AddStream(string streamName, StreamMetadataItem metadatas, string simpleNameToDisplay)
    {
        try
        {
            AddStreamOutput addStreamOutput = remoteMethods.AddStream(streamName, metadatas, generalController.ModuleAccessController.UserLogin, simpleNameToDisplay);
            UnsavedChanges = addStreamOutput.UnsavedChanges;
            if (addStreamOutput.NewItem != null)
            {
                RegisterStream(addStreamOutput.NewItem);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public bool AlreadyContainsStream(StreamMetadataItem streamMetadata)
    {
        bool flag = false;
        try
        {
            int num = 0;
            List<IoStream> list = new List<IoStream>();
            List<IoStream> list2 = new List<IoStream>();
            List<IoStream> list3 = new List<IoStream>();
            if (IoStreamMetadatas.ContainsKey(streamMetadata.StreamShortType))
            {
                foreach (StreamMetadataPropertyItem metadata in streamMetadata.MetadataProperties)
                {
                    if (IoStreamMetadatas[streamMetadata.StreamShortType].MetadataProperties.Where((StreamMetadataPropertyItem x) => x.PropertyName.ToLower() == metadata.PropertyName.ToLower()).Count() <= 0)
                    {
                        continue;
                    }

                    list.Clear();
                    list3.Clear();
                    if (ioStreams.Where((IoStream x) => x.GetPropertyByName(metadata.PropertyName) != null && x.GetPropertyByName(metadata.PropertyName).PropertyValueInString == metadata.PropertyValueInString && x.ShortType == streamMetadata.StreamShortType).Count() > 0)
                    {
                        list.AddRange(ioStreams.Where((IoStream x) => x.GetPropertyByName(metadata.PropertyName) != null && x.GetPropertyByName(metadata.PropertyName).PropertyValueInString == metadata.PropertyValueInString).ToList());
                    }

                    foreach (IoStream item in list)
                    {
                        if (!list2.Contains(item) && num == 0)
                        {
                            list2.Add(item);
                        }
                    }

                    foreach (IoStream item2 in list2)
                    {
                        if (!list.Contains(item2))
                        {
                            list3.Add(item2);
                        }
                    }

                    foreach (IoStream item3 in list3)
                    {
                        list2.Remove(item3);
                    }

                    num++;
                }
            }

            if (list2.Count() > 0)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            flag = false;
        }

        return flag;
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
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void RemoveLink(long controllerId, long streamId, long datatableItemId, long oldControllerId = 0L)
    {
        try
        {
            if ((from x in GetIoStreams()
                 where x.Id == streamId
                 select x).Count() <= 0 || !GetIoControllers().ContainsKey(controllerId))
            {
                return;
            }

            IoStream ioStream = (from x in GetIoStreams()
                                 where x.Id == streamId
                                 select x).FirstOrDefault();
            IoController ioController = GetIoControllers()[controllerId];
            if (ioStream != null && ioController != null && (from x in ioStream.GetIoStreamDataTableItems()
                                                             where x.Id == datatableItemId
                                                             select x).Count() > 0)
            {
                remoteMethods.RemoveLink(controllerId, streamId, datatableItemId, generalController.ModuleAccessController.UserLogin, oldControllerId);
                IoStreamDataTableItem ioStreamDataTableItem = (from x in ioStream.GetIoStreamDataTableItems()
                                                               where x.Id == datatableItemId
                                                               select x).FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void RemoveIoComponent(long componentId)
    {
        try
        {
            if (ioControllers.ContainsKey(componentId))
            {
                IoController ioController = ioControllers[componentId];
                List<LinkItem> linkWithStreams = ioController.GetLinkWithStreams();
                foreach (LinkItem link in linkWithStreams)
                {
                    foreach (IoStreamDataTableItem ioStreamDataTableItem in ioStreams.Where((IoStream x) => x.Id == link.IdStream).FirstOrDefault().GetIoStreamDataTableItems())
                    {
                        LinkItem linkItem = (from x in ioStreamDataTableItem.GetLinkedControllers()
                                             where x.IdController == componentId
                                             select x).FirstOrDefault();
                        if (linkItem != null)
                        {
                            ioStreamDataTableItem.GetLinkedControllers().Remove(linkItem);
                        }
                    }
                }

                ioControllers.Remove(componentId);
            }
            else if (ioStreams.Where((IoStream x) => x.Id == componentId).FirstOrDefault() != null)
            {
                IoStream ioStream = ioStreams.Where((IoStream x) => x.Id == componentId).FirstOrDefault();
                List<IoStreamDataTableItem> list = (from x in ioStream.GetIoStreamDataTableItems()
                                                    where x.GetLinkedControllers().Count > 0
                                                    select x).ToList();
                foreach (IoStreamDataTableItem item in list)
                {
                    List<LinkItem> list2 = item.GetLinkedControllers().ToList();
                    foreach (LinkItem item2 in list2)
                    {
                        if (ioControllers[item2.IdController].ComponentName.Contains(item.Name + "("))
                        {
                            ioControllers.Remove(item2.IdController);
                            remoteMethods.RemoveIoComponent(item2.IdController, generalController.ModuleAccessController.UserLogin);
                        }
                    }

                    foreach (LinkItem item3 in list2)
                    {
                        item.GetLinkedControllers().Remove(item3);
                    }
                }

                ioStreams.Remove(ioStream);
            }

            remoteMethods.RemoveIoComponent(componentId, generalController.ModuleAccessController.UserLogin);
            UnsavedChanges = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void AddController(string name, string shortTypeName, string componentName, string unit = "", int idTargetStream = -1, int idDataTableElement = -1, IODirectionEnum iODirection = IODirectionEnum.Output, string errorMsg = "", int priority = -1, int type = -1, int mainItemId = -1, string streamSimpleName = "", double minValue = 0.0, double maxValue = 0.0, double idController = -1.0, long scaleMin = 0L, long scaleMax = 0L)
    {
        try
        {
            AddControllerOutput addControllerOutput = remoteMethods.AddController(name, shortTypeName, unit, errorMsg, priority, type, mainItemId, streamSimpleName, minValue, maxValue, idController, componentName, scaleMin, scaleMax);
            if (addControllerOutput != null && addControllerOutput.NewItem != null)
            {
                if (addControllerOutput.NewItem.ShortTypeName == "SIGNAL" || addControllerOutput.NewItem.ShortTypeName == "ADSIGNAL")
                {
                    addControllerOutput.NewItem.ShortTypeName = "Signal";
                }

                RegisterController(addControllerOutput.NewItem);
            }

            if (idTargetStream != -1 && idDataTableElement != -1)
            {
                LinkControllerToStream(idTargetStream, Convert.ToInt32(addControllerOutput.NewItem.Identifier), idDataTableElement, iODirection);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    private void RegisterStream(StreamElement stream)
    {
        try
        {
            IoStream ioStream = new IoStream
            {
                Id = Convert.ToInt32(stream.Identifier),
                ComponentType = stream.ComponentType,
                ShortType = stream.ShortTypeName,
                IsArchive = stream.IsArchive
            };
            ioStream.GetProperties().AddRange(LoadProperties(ioStream.Id, isController: false));
            RepeatedField<StreamDataTableElement> dataTable = remoteMethods.GetStreamDataTable(Convert.ToInt32(stream.Identifier)).DataTable;
            foreach (StreamDataTableElement item in dataTable)
            {
                IoStreamDataTableItem ioStreamDataTableItem = new IoStreamDataTableItem
                {
                    Name = item.DataName,
                    Unit = item.Unit,
                    ErrorMsg = item.Errormsg,
                    Priority = (int)item.Priority,
                    Type = (int)item.Type,
                    Id = Convert.ToInt32(item.Id),
                    MinValue = Convert.ToDouble(item.Minvalue),
                    MaxValue = Convert.ToDouble(item.Maxvalue)
                };
                foreach (ElementProperty item2 in item.PropertiesAsIdentifier)
                {
                    ioStreamDataTableItem.GetIdentifierProperties().Add(new IoItemPropertyItem
                    {
                        PropertyName = item2.PropertyName,
                        PropertyValueInString = item2.ParsedToStringPropertyValue
                    });
                }

                if (ioControllers.Count > 0)
                {
                    foreach (LinkElement linkWithcontroller in item.LinkedControllers)
                    {
                        if (ioControllers.Where((KeyValuePair<long, IoController> x) => x.Key == linkWithcontroller.IdController).Count() != 1)
                        {
                            continue;
                        }

                        LinkItem newLink = new LinkItem
                        {
                            IdController = Convert.ToInt32(linkWithcontroller.IdController),
                            IODirection = (IODirectionEnum)Enum.Parse(typeof(IODirectionEnum), linkWithcontroller.IoDirection),
                            IdStream = Convert.ToInt32(stream.Identifier),
                            IdDataTableItem = Convert.ToInt32(item.Id)
                        };
                        if (ioControllers != null && ioControllers.Where((KeyValuePair<long, IoController> x) => x.Key == Convert.ToInt32(linkWithcontroller.IdController)).Count() > 0 && (from x in ioControllers.Where((KeyValuePair<long, IoController> x) => x.Key == Convert.ToInt32(linkWithcontroller.IdController)).First().Value.GetLinkWithStreams()
                                                                                                                                                                                            where x.IdStream == newLink.IdStream && x.IdDataTableItem == newLink.IdDataTableItem
                                                                                                                                                                                            select x).Count() < 1)
                        {
                            ioControllers.Where((KeyValuePair<long, IoController> x) => x.Key == Convert.ToInt32(linkWithcontroller.IdController)).First().Value.GetLinkWithStreams().Add(newLink);
                        }

                        if ((from x in ioStreamDataTableItem.GetLinkedControllers()
                             where x.IdController == newLink.IdController
                             select x).Count() < 1)
                        {
                            ioStreamDataTableItem.GetLinkedControllers().Add(newLink);
                        }
                    }
                }

                foreach (string authorizedControllerType in item.AuthorizedControllerTypes)
                {
                    ioStreamDataTableItem.GetauthorizedControllerTypes().Add(authorizedControllerType);
                }

                ioStream.GetIoStreamDataTableItems().Add(ioStreamDataTableItem);
            }

            ioStreams.Add(ioStream);
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public string EditController(IoController controller, string actionName)
    {
        string result = string.Empty;
        try
        {
            if (controller.SimpleNameNewValue == null)
            {
                controller.SimpleNameNewValue = controller.SimpleNameToDisplay;
            }

            EditControllerOutput editControllerOutput = remoteMethods.EditController(controller.Id, (from x in controller.GetProperties()
                                                                                                     where x.IsEditable
                                                                                                     select x).ToList(), (from x in controller.GetAlarms()
                                                                                                                          select x.Id).ToList(), actionName, generalController.ModuleAccessController.UserLogin, controller.SimpleNameNewValue);
            if (editControllerOutput != null && editControllerOutput.ErrorMessage != string.Empty)
            {
                result = editControllerOutput.ErrorMessage;
            }

            return result;
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            return result;
        }
    }

    public Dictionary<string, IoItemPropertyItem> GetElementProperties(int stremId)
    {
        Dictionary<string, IoItemPropertyItem> dictionary = new Dictionary<string, IoItemPropertyItem>();
        foreach (ElementProperty property in remoteMethods.GetElementProperties(stremId).Properties)
        {
            IoItemPropertyItem ioItemPropertyItem = new IoItemPropertyItem
            {
                PropertyName = property.PropertyName,
                PropertyValueInString = property.ParsedToStringPropertyValue,
                IsEditable = property.IsEditable,
                DisplayText = property.DisplayText,
                PropertyType = Type.GetType(property.PropertyTypeInString)
            };
            ioItemPropertyItem.AuthorizedValues.AddRange(property.AuthorizedValuesInString);
            dictionary.Add(ioItemPropertyItem.PropertyName, ioItemPropertyItem);
        }

        return dictionary;
    }

    public void LoadEquipment()
    {
        availableEquipment.Clear();
        foreach (EquipmentElement item in remoteMethods.LoadEquipment().Equipments)
        {
            EquipmentItem item2 = new EquipmentItem
            {
                Id = item.Id,
                Name = item.Name,
                Type = (Orthodyne.CoreCommunicationLayer.Models.IndCom.IndCommTypeEnum)item.Type,
                Items = item.Items.Select((ItemElement x) => new Item
                {
                    Adress = double.Parse(x.Adress),
                    Name = x.Name,
                    Id = x.Id,
                    IdEqu = item.Id,
                    VarTypeCom = (VarTypeComEnum)x.VarTypeCom,
                    VarTypeOrtho = (Orthodyne.CoreCommunicationLayer.Models.IndCom.IndCommTypeComponentEnum)x.VarTypeOrtho
                }).ToList(),
                Params = item.Params.Select((ParamElement x) => new ParamItem
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    IdEqu = item.Id
                }).ToList()
            };
            availableEquipment.Add(item2);
        }
    }

    public bool CheckIfDllExists(string name)
    {
        return remoteMethods.CheckIfDllExists(name).Exists;
    }

    public List<string> GetAvailableDllStream()
    {
        List<string> list = new List<string>();
        foreach (DllStreamElement item in remoteMethods.GetAvailableDllStreamList().DllStreams.ToList())
        {
            list.Add(item.Name);
        }

        return list;
    }

    public void EditStream(IoStream stream, string actionName)
    {
        try
        {
            EditStreamOutput editStreamOutput = remoteMethods.EditStream(stream.Id, (from x in stream.GetProperties()
                                                                                     where x.IsEditable
                                                                                     select x).ToList(), "Confirm", generalController.ModuleAccessController.UserLogin);
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public List<StreamMetadataItem> SearchAvailableStreams(bool StmLine, bool Beck)
    {
        List<StreamMetadataItem> list = new List<StreamMetadataItem>();
        try
        {
            foreach (StreamMetadataElement foundedStream in remoteMethods.SearchAvailableStreams(StmLine, Beck).FoundedStreams)
            {
                StreamMetadataItem streamMetadataItem = new StreamMetadataItem
                {
                    StreamShortType = foundedStream.StreamShortTypeName
                };
                foreach (PropertyElement property in foundedStream.Properties)
                {
                    streamMetadataItem.MetadataProperties.Add(new StreamMetadataPropertyItem
                    {
                        PropertyName = property.PropertyName,
                        PropertyTypeInString = property.PropertyTypeInString,
                        PropertyValueInString = property.PropertyValueInString,
                        LabelForUi = property.LabelForUi
                    });
                }

                list.Add(streamMetadataItem);
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }

        return list;
    }

    public List<StreamMetadataItem> SearchAvailableStreamsIcp()
    {
        List<StreamMetadataItem> list = new List<StreamMetadataItem>();
        try
        {
            foreach (StreamMetadataElement foundedStream in remoteMethods.SearchAvailableStreamsIcp().FoundedStreams)
            {
                StreamMetadataItem streamMetadataItem = new StreamMetadataItem
                {
                    StreamShortType = foundedStream.StreamShortTypeName
                };
                foreach (PropertyElement property in foundedStream.Properties)
                {
                    streamMetadataItem.MetadataProperties.Add(new StreamMetadataPropertyItem
                    {
                        PropertyName = property.PropertyName,
                        PropertyTypeInString = property.PropertyTypeInString,
                        PropertyValueInString = property.PropertyValueInString,
                        LabelForUi = property.LabelForUi
                    });
                }

                list.Add(streamMetadataItem);
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }

        return list;
    }

    public List<StreamMetadataItem> SearchAvailableIndCommunication()
    {
        List<StreamMetadataItem> list = new List<StreamMetadataItem>();
        try
        {
            foreach (StreamMetadataElement foundedStream in remoteMethods.SearchAvailableIndCommunication().FoundedStreams)
            {
                StreamMetadataItem streamMetadataItem = new StreamMetadataItem
                {
                    StreamShortType = foundedStream.StreamShortTypeName
                };
                foreach (PropertyElement property in foundedStream.Properties)
                {
                    streamMetadataItem.MetadataProperties.Add(new StreamMetadataPropertyItem
                    {
                        PropertyName = property.PropertyName,
                        PropertyTypeInString = property.PropertyTypeInString,
                        PropertyValueInString = property.PropertyValueInString,
                        LabelForUi = property.LabelForUi
                    });
                }

                list.Add(streamMetadataItem);
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }

        return list;
    }

    public void RequestAlarmCreation(string alarmType, IoController controllerTarget)
    {
        try
        {
            if (controllerTarget != null)
            {
                RequestAlarmCreationOutput requestAlarmCreationOutput = remoteMethods.RequestAlarmCreation(alarmType, controllerTarget.Id, generalController.ModuleAccessController.UserLogin);
                generalController.ModuleAlarmController.LoadAlarms();
                controllerTarget.GetAlarms().Add(generalController.ModuleAlarmController.Alarms[requestAlarmCreationOutput.AlarmId]);
            }
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void AlarmAck()
    {
        try
        {
            AlarmAckOutput alarmAckOutput = remoteMethods.AlarmAck();
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public int GetHighAlarms()
    {
        int result = 0;
        try
        {
            GetHighAlarmsOutput highAlarms = remoteMethods.GetHighAlarms();
            result = (int)highAlarms.Level;
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }

        return result;
    }

    public void LoadStatusIoStreamItem()
    {
        try
        {
            statusiostreams.Clear();
            GetStatusIoStreamListOutput statusIoStreamList = remoteMethods.GetStatusIoStreamList();
            foreach (StatusIoStreamElement item2 in statusIoStreamList.Statusiostream)
            {
                StatusIoStream item = new StatusIoStream
                {
                    IoStreamId = item2.IoStreamId,
                    IsStatus = item2.IsStatus,
                    SimpleNameToDisplay = item2.SimpleNameToDisplay
                };
                statusiostreams.Add(item);
            }

            StatusIoStream statusIoStream1 = statusiostreams.Where((StatusIoStream x) => x.IoStreamId == -1).FirstOrDefault();
            statusiostreams = (from x in statusiostreams
                               orderby (x != statusIoStream1) ? 1 : 0, x.SimpleNameToDisplay
                               select x).ToList();
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }
}
#if false // Journal de décompilation
'193' éléments dans le cache
------------------
Résoudre : 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\mscorlib.dll'
------------------
Résoudre : 'Grpc.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d754f35622e28bad'
Un seul assembly trouvé : 'Grpc.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d754f35622e28bad'
Charger à partir de : 'C:\Users\Alial\.nuget\packages\grpc.core\1.18.0\lib\netstandard1.5\Grpc.Core.dll'
------------------
Résoudre : 'AccessModuleInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'AccessModuleInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'AlarmManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'AlarmManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'Orthodyne.Common.Configuration, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Un seul assembly trouvé : 'Orthodyne.Common.Configuration, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Charger à partir de : 'Z:\ModuleGeneralConfiguration\GeneralconfigurationManager\Orthodyne.Common.Configuration.dll'
------------------
Résoudre : 'AnalysisModuleInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'AnalysisModuleInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'AuditTrailManagerInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'AuditTrailManagerInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'GeneralConfigurationManagerInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Un seul assembly trouvé : 'GeneralConfigurationManagerInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Charger à partir de : 'C:\Users\Alial\Desktop\MODE DES BITS\Orthodyne.CoreCommunicationLayer\Orthodyne.CoreCommunicationLayer\GeneralConfigurationManagerInterface.dll'
------------------
Résoudre : 'HistoryManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'HistoryManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'IOManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Un seul assembly trouvé : 'IOManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Charger à partir de : 'Z:\ModuleGeneralConfiguration\GeneralconfigurationManager\IOManagerInterface.dll'
------------------
Résoudre : 'JobInterfaces, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'JobInterfaces, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'RequestManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'RequestManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'SchedulerManagerInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'SchedulerManagerInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'TranslationModuleInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
Introuvable par le nom : 'TranslationModuleInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
------------------
Résoudre : 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.dll'
------------------
Résoudre : 'System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System.Windows.Forms, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
AVERTISSEMENT : Incompatibilité de version. Attendu : '4.0.0.0'. Reçu : '6.0.2.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Windows.Forms.dll'
------------------
Résoudre : 'Google.Protobuf, Version=3.6.1.0, Culture=neutral, PublicKeyToken=a7d26565bac4d604'
Un seul assembly trouvé : 'Google.Protobuf, Version=3.6.1.0, Culture=neutral, PublicKeyToken=a7d26565bac4d604'
Charger à partir de : 'C:\Users\Alial\.nuget\packages\google.protobuf\3.6.1\lib\netstandard1.0\Google.Protobuf.dll'
------------------
Résoudre : 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Core.dll'
------------------
Résoudre : 'CodeExceptionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Un seul assembly trouvé : 'CodeExceptionManager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
Charger à partir de : 'C:\Users\Alial\Downloads\CodeExceptionManager.dll'
------------------
Résoudre : 'System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Drawing, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '4.0.0.0'. Reçu : '6.0.2.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Drawing.dll'
------------------
Résoudre : 'Microsoft.Win32.Registry, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'Microsoft.Win32.Registry, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\Microsoft.Win32.Registry.dll'
------------------
Résoudre : 'System.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Runtime.dll'
------------------
Résoudre : 'System.Security.Principal.Windows, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Principal.Windows, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Principal.Windows.dll'
------------------
Résoudre : 'System.Security.Permissions, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Security.Permissions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Security.Permissions.dll'
------------------
Résoudre : 'System.Collections, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Collections, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Collections.dll'
------------------
Résoudre : 'System.Collections.NonGeneric, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Collections.NonGeneric, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Collections.NonGeneric.dll'
------------------
Résoudre : 'System.Collections.Concurrent, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Collections.Concurrent, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Collections.Concurrent.dll'
------------------
Résoudre : 'System.ObjectModel, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ObjectModel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ObjectModel.dll'
------------------
Résoudre : 'System.Console, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Console, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Console.dll'
------------------
Résoudre : 'System.Runtime.InteropServices, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Runtime.InteropServices, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Runtime.InteropServices.dll'
------------------
Résoudre : 'System.Diagnostics.Contracts, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Diagnostics.Contracts, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.Contracts.dll'
------------------
Résoudre : 'System.Diagnostics.StackTrace, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Diagnostics.StackTrace, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.StackTrace.dll'
------------------
Résoudre : 'System.Diagnostics.Tracing, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Diagnostics.Tracing, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.Tracing.dll'
------------------
Résoudre : 'System.IO.FileSystem.DriveInfo, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.IO.FileSystem.DriveInfo, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.FileSystem.DriveInfo.dll'
------------------
Résoudre : 'System.IO.IsolatedStorage, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.IO.IsolatedStorage, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.IsolatedStorage.dll'
------------------
Résoudre : 'System.ComponentModel, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ComponentModel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ComponentModel.dll'
------------------
Résoudre : 'System.Threading.Thread, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Threading.Thread, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Threading.Thread.dll'
------------------
Résoudre : 'System.Reflection.Emit, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Reflection.Emit, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Reflection.Emit.dll'
------------------
Résoudre : 'System.Reflection.Emit.ILGeneration, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Reflection.Emit.ILGeneration, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Reflection.Emit.ILGeneration.dll'
------------------
Résoudre : 'System.Reflection.Emit.Lightweight, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Reflection.Emit.Lightweight, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Reflection.Emit.Lightweight.dll'
------------------
Résoudre : 'System.Reflection.Primitives, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Reflection.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Reflection.Primitives.dll'
------------------
Résoudre : 'System.Resources.Writer, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Resources.Writer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Resources.Writer.dll'
------------------
Résoudre : 'System.Runtime.CompilerServices.VisualC, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Runtime.CompilerServices.VisualC, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Runtime.CompilerServices.VisualC.dll'
------------------
Résoudre : 'System.Runtime.InteropServices.RuntimeInformation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Runtime.InteropServices.RuntimeInformation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Runtime.InteropServices.RuntimeInformation.dll'
------------------
Résoudre : 'System.Runtime.Serialization.Formatters, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Runtime.Serialization.Formatters, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Runtime.Serialization.Formatters.dll'
------------------
Résoudre : 'System.Security.AccessControl, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.AccessControl, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.AccessControl.dll'
------------------
Résoudre : 'System.IO.FileSystem.AccessControl, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.IO.FileSystem.AccessControl, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.FileSystem.AccessControl.dll'
------------------
Résoudre : 'System.Threading.AccessControl, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Threading.AccessControl, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Threading.AccessControl.dll'
------------------
Résoudre : 'System.Security.Claims, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Claims, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Claims.dll'
------------------
Résoudre : 'System.Security.Cryptography.Algorithms, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Cryptography.Algorithms, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Cryptography.Algorithms.dll'
------------------
Résoudre : 'System.Security.Cryptography.Primitives, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Cryptography.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Cryptography.Primitives.dll'
------------------
Résoudre : 'System.Security.Cryptography.Csp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Cryptography.Csp, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Cryptography.Csp.dll'
------------------
Résoudre : 'System.Security.Cryptography.Encoding, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Cryptography.Encoding, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Cryptography.Encoding.dll'
------------------
Résoudre : 'System.Security.Cryptography.X509Certificates, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Cryptography.X509Certificates, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Cryptography.X509Certificates.dll'
------------------
Résoudre : 'System.Text.Encoding.Extensions, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Text.Encoding.Extensions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Text.Encoding.Extensions.dll'
------------------
Résoudre : 'System.Threading, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Threading, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Threading.dll'
------------------
Résoudre : 'System.Threading.Overlapped, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Threading.Overlapped, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Threading.Overlapped.dll'
------------------
Résoudre : 'System.Threading.ThreadPool, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Threading.ThreadPool, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Threading.ThreadPool.dll'
------------------
Résoudre : 'System.Threading.Tasks.Parallel, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Threading.Tasks.Parallel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Threading.Tasks.Parallel.dll'
------------------
Résoudre : 'System.CodeDom, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.CodeDom, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.CodeDom.dll'
------------------
Résoudre : 'Microsoft.Win32.SystemEvents, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'Microsoft.Win32.SystemEvents, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\Microsoft.Win32.SystemEvents.dll'
------------------
Résoudre : 'System.Diagnostics.Process, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Diagnostics.Process, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.Process.dll'
------------------
Résoudre : 'System.Collections.Specialized, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Collections.Specialized, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Collections.Specialized.dll'
------------------
Résoudre : 'System.ComponentModel.TypeConverter, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ComponentModel.TypeConverter, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ComponentModel.TypeConverter.dll'
------------------
Résoudre : 'System.ComponentModel.EventBasedAsync, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ComponentModel.EventBasedAsync, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ComponentModel.EventBasedAsync.dll'
------------------
Résoudre : 'System.ComponentModel.Primitives, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ComponentModel.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ComponentModel.Primitives.dll'
------------------
Résoudre : 'Microsoft.Win32.Primitives, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'Microsoft.Win32.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\Microsoft.Win32.Primitives.dll'
------------------
Résoudre : 'System.Configuration.ConfigurationManager, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Configuration.ConfigurationManager, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Configuration.ConfigurationManager.dll'
------------------
Résoudre : 'System.Diagnostics.TraceSource, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Diagnostics.TraceSource, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.TraceSource.dll'
------------------
Résoudre : 'System.Diagnostics.TextWriterTraceListener, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Diagnostics.TextWriterTraceListener, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.TextWriterTraceListener.dll'
------------------
Résoudre : 'System.Diagnostics.PerformanceCounter, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Diagnostics.PerformanceCounter, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.PerformanceCounter.dll'
------------------
Résoudre : 'System.Diagnostics.EventLog, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Diagnostics.EventLog, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.EventLog.dll'
------------------
Résoudre : 'System.Diagnostics.FileVersionInfo, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Diagnostics.FileVersionInfo, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Diagnostics.FileVersionInfo.dll'
------------------
Résoudre : 'System.IO.Compression, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System.IO.Compression, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.Compression.dll'
------------------
Résoudre : 'System.IO.FileSystem.Watcher, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.IO.FileSystem.Watcher, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.FileSystem.Watcher.dll'
------------------
Résoudre : 'System.IO.Ports, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Introuvable par le nom : 'System.IO.Ports, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
------------------
Résoudre : 'System.Windows.Extensions, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Windows.Extensions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Windows.Extensions.dll'
------------------
Résoudre : 'System.Net.Requests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.Requests, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.Requests.dll'
------------------
Résoudre : 'System.Net.Primitives, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.Primitives.dll'
------------------
Résoudre : 'System.Net.HttpListener, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Net.HttpListener, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.HttpListener.dll'
------------------
Résoudre : 'System.Net.ServicePoint, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Net.ServicePoint, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.ServicePoint.dll'
------------------
Résoudre : 'System.Net.NameResolution, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.NameResolution, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.NameResolution.dll'
------------------
Résoudre : 'System.Net.WebClient, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Net.WebClient, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.WebClient.dll'
------------------
Résoudre : 'System.Net.WebHeaderCollection, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.WebHeaderCollection, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.WebHeaderCollection.dll'
------------------
Résoudre : 'System.Net.WebProxy, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Net.WebProxy, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.WebProxy.dll'
------------------
Résoudre : 'System.Net.Mail, Version=0.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Net.Mail, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.Mail.dll'
------------------
Résoudre : 'System.Net.NetworkInformation, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.NetworkInformation, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.NetworkInformation.dll'
------------------
Résoudre : 'System.Net.Ping, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.Ping, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.Ping.dll'
------------------
Résoudre : 'System.Net.Security, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.Security, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.Security.dll'
------------------
Résoudre : 'System.Net.Sockets, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.Sockets, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.Sockets.dll'
------------------
Résoudre : 'System.Net.WebSockets.Client, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.WebSockets.Client, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.WebSockets.Client.dll'
------------------
Résoudre : 'System.Net.WebSockets, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.WebSockets, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.WebSockets.dll'
------------------
Résoudre : 'System.Text.RegularExpressions, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Text.RegularExpressions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Text.RegularExpressions.dll'
------------------
Résoudre : 'System.Windows.Forms.Primitives, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System.Windows.Forms.Primitives, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Windows.Forms.Primitives.dll'
------------------
Résoudre : 'System.IO.MemoryMappedFiles, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.IO.MemoryMappedFiles, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.MemoryMappedFiles.dll'
------------------
Résoudre : 'System.Security.Cryptography.Cng, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.Cryptography.Cng, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.Cryptography.Cng.dll'
------------------
Résoudre : 'System.IO.Pipes, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.IO.Pipes, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.Pipes.dll'
------------------
Résoudre : 'System.Linq.Expressions, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Linq.Expressions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Linq.Expressions.dll'
------------------
Résoudre : 'System.IO.Pipes.AccessControl, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.IO.Pipes.AccessControl, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.IO.Pipes.AccessControl.dll'
------------------
Résoudre : 'System.Linq, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Linq, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Linq.dll'
------------------
Résoudre : 'System.Linq.Queryable, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Linq.Queryable, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Linq.Queryable.dll'
------------------
Résoudre : 'System.Linq.Parallel, Version=0.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Linq.Parallel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
AVERTISSEMENT : Incompatibilité de version. Attendu : '0.0.0.0'. Reçu : '6.0.0.0'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Linq.Parallel.dll'
------------------
Résoudre : 'System.Drawing.Common, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Drawing.Common, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Drawing.Common.dll'
------------------
Résoudre : 'System.Drawing.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Drawing.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Drawing.Primitives.dll'
------------------
Résoudre : 'System.ComponentModel.TypeConverter, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ComponentModel.TypeConverter, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ComponentModel.TypeConverter.dll'
------------------
Résoudre : 'System.Configuration.ConfigurationManager, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Configuration.ConfigurationManager, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Configuration.ConfigurationManager.dll'
------------------
Résoudre : 'System.Windows.Forms, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System.Windows.Forms, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Windows.Forms.dll'
------------------
Résoudre : 'System.Windows.Forms.Design, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Un seul assembly trouvé : 'System.Windows.Forms.Design, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Windows.Forms.Design.dll'
------------------
Résoudre : 'System.Security.Permissions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Un seul assembly trouvé : 'System.Security.Permissions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.WindowsDesktop.App.Ref\6.0.36\ref\net6.0\System.Security.Permissions.dll'
------------------
Résoudre : 'System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Runtime.dll'
------------------
Résoudre : 'System.Security.AccessControl, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Security.AccessControl, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Security.AccessControl.dll'
------------------
Résoudre : 'System.ComponentModel.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ComponentModel.Primitives, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ComponentModel.Primitives.dll'
------------------
Résoudre : 'System.ObjectModel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.ObjectModel, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.ObjectModel.dll'
------------------
Résoudre : 'System.Net.WebHeaderCollection, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Un seul assembly trouvé : 'System.Net.WebHeaderCollection, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
Charger à partir de : 'C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.36\ref\net6.0\System.Net.WebHeaderCollection.dll'
#endif
