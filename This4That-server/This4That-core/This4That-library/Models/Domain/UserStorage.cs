using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.IncentiveModels;

namespace This4That_library.Domain
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

        public UserStorage()
        {
            //FIXME: remove
            User user = new User("1234", IncentiveSchemesEnum.Centralized, new Gamification());
            User platformUser = new User("Platform", IncentiveSchemesEnum.Centralized, new Gamification());
            this.Users.Add("1234", user);
        }

        public string CreateUser(IncentiveSchemesEnum incentiveScheme, Incentive incentive)
        {
            try
            {
                User user;
                string userId = Guid.NewGuid().ToString().Substring(0, 8);

                while (Users.ContainsKey(userId))
                {
                    userId = Guid.NewGuid().ToString().Substring(0, 8);
                }
                user = new User(userId, incentiveScheme, incentive);
                Users.Add(userId, user);
                return user.UserID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SaveUserReport(ReportDTO report)
        {
            try
            {
                Users[report.UserID].MyReports.Add(report.ReportID);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public User GetUserByID(string userID)
        {
            if (Users.Keys.Contains(userID))
                return Users[userID];
            else
                return null;
        }
    }
}
