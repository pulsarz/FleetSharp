using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException(string boxId)
            : base($"Invalid input: {boxId}")
        {
        }
    }
}
