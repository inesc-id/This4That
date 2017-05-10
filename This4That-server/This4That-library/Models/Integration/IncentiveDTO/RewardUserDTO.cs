using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_platform.Integration;

namespace This4That_library.Models.Integration.IncentiveDTO
{
    public class RewardUserDTO : APIResponseDTO
    {
        private object reward;

        public object Reward
        {
            get
            {
                return reward;
            }

            set
            {
                reward = value;
            }
        }
    }
}
