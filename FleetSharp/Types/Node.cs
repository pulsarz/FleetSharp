using FleetSharp.Models;
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

    public class SignTXWrapper
    {
        public UnsignedTransaction tx { get; set; }
    }

    public class NodeParameters
    {
        public int? outputCost { get; set; }
        public int? tokenAccessCost { get; set; }
        public long? maxBlockCost { get; set; }
        public long? height { get; set; }
        public long? maxBlockSize { get; set; }
        public int? dataInputCost { get; set; }
        public int? blockVersion { get; set; }
        public int? inputCost { get; set; }
        public long? storageFeeFactor { get; set; }
        public int? minValuePerByte { get; set; }
    }

    public class NodeInfo
    {
        public long? currentTime { get; set; }
        public string? network { get; set; }
        public string? name { get; set; }
        public string? stateType { get; set; }
        public long? difficulty { get; set; }
        public string? bestFullHeaderId { get; set; }
        public string? bestHeaderId { get; set; }
        public int? peerCount { get; set; }
        public int? unconfirmedCount { get; set; }
        public string? appVersion { get; set; }
        public bool? eip37Supported { get; set; }
        public string? stateRoot { get; set; }
        public string? genesisBlockId { get; set; }
        public string? previousFullHeaderId { get; set; }
        public long? fullHeight { get; set; }
        public long? headersHeight { get; set; }
        public string? stateVersion { get; set; }
        public double? fullBlocksScore { get; set; }
        public long? maxPeerHeight { get; set; }
        public long? launchTime { get; set; }
        public bool? isExplorer { get; set; }
        public long? lastSeenMessageTime { get; set; }
        public bool? eip27Supported { get; set; }
        public double? headersScore { get; set; }
        public NodeParameters? parameters { get; set; }
        public bool? isMining { get; set; }
    }

    public class WalletBoxesUnspent
    {
        public int? confirmationsNum { get; set; }
        public string? address { get; set; }
        public string? creationTransaction { get; set; }
        public List<int>? scans { get; set; }
        public bool? onchain { get; set; }
        public int? creationOutIndex { get; set; }
        public string? spendingTransaction { get; set; }
        public Box<long>? box { get; set; }
    }

    public class NodeWalletUnlock
    {
        public string pass { get; set; }
    }
}
