using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orthodyne.CoreCommunicationLayer.Models.Access
{
    public class GroupItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long LevelEnabled { get; set; }
        public long LevelVisible { get; set; }

        //FDB : 22/03/2023 OS-164
        private List<long> idSecuredItems = new List<long>();
        public List<long> IdSecuredItems { get { return idSecuredItems; } }

        private List<UserItem> usersInGroup=new List<UserItem>();
        public List<UserItem> UsersInGroup { get { return usersInGroup; } }

        //FDB : 05/04/2023 OS-196
        public bool IsEditable { get; set; }
    }
}
