using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class DuplicateInputException : Exception
    {
        public DuplicateInputException() : base()
        {
        }

        public DuplicateInputException(string boxId) : base($"Box '{boxId}' is already included.")
        {
        }
    }
}
