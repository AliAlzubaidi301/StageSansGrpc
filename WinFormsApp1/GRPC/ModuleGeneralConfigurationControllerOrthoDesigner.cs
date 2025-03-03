#region assembly Orthodyne.CoreCommunicationLayer, Version=0.1.0.1, Culture=neutral, PublicKeyToken=null
// Z:\CommunicationLayer\Orthodyne.CoreCommunicationLayer.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeExceptionManager.Model.Objects;
using IGeneralConfigurationManager;
using Orthodyne.CoreCommunicationLayer.Models.GeneralConfiguration;
using Orthodyne.CoreCommunicationLayer.Services;

namespace Orthodyne.CoreCommunicationLayer.Controllers;

public class ModuleGeneralConfigurationControllerOrthoDesigner
{
    private ModuleGeneralConfigurationRevocationService remoteMethods;

    private GeneralController generalController;

    public ModuleGeneralConfigurationControllerOrthoDesigner(ModuleGeneralConfigurationRevocationService remoteMethods, GeneralController generalController)
    {
        try
        {
            this.generalController = generalController;
            this.remoteMethods = remoteMethods;
        }
        catch (Exception ex)
        {
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void DeleteFlag(FlagItem toDelete, string userName)
    {
        try
        {
            DeleteFlagsElementInput toDelete2 = new DeleteFlagsElementInput
            {
                ComputerName = Environment.MachineName,
                Flag = new FlagElement
                {
                    Id = toDelete.Id,
                    Name = toDelete.Name
                },
                UserName = userName
            };
            DeleteFlagsElementOutput deleteFlagsElementOutput = remoteMethods.DeleteFlag(toDelete2);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void EditFlag(FlagItem toUpdate, string userName)
    {
        try
        {
            EditFlagsElementInput input = new EditFlagsElementInput
            {
                ComputerName = Environment.MachineName,
                Flag = new FlagElement
                {
                    Id = toUpdate.Id,
                    Name = toUpdate.Name
                },
                UserName = userName
            };
            EditFlagsElementOutput editFlagsElementOutput = remoteMethods.EditFlags(input);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public List<FlagItem> LoadFlags()
    {
        List<FlagItem> list = new List<FlagItem>();
        try
        {
            LoadFlagsElementInput input = new LoadFlagsElementInput();
            foreach (FlagElement flag in remoteMethods.LoadFlags(input).Flags)
            {
                FlagItem item = new FlagItem
                {
                    Id = flag.Id,
                    Name = flag.Name
                };
                list.Add(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            return null;
        }

        return list;
    }

    public List<ParameterItem> LoadConfigurationElements()
    {
        List<ParameterItem> list = new List<ParameterItem>();
        try
        {
            LoadConfigurationElementsInput input = new LoadConfigurationElementsInput();
            foreach (ConfigurationElement element in remoteMethods.LoadConfigurationElements(input).Elements)
            {
                ParameterItem item = new ParameterItem
                {
                    Comment = element.Comment,
                    Id = element.Id,
                    Name = element.Name,
                    Value = element.Value
                };
                list.Add(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            return null;
        }

        return list;
    }

    public ParameterItem LoadConfigurationElementByName(string name)
    {
        ParameterItem parameterItem = new ParameterItem();
        try
        {
            LoadConfigurationElementByNameInput input = new LoadConfigurationElementByNameInput
            {
                Name = name
            };
            ConfigurationElement element = remoteMethods.LoadConfigurationElementByName(input).Element;
            parameterItem = new ParameterItem
            {
                Comment = element.Comment,
                Id = element.Id,
                Name = element.Name,
                Value = element.Value
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            return null;
        }

        return parameterItem;
    }

    public void EditGeneralConfiguration(long id, string name, string value, string comment)
    {
        try
        {
            EditConfigurationElementInput input = new EditConfigurationElementInput
            {
                ToUpdate = new ConfigurationElement
                {
                    Comment = comment,
                    Id = id,
                    Name = name,
                    Value = value
                },
                UserName = generalController.ModuleAccessController.UserLogin,
                ComputerName = Environment.MachineName
            };
            remoteMethods.EditGeneralConfiguration(input);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        }
    }

    public void CreateGeneralConfiguration(long id, string name, string value, string comment)
    {
        try
        {
            CreateConfigurationElementInput createConfigurationElementInput = new CreateConfigurationElementInput();
            ConfigurationElement configurationElement = new ConfigurationElement();
            configurationElement.Name = name;
            configurationElement.Value = value;
            configurationElement.Comment = comment;
            createConfigurationElementInput.Parameter = configurationElement;
            createConfigurationElementInput.UserName = generalController.ModuleAccessController.UserLogin;
            createConfigurationElementInput.ComputerName = Environment.MachineName;
            remoteMethods.CreateConfigurationElement(createConfigurationElementInput);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
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
Introuvable par le nom : 'IOManagerInterface, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null'
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
