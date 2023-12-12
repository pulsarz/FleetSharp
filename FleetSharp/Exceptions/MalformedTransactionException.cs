using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class MalformedTransactionException : Exception
    {
        public MalformedTransactionException(string message)
            : base($"Malformed transaction: {message}")
        {
        }
    }
}
