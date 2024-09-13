using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder.Interface
{
    public interface ITransactionBuilderSettings
    {
        public int maxTokensPerChangeBox();
        public bool canBurnTokens();
        public bool shouldIsolateErgOnChange();
        public TransactionBuilderSettings setMaxTokensPerChangeBox(int max);
        public TransactionBuilderSettings allowTokenBurning(bool allow);
        public TransactionBuilderSettings isolateErgOnChange(bool isolate);
    }
}
