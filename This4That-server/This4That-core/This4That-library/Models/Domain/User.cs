using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.IncentiveModels;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class User
    {
        private string userID;
        private List<string> myTasks = new List<string>();
        private List<string> myReports = new List<string>();    
        private List<string> subscribedTopics = new List<string>();

        private Wallet wallet = new Wallet();

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

        public Wallet Wallet
        {
            get
            {
                return wallet;
            }

            set
            {
                wallet = value;
            }
        }

        public User(string userId, Incentive incentive)
        {
            userID = userId;
            this.Wallet.WalletAddress = userId;
            this.Wallet.Balance = incentive.WalletEmpty();
        }

        public void SubscribeTopic(string topicName)
        {
            if (!SubscribedTopics.Contains(topicName))
                SubscribedTopics.Add(topicName);
        }
    }
}
