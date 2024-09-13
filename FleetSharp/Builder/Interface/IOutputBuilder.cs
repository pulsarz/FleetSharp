using FleetSharp.Models;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder.Interface
{
    public interface IOutputBuilder
    {
        public long estimateMinBoxValue(long valuePerByte);
        public long GetValue();
        public ErgoAddress GetAddress();
        public string GetErgoTree();
        public long? GetCreationHeight();
        public List<TokenAmount<long>> GetAssets();
        public NonMandatoryRegisters? GetAdditionalRegisters();
        public NewToken<long>? minting();
        public OutputBuilder SetValue(long value);
        public OutputBuilder AddToken(TokenAmount<long> token, bool sum);
        public OutputBuilder AddTokens(List<TokenAmount<long>> tokens, bool sum);
        public OutputBuilder RemoveTokens(string tokenId);
        public OutputBuilder RemoveTokens(List<string> tokenIds);
        public OutputBuilder mintToken(NewToken<long> token);
        public OutputBuilder SetCreationHeight(long height, bool replace);
        public OutputBuilder SetAdditionalRegisters(NonMandatoryRegisters registers);
        public BoxCandidate<long> build(List<ErgoUnsignedInput>? transactionInputs);
    }
}
