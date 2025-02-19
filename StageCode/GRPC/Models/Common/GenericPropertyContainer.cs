using CodeExceptionManager.Model.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Common
{
    public class GenericPropertyContainer
    {
        private Dictionary<string, GenericProperty> data = new Dictionary<string, GenericProperty>();
        public GenericProperty Item(string name) {
            if (Contains(name)) return data[name];
            else return null;
        }

        public bool Contains(string name) {
            if (data.ContainsKey(name)) return true;
            else return false;
        }

        public void Insert(GenericProperty item) {
            if (!Contains(item.Name))
                data.Add(item.Name, item);
        }
        public void Insert(string name, Type type = null, List<object> authorizedValues = null, string displayText = "", string proxyOption = "", object value = null) { 
            if(!Contains(name)) {
                GenericProperty newItem = new GenericProperty() { Name = name, DisplayedName = displayText, ProxyOption = proxyOption };
                if (type != null) newItem.Type = type;
                else if (value != null) newItem.Type = value.GetType();
                if (authorizedValues != null && authorizedValues.Count() > 0) newItem.AuthorizedValues.AddRange(authorizedValues.Select(x => Convert.ChangeType(x, newItem.Type)));
                if (value != null) {
                    newItem.UpdateValue(value); 
                } else if(type != null) {
                    if (authorizedValues != null && authorizedValues.Count() > 0) newItem.UpdateValue(authorizedValues.First());
                    else newItem.UpdateValue(type.IsValueType ? Activator.CreateInstance(type) : type == typeof(string) ? "" : null);
                }
                data.Add(name, newItem);
            }
        }
        public void Remove(GenericProperty item) {
            if (Contains(item.Name)) data.Remove(item.Name);   
        }
        public void Remove(string name)
        {
            if (Contains(name)) data.Remove(name);
        }    

        public List<GenericProperty> ToList() {
            return this.data.Values.ToList();
        }

        public GenericPropertyContainer CopyItem() {
            GenericPropertyContainer returnValue = new GenericPropertyContainer();
            foreach(KeyValuePair<string, GenericProperty> item in this.data) {
                returnValue.Insert(item.Value.CopyItem());
            }
            return returnValue;
        }
    }
}
