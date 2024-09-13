using FleetSharp.Builder.Selector;
using FleetSharp.Models;
using FleetSharp.Types;
using FleetSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder.Interface
{
    public interface ITransactionBuilder
    {
        public List<ErgoUnsignedInput> inputs();
        public List<string> inputsForcedInclusion();
        public List<ErgoUnsignedInput> dataInputs();
        public List<OutputBuilder> outputs();
        public ErgoAddress? changeAddress();
        public long? fee();
        public List<TokenAmount<long>>? burning();
        public TransactionBuilderSettings? settings();
        public long creationHeight();
        public TransactionBuilder clearInputsExclForced();
        public TransactionBuilder and();
        public TransactionBuilder from(List<ErgoUnsignedInput> inputs);
        public TransactionBuilder from(List<Box<long>> inputs);
        public TransactionBuilder fromForcedInclusion(List<ErgoUnsignedInput> inputs);
        public TransactionBuilder fromForcedInclusion(List<Box<long>> inputs);
        public TransactionBuilder to(List<OutputBuilder> outputs);
        public TransactionBuilder withDataFrom(List<ErgoUnsignedInput> dataInputs);
        public TransactionBuilder withDataFrom(List<Box<long>> dataInputs);
        public TransactionBuilder sendChangeTo(ErgoAddress address);
        public TransactionBuilder payFee(long amount);
        public TransactionBuilder payMinFee();
        public TransactionBuilder burnTokens(List<TokenAmount<long>> tokens);
        public BoxAmounts OutputSum(List<OutputBuilder> outputs, SelectionTarget<long>? basis);
        public long estimateChangeSize(ChangeEstimationParams parm);
        public long estimateMinChangeValue(ChangeEstimationParams parm);
        public ErgoUnsignedTransaction build();

    }
}
