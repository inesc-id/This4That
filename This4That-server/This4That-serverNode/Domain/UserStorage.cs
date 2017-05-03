using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration.ReportDTO;

namespace This4That_serverNode.Domain
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
            User user = new User();
            user.UserID = "1234";
            this.Users.Add("1234", user);
        }

        public string CreateUser()
        {
            try
            {
                User user;
                string userId = Guid.NewGuid().ToString().Substring(0, 8);

                while (Users.ContainsKey(userId))
                {
                    userId = Guid.NewGuid().ToString().Substring(0, 8);
                }
                user = new User();
                user.UserID = userId;
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

        public User GetUser(string userID)
        {
            if (Users.Keys.Contains(userID))
                return Users[userID];
            else
                return null;
        }
    }
}
