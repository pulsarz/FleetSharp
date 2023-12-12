using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class NotAllowedTokenBurningException : Exception
    {
        public NotAllowedTokenBurningException()
            : base("This transaction is trying to burn tokens. If that's your intention you must explicitly allow token burning on TransactionBuilder.configure() method. If no, a change address should be provided.")
        {
        }
    }
}
