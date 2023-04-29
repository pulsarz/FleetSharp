using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Types
{
    public class ProverResult
    {
        public string proofBytes { get; }
        public ContextExtension extension { get; }

        public ProverResult(string proofBytes, ContextExtension extension)
        {
            this.proofBytes = proofBytes;
            this.extension = extension;
        }
    }
}
