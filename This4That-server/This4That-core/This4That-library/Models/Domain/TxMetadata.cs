using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Incentives;

namespace This4That_library.Models.Domain
{
    public class TxMetadata
    {
        private Incentive incentive;
        private int quantity;
        private object txValue;

        public Incentive Incentive
        {
            get
            {
                return incentive;
            }

            set
            {
                incentive = value;
            }
        }

        public int Quantity
        {
            get
            {
                return quantity;
            }

            set
            {
                quantity = value;
            }
        }

        public object TxValue
        {
            get
            {
                return txValue;
            }

            set
            {
                txValue = value;
            }
        }

        public TxMetadata(Incentive pIncentive, int qty, object value)
        {
            this.Incentive = pIncentive;
            this.Quantity = qty;
            this.TxValue = value;
        }
    }
}
