using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.ReportDTO;

namespace This4That_serverNode.Domain
{
    public class User
    {
        private string userID;
        private List<string> myTasks = new List<string>();
        private List<string> myReports = new List<string>();    
        private List<string> subscribedTopics = new List<string>();

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

        public List<string> MyTasks
        {
            get
            {
                return myTasks;
            }

            set
            {
                myTasks = value;
            }
        }

        public List<string> SubscribedTopics
        {
            get
            {
                return subscribedTopics;
            }

            set
            {
                subscribedTopics = value;
            }
        }

        public List<string> MyReports
        {
            get
            {
                return myReports;
            }

            set
            {
                myReports = value;
            }
        }


        public void SubscribeTopic(string topicName)
        {
            if (!SubscribedTopics.Contains(topicName))
                SubscribedTopics.Add(topicName);
        }
    }
}
