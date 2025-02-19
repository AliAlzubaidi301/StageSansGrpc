using CodeExceptionManager.Model.Objects;
using Orthodyne.CoreCommunicationLayer.Models.Analysis;
using Orthodyne.CoreCommunicationLayer.Models.Job;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{

    public class ModuleJobController
    {
        //Todo messages
        private Dictionary<string, Models.Job.JobItem> jobs = new Dictionary<string, Models.Job.JobItem>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, Models.Job.VariableItem> variables = new Dictionary<string, Models.Job.VariableItem>();
        private Dictionary<string, Models.Job.JobItem> runningJobs = new Dictionary<string, Models.Job.JobItem>();
        private List<Models.Job.JobItem> jobToRemoveFromRunningJobs = new List<Models.Job.JobItem>();
        //private MainWindow main;
        private bool AnalysisHasRun = false;
        private GeneralController generalController;
        public const string ANALYSIS_JOB_TASK_TYPE_NAME = "Analysis";
        public const string ANALYSIS_WAIT_TASK_TYPE_NAME = "Wait";
        public const string ANALYSIS_DIALOG_TASK_TYPE_NAME = "Dialog";
        private long previousSequenceId = -1;

        private ModuleJobRemoteMethodInvocationService remoteMethods;
        public Models.Job.JobItem RunningJob { get; set; }
        public Dictionary<string, Models.Job.JobItem> RunningJobs { get { return this.runningJobs; } }

        private Dictionary<string, Models.Job.LangageTokenItem> langageTokenItems = new Dictionary<string, Models.Job.LangageTokenItem>();

        public string OrthoViewPageToShow = string.Empty;

        internal ModuleJobController(ModuleJobRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {
            try
            {
                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
                RunningJob = null;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }


        public void LoadElements()
        {
            try
            {
                variables.Clear();
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
    }
}