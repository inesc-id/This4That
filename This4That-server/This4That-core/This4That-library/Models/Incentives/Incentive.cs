using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace This4That_library.Models.Incentives
{
    [Serializable]
    public abstract class Incentive
    {
        private List<string> incentives;

        public List<string> Incentives
        {
            get
            {
                return incentives;
            }
        }

        public Incentive(List<string> incentives)
        {
            this.incentives = incentives;
        }

        public abstract bool CheckSufficientCredits(object balance, object incentiveValue);

        public abstract Dictionary<string, int> InitIncentivesWallet();

        public abstract List<string> GetIncentivesName();

        public abstract IncentiveAssigned RegisterUserIncentive();

        public abstract IncentiveAssigned CompleteTaskIncentive();

        public abstract IncentiveAssigned CreateTaskIncentive();

    }

    [Serializable]
    public class IncentiveAssigned
    {
        [JsonProperty(PropertyName = "name")]
        private string incentiveName;
        [JsonProperty(PropertyName = "quantity")]
        private int incentiveQty;

        public IncentiveAssigned()
        {

        }
        public IncentiveAssigned(string incentiveName, int incentiveQty)
        {
            this.incentiveName = incentiveName;
            this.incentiveQty = incentiveQty;
        }

        public string IncentiveName
        {
            get
            {
                return incentiveName;
            }

            set
            {
                incentiveName = value;
            }
        }

        public int IncentiveQty
        {
            get
            {
                return incentiveQty;
            }

            set
            {
                incentiveQty = value;
            }
        }

        public override string ToString()
        {
            return $"Incentive Name: [{IncentiveName}] Quantity: [{incentiveQty}]";
        }
    }
}
