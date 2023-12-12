using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class UndefinedMintingContextException : Exception
    {
        public UndefinedMintingContextException()
            : base("Minting context is undefined. Transaction's inputs must be included in order to determine minting token id.")
        {
        }
    }
}
