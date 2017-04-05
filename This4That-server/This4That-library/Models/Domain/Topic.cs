using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class Topic
    {
        private string name;
        private List<string> listOfTaskIDs = new List<string>();

        public Topic(string topicName)
        {
            this.Name = topicName;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public List<string> ListOfTaskIDs
        {
            get
            {
                return listOfTaskIDs;
            }

            set
            {
                listOfTaskIDs = value;
            }
        }
    }
}
