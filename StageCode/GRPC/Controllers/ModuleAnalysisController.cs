using CodeExceptionManager.Model.Objects;
//using Google.Protobuf.WellKnownTypes;
using Orthodyne.CoreCommunicationLayer.Converters;
using Orthodyne.CoreCommunicationLayer.Models.Analysis;
using Orthodyne.CoreCommunicationLayer.Models.Gas;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleAnalysisController
    {
        private GeneralController generalController;
        private ModuleAnalysisRemoteMethodInvocationService remoteMethods;
        private List<TimeEventTemplateItem> timeEventTemplates = new List<TimeEventTemplateItem>();
        private Dictionary<string, FormulaItem> existingFormulas = new Dictionary<string, FormulaItem>();
        private Dictionary<long, SpecificDataItem> specificDataInCache = new Dictionary<long, SpecificDataItem>();
        private Dictionary<long, AnalysisResultItem> currentlyInCacheRunningAnalysis = new Dictionary<long, AnalysisResultItem>();
        private Dictionary<long, AnalysisResultItem> currentlyInCacheResults = new Dictionary<long, AnalysisResultItem>();
        private Dictionary<long, SequenceItem> currentVersionOfSequencesInCache = new Dictionary<long, SequenceItem>();
        private Dictionary<long, ImpurityItem> currentVersionOfImpuritiesInCache = new Dictionary<long, ImpurityItem>();
        private List<GasItem> gasList = new List<GasItem>();
        private List<ImpurityUnitItem> impurityUnitList = new List<ImpurityUnitItem>();


        public List<TimeEventTemplateItem> TimeEventTemplates { get { return this.timeEventTemplates; } }
        public Dictionary<string, FormulaItem> ExistingFormulas { get { return existingFormulas; } }
        public Dictionary<long, SpecificDataItem> SpecificDataInCache { get { return this.specificDataInCache; } }
        public Dictionary<long, AnalysisResultItem> CurrentlyInCacheRunningAnalysis { get { return this.currentlyInCacheRunningAnalysis; } }
        public Dictionary<long, AnalysisResultItem> CurrentlyInCacheResults { get { return this.currentlyInCacheResults; } }
        public Dictionary<long, SequenceItem> CurrentVersionOfSequencesInCache
        {
            get
            {
                return this.currentVersionOfSequencesInCache;
            }
        }
        public Dictionary<long, ImpurityItem> CurrentVersionOfImpuritiesInCache { get { return this.currentVersionOfImpuritiesInCache; } }
        public List<GasItem> GasList { get { return gasList; } }
        public List<ImpurityUnitItem> ImpurityUnitList { get { return impurityUnitList; } }


        /*
         Maybe will be removed, for the moment we don't have the mechanic to get the running jobs / analysis 
         on the system, one we will, we will need to filter which can be shown on the visualiser
         but for the moment there will only be one analysis running for a synoptic, later, this object will 
         be removed and we will listen on the Analysis in cache dictionary
         */
        public AnalysisResultItem CurrentlyRunningAnalysis { get; set; }
        /*
        Will be removed, just keeping it for the compatibility with OrthoView (08/02/2022) to save time,
        but lately, we will get it from the Currently In Chache result dictionary 
         */
        public AnalysisResultItem CurrentlyLoadedAnalysis { get; set; }

        public bool UnsavedChangesPresent { get; set; }

        internal ModuleAnalysisController(ModuleAnalysisRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {
            try
            {
                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
                UnsavedChangesPresent = false;
                //this.definedFormulas = LoadFormulas();
                /*
                RefreshCurrentImpurities();
                RefreshCurrentSequences();*/
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void UndoChanges()
        {
            try
            {
                SpecificDataInCache.Clear();
                UnsavedChangesPresent = false;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Load the defined sequence from the core's cache to the current API's cache
        /// </summary>

    }
}
