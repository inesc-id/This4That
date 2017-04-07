using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_serverNode.Domain
{
    public class UserStorage
    {
        private Dictionary<string, User> users = new Dictionary<string, User>();

        public Dictionary<string, User> Users
        {
            get
            {
                return users;
            }

            set
            {
                users = value;
            }
        }
    }
}
