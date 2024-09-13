using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Interface
{
    public interface IExplorer
    {
        public Box<long> ConvertExplorerBoxToFleetBox(ExplorerBox box);
        public Task<Box<long>?> GetBoxById(string boxId);
        public Task<List<Box<long>>?> GetUnspentBoxesByErgoTree(string ergoTree, int chunkSize);
        public Task<List<Box<long>>?> GetUnspentBoxesByTokenId(string tokenId, int chunkSize);
        public Task<TokenDetail<long>?> GetTokenById(string tokenId);
        public Task<NodeBalance<long>?> GetAddressBalance(string address);
        public Task<List<NodeBalance<long>>> GetAddressesBalances(List<string> addresses);
    }
}
