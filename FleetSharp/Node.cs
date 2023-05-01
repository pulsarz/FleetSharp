﻿using FleetSharp.Models;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FleetSharp
{
    public class NodeInterface
    {
        private static HttpClient client = new HttpClient();

        private string nodeURL;
        private string apiKey;

        public NodeInterface(string nodeURL, string? apiKey = null) {
            this.nodeURL = nodeURL;
            this.apiKey = apiKey;

            //add api key
            if (apiKey != null) client.DefaultRequestHeaders.Add("api_key", apiKey);
        }

        //take should not be hiogher then 100
        private async Task<List<NodeMempoolTransaction>?> _GetMempool(int offset, int take)
        {
            List<NodeMempoolTransaction>? mempool = await client.GetFromJsonAsync<List<NodeMempoolTransaction>>($"{this.nodeURL}/transactions/unconfirmed?limit={take}&offset={offset}");
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

        public async Task<NodeMempoolTransaction?> GetTX(string? txId)
        {
            NodeMempoolTransaction? tx = null;
            if (txId == null) return null;

            tx = await client.GetFromJsonAsync<NodeMempoolTransaction>($"{this.nodeURL}/blockchain/transaction/byId/{txId}");

            return tx;
        }

        //Tries to find unspent + mempool first, if not found tries spent ones.
        public async Task<Box<long>?> GetBox(string? boxId)
        {
            Box<long>? box = null;
            if (boxId == null) return null;

            try
            {
                box = await client.GetFromJsonAsync<Box<long>>($"{this.nodeURL}/utxo/withPool/byId/{boxId}");
            }
            catch (Exception e)
            {

            }

            if (box == null)
            {
                //Retrieve box from blockchain indexer instead
                box = await client.GetFromJsonAsync<Box<long>>($"{this.nodeURL}/blockchain/box/byId/{boxId}");
            }

            return box;
        }
        public async Task<List<Box<long>?>> GetBoxes(List<string> boxIds)
        {
            var taskList = new List<Task<Box<long>?>>();

            foreach (var boxId in boxIds)
            {
                taskList.Add(GetBox(boxId));
            }

            var result = await Task.WhenAll(taskList.ToList()).ConfigureAwait(false);
            return result.ToList();
        }

        public async Task<List<Box<long>>> GetUnspentBoxesByErgoTree(string ergoTree)
        {
            List<Box<long>> boxes = new List<Box<long>>();
            List<Box<long>>? temp = null;
            int limit = 1000;

            if (ergoTree == null) return null;

            do
            {
                //temp = await client.GetFromJsonAsync<List<NodeBox>>($"{this.nodeURL}/blockchain/box/unspent/byErgoTree/{ergoTree}?offset={boxes.Count}&limit={limit}", jsonOptions);
                temp = null;
                //var response = await client.PostAsJsonAsync($"{this.nodeURL}/blockchain/box/unspent/byErgoTree?offset={boxes.Count}&limit={limit}", ergoTree);
                var response = await client.PostAsync($"{this.nodeURL}/blockchain/box/unspent/byErgoTree?offset={boxes.Count}&limit={limit}", new StringContent(ergoTree, Encoding.UTF8, "application/json"));
                var content = await response.Content.ReadAsStringAsync();
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
                taskList.Add(GetUnspentBoxesByErgoTree(ergoTree));
            }

            var result = await Task.WhenAll(taskList.ToList()).ConfigureAwait(false);
            foreach (var unspent in result)
            {
                if (unspent != null) boxes.AddRange(unspent);
            }

            return boxes;
        }

        public async Task<TokenDetail<long>?> GetToken(string? tokenId)
        {
            TokenDetail<long>? token = null;
            if (tokenId == null) return null;

            token = await client.GetFromJsonAsync<TokenDetail<long>>($"{this.nodeURL}/blockchain/token/byId/{tokenId}");

            return token;
        }

        public async Task<NodeBalance<long>?> GetAddressBalance(string? address)
        {
            NodeBalance<long>? balance = null;

            if (address == null) return null;

            var response = await client.PostAsync($"{this.nodeURL}/blockchain/balance", new StringContent(address, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
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
                taskList.Add(GetAddressBalance(address));
            }

            var result = await Task.WhenAll(taskList.ToList()).ConfigureAwait(false);
            return result.ToList();
        }
        /*
        public async Task<string?> ErgoTreeToAddress(string? ergoTree)
        {
            if (ergoTree == null) return null;
            NodeErgoTreeToAddress? address = await client.GetFromJsonAsync<NodeErgoTreeToAddress>($"{this.nodeURL}/utils/ergoTreeToAddress/{ergoTree}");
            return address.address;
        }*/

        public async Task<NodeIndexedHeight?> GetIndexedHeight()
        {
            return await client.GetFromJsonAsync<NodeIndexedHeight>($"{this.nodeURL}/blockchain/indexedHeight");
        }

        public async Task<bool> IsValidAddress(string? address)
        {
            if (address == null) return false;

            var response = await client.PostAsJsonAsync($"{this.nodeURL}/utils/address", address);
            var content = await response.Content.ReadAsStringAsync();
            if (content == null || content == "") return false;
            IsValidAddress? valid = JsonSerializer.Deserialize<IsValidAddress>(content);
            if (valid == null) return false;
            return valid.isValid;
        }

        //Sign an arbitrary transaction
        public async Task<SignedTransaction?> SignTransaction(ErgoUnsignedTransaction tx)
        {
            SignTXWrapper wrapper = new SignTXWrapper() { tx = tx };

            var response = await client.PostAsJsonAsync($"{this.nodeURL}/wallet/walletTransactionSign", wrapper, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
            var content = await response.Content.ReadAsStringAsync();
            if (content == null || content == "") return null;

            SignedTransaction? signed = JsonSerializer.Deserialize<SignedTransaction>(content);
            return signed;
        }

        public async Task<NodeMempoolTransaction?> FillMissingInfoMempoolTX(NodeMempoolTransaction? tx)
        {
            if (tx == null) return null;
            //get input boxes
            /*for (var i = 0; i < tx.inputs?.Count; i++)
            {
                if (tx.inputs[i] != null)
                {
                    Box<long>? tempBox = await GetBox(tx.inputs[i]?.boxId);
                    tx.inputs[i] = tempBox;
                }
            }*/

            var inputBoxIds = tx.inputs.Select(x => x.boxId).Where(x => x != null).ToList();
            var inputBoxes = await GetBoxes(inputBoxIds);

            tx.inputs = tx.inputs.Where(x => !inputBoxes.Exists(y => y.boxId == x.boxId)).ToList();
            foreach (var tempBox in inputBoxes)
            {
                if (tempBox != null) tx.inputs.Add(tempBox);
            }

            /*
            //get input addresses from ergotree
            for (var i = 0; i < tx.inputs?.Count; i++)
            {
                if (tx.inputs[i] != null)
                {
                    tx.inputs[i].address = ErgoAddress.fromErgoTree(tx.inputs[i]?.ergoTree, Network.Mainnet).encode(Network.Mainnet);
                }
            }

            //get output addresses from ergotree
            for (var i = 0; i < tx.outputs?.Count; i++)
            {
                if (tx.outputs[i] != null)
                {
                    tx.outputs[i].address = ErgoAddress.fromErgoTree(tx.outputs[i]?.ergoTree, Network.Mainnet).encode(Network.Mainnet);
                }
            }*/

            return tx;
        }
    }
}
