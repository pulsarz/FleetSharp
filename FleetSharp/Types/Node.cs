using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FleetSharp.Types
{
    public class NodeErgoTreeToAddress
    {
        public string? address { get; set; }
    }
    /* public class NodeAsset
     {
         public string? tokenId { get; set; }
         public long amount { get; set; }
     }

     public class NodeAdditionalRegisters
     {
         public string? R4 { get; set; }
         public string? R5 { get; set; }
         public string? R6 { get; set; }
         public string? R7 { get; set; }
         public string? R8 { get; set; }
         public string? R9 { get; set; }
     }*/
    /*
    public class NodeBox//mempool only returns boxid for inputs in mempool (it does for outputs), need to manually fetch each box and fill it in here
    {
        public string? boxId { get; set; }
        public long value { get; set; }
        public string? ergoTree { get; set; }
        public string? address { get; set; }//not directly available, is filled in from the ergotree when processing only
        public int creationHeight { get; set; }
        public List<NodeAsset>? assets { get; set; }
        public NodeAdditionalRegisters? additionalRegisters { get; set; }
        public string? transactionId { get; set; }
        public int index { get; set; }
    }*/
    public class NodeMempoolTransaction
    {
        public string? id { get; set; }
        public int size { get; set; }

        public List<Box<long>>? inputs { get; set; }
        //datainputs not used
        public List<Box<long>>? outputs { get; set; }
    }
    /*
        public class NodeToken
        {
            public string? id { get; set; }
            public string? boxId { get; set; }
            public long emissionAmount { get; set; }
            public string? name { get; set; }
            public string? description { get; set; }
            public int decimals { get; set; }
        }

        public class NodeBalanceToken
        {
            public string? tokenId { get; set; }
            public long? amount { get; set; }
            public int? decimals { get; set; }
            public string? name { get; set; }
        }*/

    public class NodeBalanceWrapper<AmountType>
    {
        public AmountType? nanoErgs { get; set; }
        public List<BalanceToken<AmountType>>? tokens { get; set; }
    }

    public class NodeBalance<AmountType>
    {
        public NodeBalanceWrapper<AmountType>? confirmed { get; set; }
        public NodeBalanceWrapper<AmountType>? unconfirmed { get; set; }
    }

    public class NodeIndexedHeight
    {
        public int? indexedHeight { get; set; }
        public int? fullHeight { get; set; }
    }

    public class IsValidAddress
    {
        public string? error { get; set; }
        public string? address { get; set; }
        public bool isValid { get; set; }
    }
}
