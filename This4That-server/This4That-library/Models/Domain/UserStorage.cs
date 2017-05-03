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
        private Dictionary<string, @object> users = new Dictionary<string, @object>();

        public Dictionary<string, @object> Users
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
            @object user = new @object("1234", IncentiveSchemesEnum.Centralized, new Gamification());
            this.Users.Add("1234", user);
        }

        public string CreateUser(IncentiveSchemesEnum incentiveScheme, Incentive incentive)
        {
            try
            {
                @object user;
                string userId = Guid.NewGuid().ToString().Substring(0, 8);

                while (Users.ContainsKey(userId))
                {
                    userId = Guid.NewGuid().ToString().Substring(0, 8);
                }
                user = new @object(userId, incentiveScheme, incentive);
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

        public @object GetUserByID(string userID)
        {
            if (Users.Keys.Contains(userID))
                return Users[userID];
            else
                return null;
        }
    }
}
