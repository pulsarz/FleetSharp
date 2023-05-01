using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Utils
{
    public class MinimalBoxAmounts
    {
        public long value { get; set; }
        public List<TokenAmount<long>> assets { get; set; }
    }

    public class BoxAmounts
    {
        public long nanoErgs { get; set; }
        public List<TokenAmount<long>> tokens { get; set; }
    }

    internal static class BoxUtils
    {
        private const string NANOERGS_TOKEN_ID = "nanoErgs";

        public static dynamic UtxoSum(IEnumerable<MinimalBoxAmounts> boxes)
        {
            return UtxoSum(boxes, null);
        }

        public static dynamic UtxoSum(IEnumerable<MinimalBoxAmounts> boxes, string? tokenId = null)
        {
            var balances = new Dictionary<string, long>();

            foreach (var box in boxes)
            {
                if (tokenId == null || tokenId == NANOERGS_TOKEN_ID)
                {
                    balances[NANOERGS_TOKEN_ID] = balances.GetValueOrDefault(NANOERGS_TOKEN_ID) + box.value;
                }

                if (tokenId != NANOERGS_TOKEN_ID)
                {
                    foreach (var token in box.assets)
                    {
                        if (tokenId != null && tokenId != token.tokenId)
                        {
                            continue;
                        }

                        balances[token.tokenId] = balances.GetValueOrDefault(token.tokenId) + token.amount;
                    }
                }
            }

            if (tokenId != null)
            {
                return balances.GetValueOrDefault(tokenId);
            }

            return new BoxAmounts
            {
                nanoErgs = balances.GetValueOrDefault(NANOERGS_TOKEN_ID),
                tokens = balances.Where(kvp => kvp.Key != NANOERGS_TOKEN_ID).Select(kvp => new TokenAmount<long>
                {
                    tokenId = kvp.Key,
                    amount = kvp.Value
                }).ToList()
            };
        }

        public static BoxAmounts UtxoSumResultDiff(BoxAmounts amountsA, BoxAmounts amountsB)
        {
            var tokens = new List<TokenAmount<long>>();
            var nanoErgs = amountsA.nanoErgs - amountsB.nanoErgs;

            foreach (var token in amountsA.tokens)
            {
                var balance = token.amount - (amountsB.tokens.Find(t => t.tokenId == token.tokenId)?.amount ?? 0);

                if (balance != 0)
                {
                    tokens.Add(new TokenAmount<long>
                    {
                        tokenId = token.tokenId,
                        amount = balance
                    });
                }
            }

            return new BoxAmounts
            {
                nanoErgs = nanoErgs,
                tokens = tokens
            };
        }
    }
}
