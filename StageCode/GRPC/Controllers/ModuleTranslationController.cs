using CodeExceptionManager.Model.Objects;
using Orthodyne.CoreCommunicationLayer.Models.Translation;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleTranslationController
    {
        public long DEFAULT_LANGAGE_ID = 1;
        private ModuleTranslationRemoteMethodInvocationService remoteMethods;
        private GeneralController generalController;
        private Dictionary<string, Dictionary<long, string>> translatedTexts = new Dictionary<string, Dictionary<long, string>>();
        private List<TranslatedLangage> availableLangages = new List<TranslatedLangage>();

        public Dictionary<string, Dictionary<long, string>> TranslatedTexts { get { return this.translatedTexts; } }
        public List<TranslatedLangage> AvailableLangages { get { return availableLangages; } }
        public TranslatedLangage SelectedLangage { get; set; }

        internal ModuleTranslationController(ModuleTranslationRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {
            try
            {
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();

                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
                LoadAvailableLangages();
                ChangeLangage(DEFAULT_LANGAGE_ID);

                //stopwatch.Stop();
                //MessageBox.Show("Chargement langue :" + stopwatch.Elapsed.ToString());
                //stopwatch.Reset();

            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void LoadAvailableLangages()
        {
            try
            {
                this.availableLangages.Clear();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        //public void LoadTranslatedTexts()
        //{
        //    try
        //    {
        //        RequestTranslationOutput response = remoteMethods.RequestTranslation(availableLangages.Where(x => x.IsUsed).Select(x => x.Id).ToList(), translatedTexts.Keys.ToList());
        //        foreach (TranslatedTextElement text in response.Texts)
        //        {
        //            this.translatedTexts.Add(originalText, new Dictionary<long, string>());
        //            this.translatedTexts[text.OriginalText][text.IdLangage] = text.Text;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
        //    }
        //}

        public void ChangeLangage(long id)
        {
            try
            {
                SelectedLangage = availableLangages.Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void ChangeLangage(string label)
        {
            try
            {
                SelectedLangage = availableLangages.Where(x => x.Label == label).FirstOrDefault();
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        /*
        public string GetTranslation(string originalText)
        {
            try
            {
                string returnValue = originalText;
                if (originalText != null && !this.translatedTexts.ContainsKey(originalText))
                {
                    this.translatedTexts.Add(originalText, new Dictionary<long, string>());
                    RequestTranslation();
                }
                if (this.SelectedLangage != null && this.SelectedLangage.Id != DEFAULT_LANGAGE_ID)
                {
                    if (!this.translatedTexts[originalText].ContainsKey(this.SelectedLangage.Id))
                    {
                        RequestTranslation();
                    }
                    if (this.translatedTexts[originalText].ContainsKey(this.SelectedLangage.Id))
                    {
                        return this.translatedTexts[originalText][this.SelectedLangage.Id];
                    }
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
                return originalText;
            }
        }*/
    }
}
