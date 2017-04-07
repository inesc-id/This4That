using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Domain;

namespace This4That_serverNode.Domain
{
    public class User
    {
        private string userID;
        private List<CSTask> colTasks = new List<CSTask>();

        public string UserID
        {
            get
            {
                return userID;
            }

            set
            {
                userID = value;
            }
        }

        public List<CSTask> ColTasks
        {
            get
            {
                return colTasks;
            }

            set
            {
                colTasks = value;
            }
        }
    }
}
