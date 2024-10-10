using FleetSharp.Interface;
using FleetSharp.Models;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FleetSharp
{
    public class NodeInterface : INodeInterface
    {
        private static HttpClient _client;

        private string _nodeURL;
        private string apiKey;

        private JsonSerializerOptions defaultSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        private HttpResponseMessage lastHttpResponseMessage;

        public NodeInterface(string nodeURL, string? apiKey = null) {
            _nodeURL = nodeURL;
            this.apiKey = apiKey;

            _client = new HttpClient();

            //add api key
            if (apiKey != null) _client.DefaultRequestHeaders.Add("api_key", apiKey);
        }

        public HttpResponseMessage GetLastHttpResponseMessage()
        {
            return lastHttpResponseMessage;
        }

        //take should not be hiogher then 100
        private async Task<List<NodeMempoolTransaction>?> _GetMempool(int offset, int take)
        {
            var response = await _client.GetAsync($"{_nodeURL}/transactions/unconfirmed?limit={take}&offset={offset}");
            lastHttpResponseMessage = response;

            List<NodeMempoolTransaction>? mempool = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<List<NodeMempoolTransaction>>();
            return mempool;
        }

        public async Task<List<NodeMempoolTransaction>> GetMempool()
        {
            List<NodeMempoolTransaction> mempool = new List<NodeMempoolTransaction>();
            var bStop = false;
            var offset = 0;
            while (!bStop)
            {
                List<NodeMempoolTransaction>? chunk = await _GetMempool(offset, 100);
                offset += chunk.Count;
                if (chunk == null || chunk.Count < 100)
                {
                    bStop = true;
                }
                if (chunk != null)
                {
                    mempool.AddRange(chunk);
                }
            }

            return mempool;
        }

        public async Task<NodeTransaction?> GetTX(string? txId)
        {
            NodeTransaction? tx = null;
            if (txId == null) return null;

            var response = await _client.GetAsync($"{_nodeURL}/blockchain/transaction/byId/{txId}");
            lastHttpResponseMessage = response;
            tx = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<NodeTransaction>();

            return tx;
        }

        public async Task<NodeMempoolTransaction?> GetTXFromMempool(string? txId)
        {
            NodeMempoolTransaction? tx = null;
            if (txId == null) return null;

            var response = await _client.GetAsync($"{_nodeURL}/transactions/unconfirmed/byTransactionId/{txId}");
            lastHttpResponseMessage = response;
            tx = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<NodeMempoolTransaction>();

            return tx;
        }

		public async Task<NodeBlockchainTransactionsWrapper?> GetTransactionsByAddress(string address, int offset = 0, int limit = 100)
		{
			var response = await _client.PostAsync($"{_nodeURL}/blockchain/transaction/byAddress?offset={offset}&limit={limit}", new StringContent(address, Encoding.UTF8, "application/json"));
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
			if (content != null && content != "")
			{
				var ret = JsonSerializer.Deserialize<NodeBlockchainTransactionsWrapper>(content);
                return ret;
			}

            return null;
		}

        public async Task<NodeBlockchainTransactionsWrapper?> GetTransactionsByIndex(long txIndex, int offset = 0, int limit = 100)
        {
            var response = await _client.GetAsync($"{_nodeURL}/blockchain/transaction/byIndex/{txIndex}?offset={offset}&limit={limit}");
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (content != null && content != "")
            {
                var ret = JsonSerializer.Deserialize<NodeBlockchainTransactionsWrapper>(content);
                return ret;
            }

            return null;
        }

        public async Task<List<NodeFullTransaction>?> GetWalletTransactions(int? minInclusionHeight, int? maxInclusionHeight, int? minConfirmationsNum, int? maxConfirmationsNum)
        {
            var url = $"{_nodeURL}/wallet/transactions?";
            if (minInclusionHeight != null) url += $"minInclusionHeight={minInclusionHeight}&";
            if (maxInclusionHeight != null) url += $"maxInclusionHeight={maxInclusionHeight}&";
            if (minConfirmationsNum != null) url += $"minConfirmations={minConfirmationsNum}&";
            if (maxConfirmationsNum != null) url += $"maxConfirmations={maxConfirmationsNum}&";
            url = url.Substring(0, url.Length - 1);

            var response = await _client.GetAsync(url);
            lastHttpResponseMessage = response;
            return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<List<NodeFullTransaction>>();
        }

        //Tries to find unspent + mempool first, if not found tries spent ones.
        public async Task<Box<long>?> GetBox(string? boxId)
        {
            Box<long>? box = null;
            if (boxId == null) return null;

            try
            {
                var response = await _client.GetAsync($"{_nodeURL}/utxo/withPool/byId/{boxId}");
                lastHttpResponseMessage = response;
                box = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Box<long>>();
            }
            catch (Exception e)
            {

            }
            
            if (box == null)
            {
                //Retrieve box from blockchain indexer instead
                var response = await _client.GetAsync($"{_nodeURL}/blockchain/box/byId/{boxId}");
                lastHttpResponseMessage = response;
                box = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Box<long>>();
            }

            return box;
        }
        public async Task<List<Box<long>?>> GetBoxes(List<string> boxIds)
        {
            var taskList = new List<Task<Box<long>?>>();

            foreach (var boxId in boxIds)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        return await GetBox(boxId).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                });

                taskList.Add(task);
            }

            var result = await Task.WhenAll(taskList.ToList()).ConfigureAwait(false);
            return result.ToList();
        }

        public async Task<List<Box<long>>> GetBoxesFromUTXOSet(List<string> boxIds)
        {
            var boxes = new List<Box<long>>();

            var response = await _client.PostAsJsonAsync($"{_nodeURL}/utxo/withPool/byIds", boxIds, defaultSerializerOptions);
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (content != null && content != "")
            {
                boxes = JsonSerializer.Deserialize<List<Box<long>>>(content);
            }

            return boxes;
        }

        public async Task<List<Box<long>>> GetUnspentBoxesByErgoTree(string ergoTree)
        {
            List<Box<long>> boxes = new List<Box<long>>();
            List<Box<long>>? temp = null;
            int limit = 1000;

            if (ergoTree == null) return null;

            do
            {
                //temp = await client.GetFromJsonAsync<List<NodeBox>>($"{_nodeURL}/blockchain/box/unspent/byErgoTree/{ergoTree}?offset={boxes.Count}&limit={limit}", jsonOptions);
                temp = null;
                //var response = await client.PostAsJsonAsync($"{_nodeURL}/blockchain/box/unspent/byErgoTree?offset={boxes.Count}&limit={limit}", ergoTree);
                var response = await _client.PostAsync($"{_nodeURL}/blockchain/box/unspent/byErgoTree?offset={boxes.Count}&limit={limit}", new StringContent(ergoTree, Encoding.UTF8, "application/json"));
                lastHttpResponseMessage = response;
                var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                if (content != null && content != "")
                {
                    temp = JsonSerializer.Deserialize<List<Box<long>>>(content);

                    if (temp != null)
                    {
                        boxes.AddRange(temp);
                    }
                }
            }
            while (temp != null && temp?.Count == limit);

            return boxes;
        }

        //Will run all ergoTrees parallel!
        public async Task<List<Box<long>>> GetUnspentBoxesByErgoTrees(List<string> ergoTrees)
        {
            List<Box<long>> boxes = new List<Box<long>>();
            var taskList = new List<Task<List<Box<long>>>>();

            foreach (var ergoTree in ergoTrees)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        return await GetUnspentBoxesByErgoTree(ergoTree).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                });

                taskList.Add(task);
            }

            var result = await Task.WhenAll(taskList.ToList()).ConfigureAwait(false);
            foreach (var unspent in result)
            {
                if (unspent != null) boxes.AddRange(unspent);
            }

            return boxes;
        }

        //Required node version >= 5.0.15
        public async Task<List<Box<long>>> GetUnspentBoxesByTokenId(string tokenId, int offset=0, int limit=100, string sortDirection="desc", bool includeUnconfirmed = true)
        {
            if (tokenId == null) return null;

            //blockchain/box/unspent/byTokenId/0fdb7ff8b37479b6eb7aab38d45af2cfeefabbefdc7eebc0348d25dd65bc2c91?offset=0&limit=1000&sortDirection=desc&includeUnconfirmed=true
            var response = await _client.GetAsync($"{_nodeURL}/blockchain/box/unspent/byTokenId/{tokenId}?offset={offset}&limit={limit}&sortDirection={sortDirection}&includeUnconfirmed={(includeUnconfirmed ? "true" : "false")}");
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (content != null && content != "")
            {
                return JsonSerializer.Deserialize<List<Box<long>>>(content);
            }

            return null;
        }

        public async Task<List<Box<long>>?> GetAllUnspentBoxesInWallet(bool considerMempool = true)
        {
            var response = await _client.GetAsync($"{_nodeURL}/wallet/boxes/unspent?minConfirmations={(considerMempool ? "-1" : "0")}");
            lastHttpResponseMessage = response;
            var boxes = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<List<WalletBoxesUnspent>>();

            return boxes?.Select(x => x.box)?.ToList();
        }

        public async Task<List<Box<long>>?> GetUnspentBoxesScan(int scanId, bool considerMempool = true)
        {
            var response = await _client.GetAsync($"{_nodeURL}/scan/unspentBoxes/{scanId}?minConfirmations={(considerMempool ? "-1" : "0")}");
            lastHttpResponseMessage = response;
            var boxes = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<List<WalletBoxesUnspent>>();

            return boxes?.Select(x => x.box)?.ToList();
        }

        public async Task<List<NodeMempoolTransaction>> GetUnconfirmedTransactionsByErgoTree(string ergoTree, int offset = 0, int limit = 9999999)
        {
            List<NodeMempoolTransaction> txes = new List<NodeMempoolTransaction>();
            List<NodeMempoolTransaction>? temp = null;
            int chunkSize = 100;

            if (ergoTree == null) return null;

            do
            {
                temp = null;
                chunkSize = Math.Min(100, limit);

                var response = await _client.PostAsJsonAsync($"{_nodeURL}/transactions/unconfirmed/byErgoTree?offset={txes.Count + offset}&limit={chunkSize}", ergoTree, defaultSerializerOptions);
                lastHttpResponseMessage = response;
                var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                if (content != null)
                {
                    temp = JsonSerializer.Deserialize<List<NodeMempoolTransaction>>(content);

                    if (temp != null)
                    {
                        txes.AddRange(temp);
                    }
                }
            }
            while (temp != null && temp?.Count == chunkSize && temp?.Count < limit);

            return txes;
        }

        public async Task<List<Box<long>>?> GetBoxesFromMempoolByTokenId(string tokenId)
        {
            var response = await _client.GetAsync($"{_nodeURL}/transactions/unconfirmed/outputs/byTokenId/{tokenId}");
            lastHttpResponseMessage = response;
            var boxes = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<List<Box<long>>>();

            return boxes;
        }

        public async Task<TokenDetail<long>?> GetToken(string? tokenId)
        {
            TokenDetail<long>? token = null;
            if (tokenId == null) return null;

            var response = await _client.GetAsync($"{_nodeURL}/blockchain/token/byId/{tokenId}");
            lastHttpResponseMessage = response;
            token = await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<TokenDetail<long>>();

            return token;
        }

        public async Task<NodeBalance<long>?> GetAddressBalance(string? address)
        {
            NodeBalance<long>? balance = null;

            if (address == null) return null;

            var response = await _client.PostAsync($"{_nodeURL}/blockchain/balance", new StringContent(address, Encoding.UTF8, "application/json"));
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (content != null && content != "")
            {
                balance = JsonSerializer.Deserialize<NodeBalance<long>>(content);
            }

            return balance;
        }

        //Will run all addresses parallel!
        public async Task<List<NodeBalance<long>>> GetAddressesBalances(List<string> addresses)
        {
            var taskList = new List<Task<NodeBalance<long>>>();

            foreach (var address in addresses)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        return await GetAddressBalance(address).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                });

                taskList.Add(task);
            }

            var result = await Task.WhenAll(taskList.ToList()).ConfigureAwait(false);
            return result.ToList();
        }
        /*
        public async Task<string?> ErgoTreeToAddress(string? ergoTree)
        {
            if (ergoTree == null) return null;
            NodeErgoTreeToAddress? address = await client.GetFromJsonAsync<NodeErgoTreeToAddress>($"{_nodeURL}/utils/ergoTreeToAddress/{ergoTree}");
            return address.address;
        }*/

        public async Task<List<string>?> GetBlockIdsAtHeight(int blockHeight)
        {
            var response = await _client.GetAsync($"{_nodeURL}/blocks/at/{blockHeight}");
            lastHttpResponseMessage = response;
            return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<List<string>>();
        }

        public async Task<Block?> GetFullBlockById(string headerId)
        {
            var response = await _client.GetAsync($"{_nodeURL}/blocks/{headerId}");
            lastHttpResponseMessage = response;
            return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<Block>();
        }

        public async Task<List<BlockHeader>?> GetBlockHeaders(int fromHeight = 0, int? toHeight = null)
        {
            var url = $"{_nodeURL}/blocks/chainSlice?fromHeight={fromHeight}&toHeight={toHeight ?? 999999999}";
            var response = await _client.GetAsync(url);
            lastHttpResponseMessage = response;
            return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<List<BlockHeader>>();
        }

        public async Task<NodeIndexedHeight?> GetIndexedHeight()
        {
            var response = await _client.GetAsync($"{_nodeURL}/blockchain/indexedHeight");
            lastHttpResponseMessage = response;
            return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<NodeIndexedHeight>();
        }


        public async Task<NodeInfo?> GetInfo()
        {
            var response = await _client.GetAsync($"{_nodeURL}/info");
            lastHttpResponseMessage = response;
            return await response.EnsureSuccessStatusCode().Content.ReadFromJsonAsync<NodeInfo>();
        }

        public async Task<bool> UnlockWallet(string password)
        {
            var response = await _client.PostAsJsonAsync($"{_nodeURL}/wallet/unlock", new NodeWalletUnlock { pass = password }, defaultSerializerOptions);
            lastHttpResponseMessage = response;
            return response.IsSuccessStatusCode;
        }

        public async Task<long> GetCurrentHeight()
        {
            var info = await GetInfo();
            return info?.fullHeight ?? 0;
        }

        public async Task<bool> IsValidAddress(string? address)
        {
            if (address == null) return false;

            var response = await _client.PostAsJsonAsync($"{_nodeURL}/utils/address", address);
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (content == null || content == "") return false;
            IsValidAddress? valid = JsonSerializer.Deserialize<IsValidAddress>(content);
            if (valid == null) return false;
            return valid.isValid;
        }

        //Sign an arbitrary transaction
        public async Task<SignedTransaction?> SignTransaction(UnsignedTransaction tx)
        {
            SignTXWrapper wrapper = new SignTXWrapper() { tx = tx };

            var response = await _client.PostAsJsonAsync($"{_nodeURL}/wallet/transaction/sign", wrapper, defaultSerializerOptions);
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (content == null || content == "") return null;

            SignedTransaction? signed = JsonSerializer.Deserialize<SignedTransaction>(content);
            return signed;
        }

        public async Task<string?> SubmitSignedTransaction(SignedTransaction tx)
        {
            var response = await _client.PostAsJsonAsync($"{_nodeURL}/transactions", tx, defaultSerializerOptions);
            lastHttpResponseMessage = response;
            var content = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            if (content == null || content == "") return null;

            string? transactionId = JsonSerializer.Deserialize<string>(content);
            return transactionId;
        }

        public async Task<string?> SignAndSubmitTransaction(UnsignedTransaction tx)
        {
            var signedTX = await SignTransaction(tx);
            if (signedTX == null) return null;

            var transactionId = await SubmitSignedTransaction(signedTX);
            return transactionId;
        }

        public async Task<NodeMempoolTransaction?> FillMissingInfoMempoolTX(NodeMempoolTransaction? tx)
        {
            if (tx == null) return null;

            var inputBoxIds = tx.inputs.Select(x => x.boxId).Where(x => x != null).ToList();
            //var inputBoxes = await GetBoxes(inputBoxIds);
            var inputBoxes = await GetBoxesFromUTXOSet(inputBoxIds);

            tx.inputs = tx.inputs.Where(x => !inputBoxes.Exists(y => y.boxId == x.boxId)).ToList();
            foreach (var tempBox in inputBoxes)
            {
                if (tempBox != null) tx.inputs.Add(tempBox);
            }

            return tx;
        }
    }
}
