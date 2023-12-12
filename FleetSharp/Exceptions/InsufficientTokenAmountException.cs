using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class InsufficientTokenAmountException : Exception
    {
        public InsufficientTokenAmountException(string message)
            : base(message)
        {
        }
    }
}
