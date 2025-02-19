using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.IO
{
    public class IoStream
    {
        public const string COMPONENT_NAME_PROPERTY_NAME = "ComponentName";
        public const string COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY = "SimpleNameToDisplay";
        private List<IoItemPropertyItem> properties = new List<IoItemPropertyItem>();
        private List<IoStreamDataTableItem> dataTableItems = new List<IoStreamDataTableItem>();
        public int Id { get; set; }
        public string ComponentType { get; set; }
        public string ShortType { get; set; }
        public bool IsArchive { get; set; }
        public List<IoStreamDataTableItem> GetIoStreamDataTableItems() { return this.dataTableItems; }
        public List<IoItemPropertyItem> GetProperties() { return this.properties; }
        public IoItemPropertyItem GetPropertyByName(string propertyName) { return this.properties.Where(x => x.PropertyName.ToLower() == propertyName.ToLower()).FirstOrDefault(); }
        public string ComponentName
        {
            get
            {
                if (GetPropertyByName(COMPONENT_NAME_PROPERTY_NAME) != null)
                {
                    return GetPropertyByName(COMPONENT_NAME_PROPERTY_NAME).PropertyValueInString;
                }
                else
                {
                    return "";
                }
            }
        }

        public string SimpleNameToDisplay
        {
            get
            {
                if (properties != null && GetPropertyByName(COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY) != null)
                    return GetPropertyByName(COMPONENT_NAME_PROPERTY_SIMPLE_NAME_TO_DISPLAY).PropertyValueInString;
                else return "";
            }
        }

        //To append Ipv4Address information from Properties...
        public string IPv4Address 
        { 
            get
            {
                try
                {
                    return this.properties.Where(x => x.PropertyName == "IPv4Address").First().PropertyValueInString;
                }
                catch
                {
                    //Not found, Hilscher ...
                    return "";
                }

            }
        }
    }
}
