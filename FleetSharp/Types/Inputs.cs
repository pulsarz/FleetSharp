using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FleetSharp.Types
{
    public class SignedInput
    {
        public string boxId { get; set; }
        public ProverResult spendingProof { get; set; }
    }

    public class UnsignedInput  
    {
        public string boxId { get; set; }
        public Dictionary<int, string?> extension { get; set; } = new Dictionary<int, string?>();
    }

    public class EIP12UnsignedInput : UnsignedInput
    {
        public string transactionId { get; set; }
        public int index { get; set; }
        public string ergoTree { get; set; }
        public long creationHeight { get; set; }
        public string value { get; set; }
        public List<TokenAmount<long>> assets { get; set; }
        public NonMandatoryRegisters additionalRegisters { get; set; }
    }

    public class EIP12UnsignedDataInput
    {
        public string boxId { get; set; }
        public string transactionId { get; set; }
        public int index { get; set; }
        public string ergoTree { get; set; }
        public long creationHeight { get; set; }
        public string value { get; set; }
        public List<TokenAmount<long>> assets { get; set; }
        public NonMandatoryRegisters additionalRegisters { get; set; }
    }

    public class DataInput
    {
        public string boxId { get; set; }
    }
}
