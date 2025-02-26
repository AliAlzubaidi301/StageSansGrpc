using CodeExceptionManager.Model.Objects;
using Orthodyne.CoreCommunicationLayer.Models.GeneralConfiguration;
using Orthodyne.CoreCommunicationLayer.Models.IO;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
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
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //public void DeleteFlag(FlagItem toDelete, string userName)
        //{
        //    try
        //    {
        //        DeleteFlagsElementInput input = new DeleteFlagsElementInput
        //        {
        //            ComputerName = Environment.MachineName,
        //            Flag = new FlagElement
        //            {
        //                Id = toDelete.Id,
        //                Name = toDelete.Name,
        //            },
        //            UserName = userName,
        //        };
        //        DeleteFlagsElementOutput output = remoteMethods.DeleteFlag(input);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine(ex.StackTrace);
        //        new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }
        //}

        //public void EditFlag(FlagItem toUpdate, string userName)
        //{
        //    try
        //    {
        //        EditFlagsElementInput input = new EditFlagsElementInput()
        //        {
        //            ComputerName = Environment.MachineName,
        //            Flag = new FlagElement
        //            {
        //                Id = toUpdate.Id,
        //                Name = toUpdate.Name,
        //            },
        //            UserName = userName,
        //        };
        //        EditFlagsElementOutput output = remoteMethods.EditFlags(input);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine(ex.StackTrace);
        //        new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }
        //}

        //public List<FlagItem> LoadFlags()
        //{
        //    List<FlagItem> toReturn = new List<FlagItem>();
        //    try
        //    {
        //        LoadFlagsElementInput input = new LoadFlagsElementInput();
        //        foreach (FlagElement flag in remoteMethods.LoadFlags(input).Flags)
        //        {
        //            FlagItem tmp = new FlagItem
        //            {
        //                Id = flag.Id,
        //                Name = flag.Name,
        //            };
        //            toReturn.Add(tmp);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine(ex.StackTrace);
        //        new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //        return null;
        //    }
        //    return toReturn;
        //}
    
    }
}
