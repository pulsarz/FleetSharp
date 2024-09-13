using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Interface
{
    public interface IErgoAddress
    {
        public Network? GetNetworkType();
        public byte[][] getPublicKeys();
        public byte[] GetErgoTree();
        public string GetErgoTreeHex();
        public string encode(Network? network);
        public string ToString(Network? network);
    }
}
