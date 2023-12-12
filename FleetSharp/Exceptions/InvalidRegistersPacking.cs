using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class InvalidRegistersPackingException : Exception
    {
        public InvalidRegistersPackingException()
            : base("Registers should be densely packed. This means that it's not possible to use a register like 'R7' without filling 'R6', 'R5', and 'R4', for example.")
        {
        }
    }
}
