using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FleetSharp.Types
{
    public class UnsignedTransaction
    {
        public string id { get; set; }
        public List<UnsignedInput> inputs { get; set; }
        public List<DataInput> dataInputs { get; set; }
        public List<BoxCandidate<long>> outputs { get; set; }
    }

    public class EIP12UnsignedTransaction
    {
        //public string id { get; set; }
        public List<EIP12UnsignedInput> inputs { get; set; }
        public List<EIP12UnsignedDataInput> dataInputs { get; set; }
        public List<BoxCandidate<string>> outputs { get; set; }
    }

    public class SignedTransaction
    {
        public string id { get; set; }
        public List<SignedInput> inputs { get; set; }
        public List<DataInput> dataInputs { get; set; }
        public List<Box<long>> outputs { get; set; }
        public long size { get; set; }
    }
}
