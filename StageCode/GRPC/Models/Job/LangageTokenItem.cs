using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Job
{
    public class LangageTokenItem
    {
        public string Name { get; set; }
        private List<string> tokens = new List<string>(); 
        private List<string> textForAutocomplete = new List<string>();

        //TCO : Use for Treeview
        private List<LangageTokenItem> childrens = new List<LangageTokenItem>();

        public List<string> Tokens
        {
            get 
            { 
                return tokens; 
            }
        }

        public List<string> TextForAutocomplete
        {
            get
            {
                return textForAutocomplete;
            }
        }

        public List<LangageTokenItem> Childrens
        {
            get
            {
                return childrens;
            }
        }
    }
}
