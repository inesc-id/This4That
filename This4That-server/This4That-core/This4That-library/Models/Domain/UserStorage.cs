using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Incentives;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.IncentiveModels;

namespace This4That_library.Models.Domain
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
        /* USE IN CENTRALIZED VERSION
        public UserStorage()
        {
            //FIXME: remove, o incentivo nao pode ser incicializado aqui, pois o incentive engine é que o deve fazer
            User user = new User("1234", new Gamification(new List<string>() { { Gamification.GOLD_BADGE } }));
            User user1 = new User("12345", new Gamification(new List<string>() { { Gamification.GOLD_BADGE } }));
            User platformUser = new User("Platform", new Gamification(new List<string>() { { Gamification.GOLD_BADGE } }));
            this.Users.Add(user.UserID, user);
            this.Users.Add(platformUser.UserID, platformUser);
            this.Users.Add(user1.UserID, user1);
        }*/

        public bool CreateUser(string userAddress, Incentive incentive)
        {
            try
            {
                User user;

                user = new User(userAddress, incentive);
                if (Users.Keys.Contains(userAddress))
                    return false;

                Users.Add(userAddress, user);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SaveUserReport(Report report)
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
