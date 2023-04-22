using System;
using System.Collections.Generic;
using System.Text;

namespace FleetSharp.Types
{
    public class NodeErgoTreeToAddress
    {
        public string? address { get; set; }
    }
    public class NodeAsset
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
    }

    public class NodeBox//mempool only returns boxid for inputs in mempool (it does for outputs), need to manually fetch each box and fill it in here
    {
        public string? boxId { get; set; }
        public long value { get; set; }
        public string? ergoTree { get; set; }
        public string? address { get; set; }//not directly available, is filled in from the ergotree when processing only
        public long creationHeight { get; set; }
        public List<NodeAsset>? assets { get; set; }
        public NodeAdditionalRegisters? additionalRegisters { get; set; }
        public string? transactionId { get; set; }
        public long index { get; set; }
    }
    public class NodeMempoolTransaction
    {
        public string? id { get; set; }
        public long size { get; set; }

        public List<NodeBox?>? inputs { get; set; }
        //datainputs not used
        public List<NodeBox?>? outputs { get; set; }
    }

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
    }

    public class NodeBalanceWrapper
    {
        public long? nanoErgs { get; set; }
        public List<NodeBalanceToken>? tokens { get; set; }
    }

    public class NodeBalance
    {
        public NodeBalanceWrapper? confirmed { get; set; }
        public NodeBalanceWrapper? unconfirmed { get; set; }
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
