using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Interface
{
    public interface INodeInterface
    {
        public HttpResponseMessage GetLastHttpResponseMessage();
        public Task<List<NodeMempoolTransaction>> GetMempool();
        public Task<NodeTransaction?> GetTX(string? txId);
        public Task<NodeMempoolTransaction?> GetTXFromMempool(string? txId);
        public Task<NodeBlockchainTransactionsWrapper?> GetTransactionsByAddress(string address, int offset, int limit);
        public Task<NodeBlockchainTransactionsWrapper?> GetTransactionsByIndex(long txIndex, int offset, int limit);
        public Task<List<NodeFullTransaction>?> GetWalletTransactions(int? minInclusionHeight, int? maxInclusionHeight, int? minConfirmationsNum, int? maxConfirmationsNum);
        public Task<Box<long>?> GetBox(string? boxId);
        public Task<List<Box<long>?>> GetBoxes(List<string> boxIds);
        public Task<List<Box<long>>> GetBoxesFromUTXOSet(List<string> boxIds);
        public Task<List<Box<long>>> GetUnspentBoxesByErgoTree(string ergoTree);
        public Task<List<Box<long>>> GetUnspentBoxesByErgoTrees(List<string> ergoTrees);
        public Task<List<Box<long>>> GetUnspentBoxesByTokenId(string tokenId, int offset, int limit, string sortDirection, bool includeUnconfirmed);
        public Task<List<Box<long>>?> GetAllUnspentBoxesInWallet(bool considerMempool);
        public Task<List<Box<long>>?> GetUnspentBoxesScan(int scanId, bool considerMempool);
        public Task<List<NodeMempoolTransaction>> GetUnconfirmedTransactionsByErgoTree(string ergoTree, int offset, int limit);
        public Task<List<Box<long>>?> GetBoxesFromMempoolByTokenId(string tokenId);
        public Task<TokenDetail<long>?> GetToken(string? tokenId);
        public Task<NodeBalance<long>?> GetAddressBalance(string? address);
        public Task<List<NodeBalance<long>>> GetAddressesBalances(List<string> addresses);
        public Task<List<string>?> GetBlockIdsAtHeight(int blockHeight);
        public Task<Block?> GetFullBlockById(string headerId);
        public Task<List<BlockHeader>?> GetBlockHeaders(int fromHeight, int? toHeight);
        public Task<NodeIndexedHeight?> GetIndexedHeight();
        public Task<NodeInfo?> GetInfo();
        public Task<bool> UnlockWallet(string password);
        public Task<long> GetCurrentHeight();
        public Task<bool> IsValidAddress(string? address);
        public Task<SignedTransaction?> SignTransaction(UnsignedTransaction tx);
        public Task<string?> SubmitSignedTransaction(SignedTransaction tx);
        public Task<string?> SignAndSubmitTransaction(UnsignedTransaction tx);
        public Task<NodeMempoolTransaction?> FillMissingInfoMempoolTX(NodeMempoolTransaction? tx);
    }
}
