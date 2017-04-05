using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_serverNode.Domain
{
    public class PendingPayment
    {
        private string userID;
        private string refToPay;
        private object valToPay;
        private DateTime timestamp;

        public PendingPayment(string pUserId, string pRefToPay, object pValtoPay)
        {
            this.userID = pUserId;
            this.refToPay = pRefToPay;
            this.valToPay = pValtoPay;
            this.timestamp = new DateTime();
        }
    }
}
