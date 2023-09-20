using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FleetSharp
{
	public class Explorer
	{
		private string _url;
		private HttpClient _client = new HttpClient();

		public Explorer(string url = "https://api.ergoplatform.com/api/v1")
		{
			_url = url;
		}

		public Box<long> ConvertExplorerBoxToFleetBox(ExplorerBox box)
		{
			return new Box<long>
			{
				boxId = box.boxId,
				ergoTree = box.ergoTree,
				index = box.index ?? 0,
				creationHeight = box.creationHeight ?? 0,
				transactionId = box.transactionId,
				value = box.value ?? 0,
				additionalRegisters = new NonMandatoryRegisters
				{
					R4 = box.additionalRegisters?.R4?.serializedValue,
					R5 = box.additionalRegisters?.R5?.serializedValue,
					R6 = box.additionalRegisters?.R6?.serializedValue,
					R7 = box.additionalRegisters?.R7?.serializedValue,
					R8 = box.additionalRegisters?.R8?.serializedValue,
					R9 = box.additionalRegisters?.R9?.serializedValue,
				},
				assets = box.assets?.Select(x => new TokenAmount<long> { tokenId = x.tokenId, amount = x.amount ?? 0 }).ToList()
			};
		}

		public async Task<Box<long>?> GetBoxById(string boxId)
		{
			var box = await _client.GetFromJsonAsync<ExplorerBox>($"{_url}/boxes/{boxId}");
			if (box == null) return null;

			return ConvertExplorerBoxToFleetBox(box);
		}

		public async Task<List<Box<long>>?> GetUnspentBoxesByTokenId(string tokenId)
		{
			var wrapper = await _client.GetFromJsonAsync<ExplorerBoxexWrapper>($"{_url}/boxes/unspent/byTokenId/{tokenId}");
			if (wrapper == null) return null;

			return wrapper.items.Where(x => x != null).Select(x => ConvertExplorerBoxToFleetBox(x)).ToList();
		}

		public async Task<TokenDetail<long>?> GetTokenById(string tokenId)
		{
			return await _client.GetFromJsonAsync<TokenDetail<long>>($"{_url}/tokens/{tokenId}");
		}

        public async Task<NodeBalance<long>?> GetAddressBalance(string address)
        {
            return await _client.GetFromJsonAsync<NodeBalance<long>>($"{_url}/addresses/{address}/balance/total");
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
    }
}
