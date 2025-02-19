using CodeExceptionManager.Model.Objects;
using Orthodyne.CoreCommunicationLayer.Models.Scheduler;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleSchedulerController
    {
        private GeneralController generalController;
        private ModuleSchedulerRemoteMethodInvocationService remoteMethods;
        private Dictionary<long, TaskItem> definedTaskInCache = new Dictionary<long, TaskItem>();
        private Dictionary<long, CallStackItem> definedCallstackInCache = new Dictionary<long, CallStackItem>();
        internal ModuleSchedulerController(ModuleSchedulerRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {
            this.generalController = generalController;
            this.remoteMethods = remoteMethods;
            LoadCallStack();
            LoadTasks();
        }

        public Dictionary<long, TaskItem> DefinedTaskInCache
        {
            get
            {
                return definedTaskInCache;
            }
        }
        public Dictionary<long, CallStackItem> DefinedCallStackInCache
        {
            get
            {
                return definedCallstackInCache;
            }
        }

        public void DeleteCallStack(List<CallStackItem> callbacks)
        {

        }

        public void DeleteTasks(List<TaskItem> tasks)
        {
            try
            {
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void LoadTasks()
        {
        }

        public void LoadCallStack()
        {
            try
            {
                definedCallstackInCache.Clear();
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditTask(List<TaskItem> tasksToSave)
        {
            try
            {
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}
