using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class InvalidAddressException : Exception
    {
        public InvalidAddressException(string address)
            : base($"Invalid Ergo address: {address}")
        {
        }
    }
}
