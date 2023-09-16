using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Types
{
    public class POWSolutions
    {
        public string pk { get; set; }
        public string w { get; set; }
        public string n { get; set; }
        public double d { get; set; }
    }

    public class ADProof
    {
        public string headerId { get; set; }
        public string proofBytes { get; set; }
        public string digest { get; set; }
        public int size { get; set; }
    }

    public class BlockHeader
    {
        public string id {  get; set; }
        public long timestamp { get; set; }
        public short version { get; set; }
        public string adProofsRoot { get; set; }
        public string stateRoot { get; set; }
        public string transactionsRoot { get; set; }
        public int nBits { get; set; }
        public string extensionHash { get; set; }
        public POWSolutions powSolutions { get; set; }
        public int height { get; set; }
        public string difficulty { get; set; }
        public string parentId { get; set; }
        public string votes { get; set; }
        public int size { get; set; }
        public string extensionId { get; set; }
        public string transactionsId { get; set; }
        public string adProofsId { get; set; }
    }

    public class BlockTransactions
    {
        public string headerId { get; set; }
        public List<BlockTransaction> transactions { get; set; }
    }

    public class BlockExtension
    {
        public string headerId { get; set; }
        public string digest { get; set; }
        public List<List<string>> fields { get; set; }
    }

    public class Block
    {
        public BlockHeader header { get; set; }
        public BlockTransactions blockTransactions { get; set; }
        public ADProof adProofs { get; set; }
        public BlockExtension extension { get; set; }
        public int size { get; set; }
    }
}
