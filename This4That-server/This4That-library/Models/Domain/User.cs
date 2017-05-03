﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.IncentiveModels;

namespace This4That_library.Domain
{
    [Serializable]
    public class @object
    {
        private string userID;
        private List<string> myTasks = new List<string>();
        private List<string> myReports = new List<string>();    
        private List<string> subscribedTopics = new List<string>();
        private IncentiveSchemesEnum incentiveScheme;
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

        public IncentiveSchemesEnum IncentiveScheme
        {
            get
            {
                return incentiveScheme;
            }

            set
            {
                incentiveScheme = value;
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

        public @object(string userId, IncentiveSchemesEnum incentiveScheme, Incentive incentive)
        {
            userID = userId;
            this.IncentiveScheme = incentiveScheme;
            this.Wallet.Balance = incentive.InitWalletValue();
        }

        public void SubscribeTopic(string topicName)
        {
            if (!SubscribedTopics.Contains(topicName))
                SubscribedTopics.Add(topicName);
        }
    }
}
