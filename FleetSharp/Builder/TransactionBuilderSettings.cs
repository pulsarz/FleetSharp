using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder
{
    public class TransactionBuilderSettings
    {
        public static int MAX_TOKENS_PER_BOX = 120;

        private int _maxDistinctTokensPerChangeBox { get; set; }
        private bool _allowTokenBurning { get; set; }
        private bool _isolateErgOnChange { get; set; }

        public TransactionBuilderSettings()
        {
            _maxDistinctTokensPerChangeBox = MAX_TOKENS_PER_BOX;
            _allowTokenBurning = false;
            _isolateErgOnChange = false;
        }

        public int maxTokensPerChangeBox()
        {
            return _maxDistinctTokensPerChangeBox;
        }

        public bool canBurnTokens()
        {
            return _allowTokenBurning;
        }

        public bool shouldIsolateErgOnChange()
        {
            return _isolateErgOnChange;
        }

        public TransactionBuilderSettings setMaxTokensPerChangeBox(int max)
        {
            _maxDistinctTokensPerChangeBox = max;
            return this;
        }

        public TransactionBuilderSettings allowTokenBurning(bool allow = true)
        {
            _allowTokenBurning = allow;
            return this;
        }

        public TransactionBuilderSettings isolateErgOnChange(bool isolate = true)
        {
            _isolateErgOnChange = isolate;
            return this;
        }
    }
}
