using FleetSharp.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class MaxTokensOverflowException : Exception
    {
        public MaxTokensOverflowException()
            : base($"A box must contain no more than {TransactionBuilderSettings.MAX_TOKENS_PER_BOX} distinct tokens.")
        {
        }
    }
}
