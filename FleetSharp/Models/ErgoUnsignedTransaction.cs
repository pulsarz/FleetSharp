using Blake2Fast;
using FleetSharp.Sigma;
using FleetSharp.Types;
using FleetSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FleetSharp.Models
{
    public class ErgoUnsignedTransaction
    {
        private List<ErgoUnsignedInput> _inputs { get; set; }
        private List<ErgoUnsignedInput> _dataInputs { get; set; }
        private List<BoxCandidate<long>> _outputs { get; set; }

        public ErgoUnsignedTransaction(IEnumerable<ErgoUnsignedInput>  inputs, IEnumerable<ErgoUnsignedInput> dataInputs, IEnumerable<BoxCandidate<long>> outputs)
        {
            _inputs = inputs.ToList().AsReadOnly().ToList();
            _dataInputs = dataInputs.ToList().AsReadOnly().ToList();
            _outputs = outputs.ToList().AsReadOnly().ToList();
        }

        public string id()
        {
            return Tools.BytesToHex(Blake2b.ComputeHash(32, toBytes()));
        }

        public List<ErgoUnsignedInput> inputs()
        {
            return _inputs;
        }

        public List<ErgoUnsignedInput> dataInputs()
        {
            return _dataInputs;
        }

        public List<BoxCandidate<long>> outputs()
        {
            return _outputs;
        }

        public BoxAmounts burning()
        {
            return BoxUtils.UtxoSumResultDiff(BoxUtils.UtxoSum(inputs().Select(x => new MinimalBoxAmounts { value = x.value, assets = x.assets.Select(y => new TokenAmount<long> { tokenId = y.tokenId, amount = y.amount }).ToList() }))
                , BoxUtils.UtxoSum(outputs().Select(x => new MinimalBoxAmounts { value = x.value, assets = x.assets.Select(y => new TokenAmount<long> { tokenId = y.tokenId, amount = y.amount }).ToList() })));
        }

        public dynamic ToPlainObject() => ToPlainObject(BuildOutputType.Default);

        public dynamic ToPlainObject(BuildOutputType? outputType = null)
        {
            if (outputType.Equals(BuildOutputType.EIP12))
            {
                return new EIP12UnsignedTransaction
                {
                    inputs = inputs().Select(x => (EIP12UnsignedInput)x.toUnsignedInputObject(BuildOutputType.EIP12)).ToList(),
                    dataInputs = dataInputs().Select(x => (EIP12UnsignedDataInput)x.toPlainObject(BuildOutputType.EIP12)).ToList(),
                    outputs = outputs().Select(x => new BoxCandidate<string>
					{
						additionalRegisters = x.additionalRegisters,
						creationHeight = x.creationHeight,
						ergoTree = x.ergoTree,
						value = x.value.ToString(),
						assets = x.assets.Select(y => new TokenAmount<string> { tokenId = y.tokenId, amount = y.amount.ToString() }).ToList()
					}).ToList()
			};
            }
            else
            {
                return new UnsignedTransaction
                {
                    inputs = inputs().Select(x => (UnsignedInput)x.toUnsignedInputObject(outputType ?? BuildOutputType.Default)).ToList(),
                    dataInputs = dataInputs().Select(x => (DataInput)x.toPlainObject(outputType ?? BuildOutputType.Default)).ToList(),
                    outputs = outputs()
                };
            }
        }

        public EIP12UnsignedTransaction ToEIP12Object()
        {
            return ToPlainObject(BuildOutputType.EIP12);
        }

        public byte[] toBytes()
        {
            return TransactionSerializer.serializeTransaction(new MinimalUnsignedTransaction {
                inputs = inputs().Select(input => (UnsignedInput)input.toUnsignedInputObject(BuildOutputType.Default)).ToList(),
                dataInputs = dataInputs().Select(dataInput => (DataInput)dataInput.toPlainObject(BuildOutputType.Default)).ToList(),
                outputs = outputs()
            }).toBytes();
        }
    }
}
