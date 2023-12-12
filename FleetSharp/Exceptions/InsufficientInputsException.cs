using FleetSharp.Builder.Selector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class InsufficientInputsException : Exception
    {
        public SelectionTarget<long> Unreached { get; }

        public InsufficientInputsException(SelectionTarget<long> unreached)
            : base($"Insufficient inputs:{BuildString(unreached)}")
        {
            Unreached = unreached;
        }

        private static string BuildString(SelectionTarget<long> unreached)
        {
            var strings = new List<string>();

            if (unreached.nanoErgs != 0)
            {
                strings.Add(BuildString("nanoErgs", unreached.nanoErgs));
            }

            if (unreached.tokens?.Any() == true)
            {
                foreach (var token in unreached.tokens)
                {
                    strings.Add(BuildString(token.tokenId, token.amount));
                }
            }

            return string.Join("\n", strings.Select(s => $"  > {s}"));
        }

        private static string BuildString(string tokenId, long? amount)
        {
            return $"{tokenId}: {amount?.ToString()}";
        }
    }
}
