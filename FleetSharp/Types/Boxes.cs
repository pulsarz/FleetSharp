using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FleetSharp.Types
{
    public class BoxBaseType<AmountType>
    {
        public string ergoTree { get; set; }
        public long creationHeight { get; set; }
        public AmountType value { get; set; }
        public List<TokenAmount<AmountType>> assets { get; set; }
        public NonMandatoryRegisters additionalRegisters { get; set; }
    }

    public class BoxCandidate<AmountType> : BoxBaseType<AmountType>
    {
        public string? boxId { get; set; }
    }

    public class Box<AmountType> : BoxBaseType<AmountType>
    {
        public string boxId { get; set; }
        public string transactionId { get; set; }
        public int index { get; set; }
    }
}
