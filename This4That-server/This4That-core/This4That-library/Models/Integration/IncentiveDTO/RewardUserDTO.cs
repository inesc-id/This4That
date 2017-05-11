using This4That_platform.Models.Integration;

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
