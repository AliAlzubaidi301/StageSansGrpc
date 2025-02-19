using CodeExceptionManager.Model.Objects;
using Grpc.Core;
using Orthodyne.CoreCommunicationLayer.Models.Access;
using Orthodyne.CoreCommunicationLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Controllers
{
    public class ModuleAccessController
    {
        private object locker = new object();
        private string sessionId = string.Empty;
        private string userLogin = string.Empty;
        private string languageUser = string.Empty;
        private string loginReturnMessage = string.Empty;
        private string userPwd = string.Empty;

        private Dictionary<long, bool> currentSessionAccessRules = new Dictionary<long, bool>();
        private Dictionary<long, AccessRuleItem> rules = new Dictionary<long, AccessRuleItem>();
        private Dictionary<long, GroupItem> groups = new Dictionary<long, GroupItem>();
        private Dictionary<long, UserItem> users = new Dictionary<long, UserItem>();
        private Dictionary<long, ApplicationItem> applications = new Dictionary<long, ApplicationItem>();

        private List<LinkRuleSecuredAccessItem> rulesConfiguration = new List<LinkRuleSecuredAccessItem>();
        private ModuleAccessRemoteMethodInvocationService remoteMethods;
        private GeneralController generalController;

        public Dictionary<long, bool> CurrentSessionAccessRules
        {
            get
            {
                return currentSessionAccessRules;
            }
        }
        public Dictionary<long, AccessRuleItem> Rules
        {
            get
            {
                return rules;
            }
        }
        public Dictionary<long, GroupItem> Groups
        {
            get
            {
                return groups;
            }
        }
        public Dictionary<long, UserItem> Users
        {
            get
            {
                return users;
            }
        }
        public List<LinkRuleSecuredAccessItem> RulesConfiguration
        {
            get
            {
                return rulesConfiguration;
            }
        }
        public string SessionId
        {
            get
            {
                return this.sessionId;
            }
        }

        public Dictionary<long, ApplicationItem> Applications
        {
            get
            {
                return applications;
            }
        }

        public string LoginReturnMessage
        {
            get
            {
                return loginReturnMessage;
            }
            set
            {
                loginReturnMessage = value;
            }
        }

        public string LanguageUser
        {
            get
            {
                return languageUser;
            }
            set
            {
                languageUser = value;
            }
        }

        public string UserLogin
        {
            get
            {
                return userLogin;
            }
            set
            {
                userLogin = value;
            }
        }

        public string UserPwd
        {
            get
            {
                return userPwd;
            }
            set
            {
                userPwd = value;
            }
        }

        public bool GetAccess(string technicalName, string application = "", string guid = "")
        {
            if (Rules.Where(i => i.Value.TechnicalName == technicalName).FirstOrDefault().Value == null)
            {
                //create
                AccessRuleItem ruleItem = new AccessRuleItem
                {
                    IsActive = true,
                    IsEditable = true,
                    TechnicalName = technicalName,
                    Text = "[" + application + "] " + technicalName,
                    //ApplyingApplicationGuid = new List<string>() { guid }
                };
                if (string.IsNullOrEmpty(application))
                {
                    ruleItem.Text = technicalName;
                }
                if (!string.IsNullOrEmpty(guid))
                {
                    ruleItem.ApplyingApplicationGuid = new List<string>() { guid };
                }
                ruleItem.AccessGranted.Add(Groups.FirstOrDefault(x => x.Value.Name == "Admin").Value.IdSecuredItems[0], true);
                ruleItem.AccessGranted.Add(Users.FirstOrDefault(x => x.Value.Login == "Admin").Value.IdSecuredItems[0], true);
                EditRules(new List<AccessRuleItem> { ruleItem });
            }
            AccessRuleItem rule = Rules.FirstOrDefault(i => i.Value.TechnicalName == technicalName).Value;
            UserItem user = Users.Values.FirstOrDefault(x => x.IsActive && x.Login.ToLower() == UserLogin.ToLower());
            GroupItem group = Groups.FirstOrDefault(x => x.Value.IsActive && x.Value.UsersInGroup.Where(y => y.IsActive && y.Login == UserLogin.ToLower()).Count() == 1).Value;
            return (rule.AccessGranted.ContainsKey(user.IdSecuredItems[0])
                    && rule.AccessGranted[user.IdSecuredItems[0]])
                    || (group != null
                    && rule.AccessGranted.ContainsKey(group.IdSecuredItems[0])
                    && rule.AccessGranted[group.IdSecuredItems[0]]);
        }

        public bool GetAccessWithLevelVisible(string technicalName, string application = "", string guid = "", string levelVisible = "")
        {
            bool hasAccess = GetAccess(technicalName, application, guid);
            if (!IsAdmin(technicalName))
            {
                if (!string.IsNullOrEmpty(levelVisible))
                {
                    long levelVisibleUser = 0;
                    long levelVisibleGroup = 0;
                    long levelVisibleToCompare = 0;
                    AccessRuleItem rule = Rules.FirstOrDefault(x => x.Value.TechnicalName == technicalName).Value;
                    if (rule.AccessGranted.ContainsKey(Users.Values.FirstOrDefault(x => x.IsActive && x.Login.ToLower() == UserLogin.ToLower()).IdSecuredItems[0]))
                    {
                        UserItem user = Users.Values.FirstOrDefault(x => x.IsActive && x.Login.ToLower() == UserLogin.ToLower());
                        if (user != null)
                        {
                            levelVisibleUser = user.LevelVisible;
                        }
                        GroupItem group = Groups.Values.FirstOrDefault(x => x.IsActive && x.UsersInGroup.Where(y => y.IsActive && y.Login.ToLower() == UserLogin.ToLower()).Count() == 1);
                        if (group != null)
                        {
                            levelVisibleGroup = group.LevelVisible;
                        }
                    }
                    if (levelVisibleGroup > levelVisibleUser)
                    {
                        levelVisibleToCompare = levelVisibleGroup;
                    }
                    else
                    {
                        if (levelVisibleGroup < levelVisibleUser)
                        {
                            levelVisibleToCompare = levelVisibleUser;
                        }
                        else
                        {
                            levelVisibleToCompare = levelVisibleGroup;
                        }
                    }
                    return long.Parse(levelVisible) <= levelVisibleToCompare;
                }
            }
            return hasAccess;
        }

        private bool IsAdmin(string technicalName)
        {
            if(Users.Values.FirstOrDefault(x=>x.Login.ToLower()=="admin" && x.Login==UserLogin)!=null)
            {
                return true;
            }
            GroupItem group = Groups.Values.FirstOrDefault(x => x.IsActive && x.Name.ToLower() == "admin" && x.UsersInGroup.FirstOrDefault(y => y.IsActive && y.Login.ToLower() == UserLogin.ToLower()) != null);
            if (group!=null && Rules.FirstOrDefault(i => i.Value.TechnicalName == technicalName).Value.AccessGranted.ContainsKey(group.IdSecuredItems[0]))
            {
                return true;
            }
            return false;
        }

        public bool GetLevelEnabled(string technicalName, string levelEnabled = "")
        {
            if (!IsAdmin(technicalName))
            {
                if (!string.IsNullOrEmpty(levelEnabled))
                {
                    long levelEnabledUser = 0;
                    long levelEnabledGroup = 0;
                    long levelEnabledToCompare = 0;
                    AccessRuleItem rule = Rules.FirstOrDefault(x => x.Value.TechnicalName == technicalName).Value;
                    if (rule.AccessGranted.ContainsKey(Users.Values.FirstOrDefault(x => x.IsActive && x.Login.ToLower() == UserLogin.ToLower()).IdSecuredItems[0]))
                    {
                        UserItem user = Users.Values.FirstOrDefault(x => x.IsActive && x.Login.ToLower() == UserLogin.ToLower());
                        if (user != null)
                        {
                            levelEnabledUser = user.LevelEnabled;
                        }
                        GroupItem group = Groups.Values.FirstOrDefault(x => x.IsActive && x.UsersInGroup.Where(y => y.IsActive && y.Login.ToLower() == UserLogin.ToLower()).Count() == 1);
                        if (group != null)
                        {
                            levelEnabledGroup = group.LevelEnabled;
                        }
                    }
                    if (levelEnabledGroup > levelEnabledUser)
                    {
                        levelEnabledToCompare = levelEnabledGroup;
                    }
                    else
                    {
                        if (levelEnabledGroup < levelEnabledUser)
                        {
                            levelEnabledToCompare = levelEnabledUser;
                        }
                        else
                        {
                            levelEnabledToCompare = levelEnabledGroup;
                        }
                    }
                    return long.Parse(levelEnabled) <= levelEnabledToCompare;
                }
            }
            return true;
        }

        internal ModuleAccessController(ModuleAccessRemoteMethodInvocationService remoteMethods, GeneralController generalController)
        {
            try
            {
                this.generalController = generalController;
                this.remoteMethods = remoteMethods;
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }



        private void ChangeSession(string newSessionId)
        {

            try
            {
                sessionId = newSessionId;
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }
        public void EditGroups(List<GroupItem> groups)
        {
            try
            {
                try
                {
                }
                catch (RpcException rex)
                {
                    throw rex;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditUsers(List<UserItem> users)
        {
            try
            {
                try
                {
                }
                catch (RpcException rex)
                {
                    throw rex;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void DeleteRule(AccessRuleItem rule)
        {
            try
            {
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void DeleteUsers(List<UserItem> users)
        {
            try
            {
                try
                {
                }
                catch (RpcException rex)
                {
                    throw rex;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void DeleteGroup(List<GroupItem> groups)
        {
            try
            {
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditRules(List<AccessRuleItem> rules)
        {
            try
            {
                try
                {
                }
                catch (RpcException rex)
                {
                    throw rex;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public void EditRulesConfiguration(List<LinkRuleSecuredAccessItem> rulesConfiguration)
        {
            try
            {
                try
                {
                }
                catch (RpcException rex)
                {
                    throw rex;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
            catch (RpcException rex)
            {
                throw rex;
            }
            catch (Exception ex)
            {
                new LoggedException(Assembly.GetExecutingAssembly().GetName().Name, Assembly.GetExecutingAssembly().GetName().Version.ToString(), this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message, ex.StackTrace);
            }
        }

        public bool HasAccess(long idRule)
        {
            if (CurrentSessionAccessRules.ContainsKey(idRule))
            {
                return CurrentSessionAccessRules[idRule];
            }
            else return false;
        }

        public void CheckRunningCore()
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
