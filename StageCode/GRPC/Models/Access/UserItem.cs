using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Access
{
    public class UserItem
    {
        private List<GroupItem> groupAppartenance = new List<GroupItem>();
        public long Id { get; set; }
        public string Login { get; set; }
        //only used when edit, won't be stored client side
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsEditable { get; set; }
        public List<GroupItem> GroupAppartenance { get { return this.groupAppartenance; } }
        public string Language { get; set; }
        public string AuthentificationMethod { get; set; }
        public bool ChangePassword { get; set; }
        public long LevelEnabled { get; set; }
        public long LevelVisible { get; set; }
        public string Theme { get; set; }
        public bool Notification {  get; set; }
        //FDB : 22/03/2023 OS-164
        private List<long> idSecuredItems = new List<long>();
        public List<long> IdSecuredItems { get { return idSecuredItems; } }
    }
}
