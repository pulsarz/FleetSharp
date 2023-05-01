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
        public Dictionary<int, string?> extension { get; set; }

        public ProverResult(string proofBytes, Dictionary<int, string?> extension)
        {
            this.proofBytes = proofBytes;
            this.extension = extension;
        }
    }
}
