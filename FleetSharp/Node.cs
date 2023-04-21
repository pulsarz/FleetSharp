using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FleetSharp
{
    public class NodeInterface
    {
        private static HttpClient client = new HttpClient();

        public string nodeURL;

        public NodeInterface(string nodeURL) {
            this.nodeURL = nodeURL;
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

        public async Task<NodeBox?> GetBox(string? boxId)
        {
            NodeBox? box = null;
            if (boxId == null) return null;

            box = await client.GetFromJsonAsync<NodeBox>($"{this.nodeURL}/utxo/withPool/byId/{boxId}");

            return box;
        }

        public async Task<List<NodeBox>?> GetUnspentBoxesByErgoTree(string? ergoTree)
        {
            List<NodeBox>? boxes = new List<NodeBox>();
            List<NodeBox>? temp = null;
            int limit = 1000;

            if (ergoTree == null) return null;


            do
            {
                temp = await client.GetFromJsonAsync<List<NodeBox>>($"{this.nodeURL}/blockchain/box/unspent/byErgoTree/{ergoTree}?offset={boxes.Count}&limit={limit}");

                if (temp != null)
                {
                    boxes.AddRange(temp);
                }
            }
            while (temp != null && temp?.Count == limit);

            return boxes;
        }

        public async Task<NodeToken?> GetToken(string? tokenId)
        {
            NodeToken? token = null;
            if (tokenId == null) return null;

            token = await client.GetFromJsonAsync<NodeToken>($"{this.nodeURL}/blockchain/token/byId/{tokenId}");

            return token;
        }

        public async Task<string?> ErgoTreeToAddress(string? ergoTree)
        {
            if (ergoTree == null) return null;
            NodeErgoTreeToAddress? address = await client.GetFromJsonAsync<NodeErgoTreeToAddress>($"{this.nodeURL}/utils/ergoTreeToAddress/{ergoTree}");
            return address.address;
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

        public async Task<NodeMempoolTransaction?> FillMissingInfoMempoolTX(NodeMempoolTransaction? tx)
        {
            if (tx == null) return null;
            //get input boxes
            for (var i = 0; i < tx.inputs?.Count; i++)
            {
                if (tx.inputs[i] != null)
                {
                    NodeBox? tempBox = await GetBox(tx.inputs[i]?.boxId);
                    tx.inputs[i] = tempBox;
                }
            }

            //get input addresses from ergotree
            for (var i = 0; i < tx.inputs?.Count; i++)
            {
                if (tx.inputs[i] != null)
                {
                    string? address = await ErgoTreeToAddress(tx.inputs[i]?.ergoTree);
                    if (address != null) tx.inputs[i].address = address;
                }
            }

            //get output addresses from ergotree
            for (var i = 0; i < tx.outputs?.Count; i++)
            {
                if (tx.outputs[i] != null)
                {
                    string? address = await ErgoTreeToAddress(tx.outputs[i]?.ergoTree);
                    if (address != null) tx.outputs[i].address = address;
                }
            }

            return tx;
        }
    }
}
