using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FleetSharp.Types
{
    public class TokenBase<AmountType>
    {
        public AmountType amount { get; set; }
    }

    public class TokenAmount<AmountType> : TokenBase<AmountType>
    {
        public string tokenId { get; set; }
    }

    public class NewToken<AmountType> : TokenBase<AmountType>
    {
        public string? tokenId { get; set; }
        public string? name { get; set; }
        public int? decimals { get; set; }
        public string? description { get; set; }
    }

    public class BalanceToken<AmountType> : TokenBase<AmountType>
    {
        public string? tokenId { get; set; }
        public string? name { get; set; }
        public int? decimals { get; set; }
    }

    public class TokenDetail<AmountType>
    {
        public string? id { get; set; }
        public string? boxId { get; set; }
        public AmountType emissionAmount { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public int decimals { get; set; }
    }

    public class TokenTargetAmount<AmountType>
    {
        public string tokenId { get; set; }
        public AmountType? amount { get; set; }
    }
}
