using System;
using System.Collections.Generic;
using System.Text;

namespace FleetSharp.Types
{
    public enum Network
    {
        Mainnet = 0 << 4,
        Testnet = 1 << 4
    }

    public enum AddressType
    {
        P2PK = 1,
        P2SH = 2,
        P2S = 3
    }
}
