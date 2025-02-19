using Orthodyne.CoreCommunicationLayer.Models.Alarms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class IoController
    {
        private List<IoItemFeature> features;
        private List<LinkItem> linkWithStreams;
        private List<IoItemPropertyItem> properties;
        private List<string> valuesInString;
        private List<AlarmItem> alarms;
        private bool inAlarm = false;
        private string alarmName = "false";
        private Dictionary<string, List<string>> correspondences = new Dictionary<string, List<string>>();

        public const string COMPONENT_NAME_PROPERTY_NAME = "ComponentName";
        public const string COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY = "SimpleNameToDisplay";
        public const string GROUP_PROPERTY_NAME = "group_flag_id";
        public const string LINK_CHANGE_TO_ANOTHER_COMPONENT = "link_change_to_component";


        public Dictionary<string, List<string>> Correspondences
        {
            get
            {
                return correspondences;
            }
        }
        public int Id { get; set; }
        public string ComponentType { get; set; }
        public string ShortType { get; set; }
        public bool IsArchive { get; set; }
        public bool IsSavedInDB { get; set; }
        public bool ToSave { get; set; }
        public bool IsAddingLink { get; set; }
        //public bool IsRemovingLink { get; set; }
        public string SimpleNameNewValue { get; set; }

        public List<Tuple<int, int, int>> LinkToDelete { get; set; }

        public List<LinkItem> GetLinkWithStreams() { return this.linkWithStreams; }
        public List<IoItemFeature> GetFeatures() { return this.features; }
        public List<IoItemPropertyItem> GetProperties() { return this.properties; }
        public List<string> GetValuesInString() { return this.valuesInString; }
        public List<AlarmItem> GetAlarms() { return this.alarms; }
        public IoItemFeature GetFeatureByName(string featureName) { return this.features.Where(x => x.FeatureName == featureName).FirstOrDefault(); }
        public IoItemPropertyItem GetPropertyByName(string propertyName) { return this.properties.Where(x => x.PropertyName == propertyName).FirstOrDefault(); }

        public IoController()
        {
            features = new List<IoItemFeature>();
            properties = new List<IoItemPropertyItem>();
            valuesInString = new List<string>();
            linkWithStreams = new List<LinkItem>();
            alarms = new List<AlarmItem>();
            LinkToDelete = new List<Tuple<int, int, int>>();
            correspondences.Add("AD", new List<string>
            {
                "AD"
            });
            correspondences.Add("ALA", new List<string>
            {
                "ALA"
            });
            correspondences.Add("AO", new List<string>
            {
                "AO",
                "AD",
                "SIGNAL"
            });
            correspondences.Add("DI", new List<string>
            {
                "DI",
                "REL"
            });
            correspondences.Add("FID", new List<string>
            {
                "FID"
            });
            correspondences.Add("POWER", new List<string>
            {
                "POWER"
            });
            correspondences.Add("REGULATION", new List<string>
            {
                "REGULATION"
            });
            correspondences.Add("REL", new List<string>
            {
                "DI",
                "REL",
                "ALA"
            });
            correspondences.Add("SIGNAL", new List<string>
            {
                "SIGNAL"
            });
            correspondences.Add("TEMPCTRL", new List<string>
            {
                "TEMPCTRL"
            });
        }

        public string ComponentName
        {
            get
            {
                if (properties != null && GetPropertyByName(COMPONENT_NAME_PROPERTY_NAME) != null)
                    return GetPropertyByName(COMPONENT_NAME_PROPERTY_NAME).PropertyValueInString;
                else return "";
            }
        }

        public string SimpleNameToDisplay
        {
            get
            {
                if (properties != null && GetPropertyByName(COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY) != null)
                {
                    SimpleNameNewValue = GetPropertyByName(COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY).PropertyValueInString;
                    return GetPropertyByName(COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY).PropertyValueInString;
                }
                SimpleNameNewValue = "";
                return "";
            }
        }

        public string FullName
        {
            get
            {
                return SimpleNameToDisplay + "[" + ComponentName + "]";
            }
        }

        public bool InAlarm
        {
            get
            {
                return inAlarm;
            }
            set
            {
                inAlarm = value;
                AlarmName = value.ToString();
            }
        }

        public string AlarmName
        {
            get
            {
                return alarmName;
            }
            set
            {
                alarmName = value;
            }
        }

        public IoController CopyItem()
        {
            IoController copy = new IoController()
            {
                ComponentType = this.ComponentType,
                Id = this.Id,
                ShortType = this.ShortType
            };
            foreach (IoItemPropertyItem property in this.properties) copy.properties.Add(property.CopyItem());
            foreach (IoItemFeature feature in this.features) copy.features.Add(feature.CopyItem());
            foreach (AlarmItem alarm in this.alarms) copy.alarms.Add(alarm);
            foreach (LinkItem item in this.GetLinkWithStreams())
            {
                copy.GetLinkWithStreams().Add(item);
            }
            return copy;
        }
    }
}
