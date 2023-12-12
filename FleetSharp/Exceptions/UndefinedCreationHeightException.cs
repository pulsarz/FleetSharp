using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Exceptions
{
    public class UndefinedCreationHeightException : Exception
    {
        public UndefinedCreationHeightException()
            : base("Creation Height is undefined.")
        {
        }
    }
}
