using FleetSharp.Builder.Selector;
using FleetSharp.Models;
using FleetSharp.Sigma;
using FleetSharp.Types;
using FleetSharp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder
{
    public class ChangeEstimationParams
    {
        public ErgoAddress changeAddress { get; set; }
        public long creationHeight { get; set; }
        public List<TokenAmount<long>> tokens { get; set; }
        public int baseIndex { get; set; }
        public int maxTokensPerBox { get; set; }
    }
    public class TransactionBuilder
    {
        public const long RECOMMENDED_MIN_FEE_VALUE = 1100000;
        public const string FEE_CONTRACT = "1005040004000e36100204a00b08cd0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798ea02d192a39a8cc7a701730073011001020402d19683030193a38cc7b2a57300000193c2b2a57301007473027303830108cdeeac93b1a57304";

        //should be ErgoUnsignedInput or Box<Amount>
        private List<ErgoUnsignedInput> _inputs { get; set; }
        private List<string> _inputsForcedInclusionBoxIds { get; set; }
        private List<ErgoUnsignedInput> _dataInputs { get; set; }
        private List<OutputBuilder> _outputs { get; set; }
        private TransactionBuilderSettings _settings { get; set; }
        private long _creationHeight { get; set; }

        private ErgoAddress? _changeAddress { get; set; }
        private long? _feeAmount { get; set; }
        private List<TokenAmount<long>>? _burning { get; set; }

        public TransactionBuilder(long creationHeight)
        {
            _inputs = new List<ErgoUnsignedInput>();
            _inputsForcedInclusionBoxIds = new List<string>();
            _dataInputs = new List<ErgoUnsignedInput>();
            _outputs = new List<OutputBuilder>();
            _settings = new TransactionBuilderSettings();
            _creationHeight = creationHeight;
        }

        public List<ErgoUnsignedInput> inputs()
        {
            return _inputs;
        }
        public List<string> inputsForcedInclusion()
        {
            return _inputsForcedInclusionBoxIds;
        }
        public List<ErgoUnsignedInput> dataInputs()
        {
            return _dataInputs;
        }
        public List<OutputBuilder> outputs()
        {
            return _outputs;
        }
        public ErgoAddress? changeAddress()
        {
            return _changeAddress;
        }
        public long? fee()
        {
            return _feeAmount;
        }
        public List<TokenAmount<long>>? burning()
        {
            return _burning;
        }
        public TransactionBuilderSettings? settings()
        {
            return _settings;
        }
        public long creationHeight()
        {
            return _creationHeight;
        }

        /**
       * Syntax sugar to be used in composition with another methods
       *
       * @example
       * ```
       * new TransactionBuilder(height)
       *   .from(inputs)
       *   .and().from(otherInputs);
       * ```
       */

        //Removed all inputs except those specified in the forced inclusion
        public TransactionBuilder clearInputsExclForced()
        {
            _inputs = _inputs.Where(x => _inputsForcedInclusionBoxIds.Exists(y => y == x.boxId)).ToList();

            return this;
        }
        public TransactionBuilder and()
        {
            return this;
        }

        public TransactionBuilder from(List<ErgoUnsignedInput> inputs)
        {
            _inputs.AddRange(inputs);

            return this;
        }

        public TransactionBuilder from(List<Box<long>> inputs)
        {
            _inputs.AddRange(inputs.Select(x => new ErgoUnsignedInput(new InputBox { boxId = x.boxId, ergoTree = x.ergoTree, additionalRegisters = x.additionalRegisters, assets = x.assets, creationHeight = x.creationHeight, index = x.index, transactionId = x.transactionId, value = x.value })));

            return this;
        }

        //Boxes added through this method will always be included in the input selection
        public TransactionBuilder fromForcedInclusion(List<ErgoUnsignedInput> inputs)
        {
            from(inputs.Where(x => !_inputs.Exists(y => y.boxId == x.boxId)).ToList());//Only insert if not inserted already
            _inputsForcedInclusionBoxIds.AddRange(inputs.Select(x => x.boxId));

            return this;
        }

        public TransactionBuilder fromForcedInclusion(List<Box<long>> inputs)
        {
            from(inputs.Where(x => !_inputs.Exists(y => y.boxId == x.boxId)).ToList());//Only insert if not inserted already
            _inputsForcedInclusionBoxIds.AddRange(inputs.Select(x => x.boxId));

            return this;
        }

        public TransactionBuilder to(List<OutputBuilder> outputs)
        {
            _outputs.AddRange(outputs);

            return this;
        }

        public TransactionBuilder withDataFrom(List<ErgoUnsignedInput> dataInputs)
        {
            _dataInputs.AddRange(dataInputs);

            return this;
        }

        public TransactionBuilder withDataFrom(List<Box<long>> dataInputs)
        {
            _dataInputs.AddRange(dataInputs.Select(x => new ErgoUnsignedInput(new InputBox { boxId = x.boxId, ergoTree = x.ergoTree, additionalRegisters = x.additionalRegisters, assets = x.assets, creationHeight = x.creationHeight, index = x.index, transactionId = x.transactionId, value = x.value })));

            return this;
        }

        public TransactionBuilder sendChangeTo(ErgoAddress address)
        {
            _changeAddress = address;

            return this;
        }

        public TransactionBuilder payFee(long amount)
        {
            _feeAmount = amount;

            return this;
        }

        public TransactionBuilder payMinFee()
        {
            payFee(RECOMMENDED_MIN_FEE_VALUE);

            return this;
        }

        public TransactionBuilder burnTokens(List<TokenAmount<long>> tokens)
        {
            if (_burning == null) _burning = new List<TokenAmount<long>>();
            _burning.AddRange(tokens);

            return this;
        }



        private bool _isMinting()
        {
            foreach(var output in _outputs)
            {
                if (output.minting() != null) return true;
            }

            return false;
        }

        private bool _isMoreThanOneTokenBeingMinted()
        {
            var mintingCount = 0;

            foreach (var output in _outputs)
            {
                if (output.minting() != null)
                {
                    mintingCount++;

                    if (mintingCount > 1) return true;
                }
            }

            return false;
        }

        private string? _getMintingTokenId()
        {
            string tokenId = null;

            foreach (var output in _outputs)
            {
                if (output.minting() != null)
                {
                    tokenId = output.minting().tokenId;
                    break;
                }
            }

            return tokenId;
        }

        private bool _isTheSameTokenBeingMintedOutsideTheMintingBox()
        {
            var mintingTokenId = _getMintingTokenId();

            if (mintingTokenId == null) return false;

            foreach (var output in _outputs)
            {
                if (output.GetAssets().Exists(x => x.tokenId == mintingTokenId)) return true;
            }

            return false;
        }

        public BoxAmounts OutputSum(List<OutputBuilder> outputs, SelectionTarget<long>? basis = null)
        {
            Dictionary<string, long> tokens = new Dictionary<string, long>();
            long nanoErgs = 0;

            if (basis != null)
            {
                if (basis.nanoErgs != null)
                {
                    nanoErgs = basis.nanoErgs;
                }

                if (basis.tokens != null)
                {
                    foreach (var token in basis.tokens)
                    {
                        if (token.amount == null)
                        {
                            continue;
                        }

                        if (tokens.TryGetValue(token.tokenId, out var existingAmount))
                        {
                            tokens[token.tokenId] = existingAmount + token.amount;
                        }
                        else
                        {
                            tokens[token.tokenId] = token.amount;
                        }
                    }
                }
            }

            foreach (var box in outputs)
            {
                nanoErgs += box.GetValue();
                foreach (var token in box.GetAssets())
                {
                    if (tokens.TryGetValue(token.tokenId, out var existingAmount))
                    {
                        tokens[token.tokenId] = existingAmount + token.amount;
                    }
                    else
                    {
                        tokens[token.tokenId] = token.amount;
                    }
                }
            }

            return new BoxAmounts
            {
                nanoErgs = nanoErgs,
                tokens = tokens.Select(token => new TokenAmount<long> { tokenId = token.Key, amount = token.Value }).ToList()
            };
        }

        public long estimateChangeSize(ChangeEstimationParams parm)
        {
            var neededBoxes = (int)Math.Ceiling(parm.tokens.Count / Convert.ToDouble(parm.maxTokensPerBox));
            var size = 0;
            size += (int)VLQ.EstimateVlqSize(OutputBuilder.SAFE_MIN_BOX_VALUE);
            size += Tools.HexByteSize(parm.changeAddress.GetErgoTreeHex());
            size += (int)VLQ.EstimateVlqSize(parm.creationHeight);
            size += (int)VLQ.EstimateVlqSize(0);//empty registers length
            size += 32;//BLAKE_256_HASH_LENGTH

            size = size * neededBoxes;
            for (var i = 0; i < neededBoxes; i++)
            {
                size += (int)VLQ.EstimateVlqSize(parm.baseIndex + i);
            }

            size += parm.tokens.Aggregate(0, (acc, curr) => acc += Tools.HexByteSize(curr.tokenId) + (int)VLQ.EstimateVlqSize(curr.amount));

            if (parm.tokens.Count > parm.maxTokensPerBox)
            {
                if (parm.tokens.Count % parm.maxTokensPerBox > 0)
                {
                    size += (int)VLQ.EstimateVlqSize(parm.maxTokensPerBox) * (parm.tokens.Count / parm.maxTokensPerBox);
                    size += (int)VLQ.EstimateVlqSize(parm.tokens.Count % parm.maxTokensPerBox);
                }
                else
                {
                    size += (int)VLQ.EstimateVlqSize(parm.maxTokensPerBox) * neededBoxes;
                }
            }
            else
            {
                size += (int)VLQ.EstimateVlqSize(parm.tokens.Count);
            }

            return size;
        }

        public long estimateMinChangeValue(ChangeEstimationParams parm)
        {
            var size = estimateChangeSize(parm);
            return (size * OutputBuilder.BOX_VALUE_PER_BYTE);
        }

        public ErgoUnsignedTransaction build()
        {
            if (_isMinting())
            {
                if (_isMoreThanOneTokenBeingMinted())
                {
                    throw new InvalidOperationException("Only one token can be minted per transaction.");
                }

                if (_isTheSameTokenBeingMintedOutsideTheMintingBox())
                {
                    throw new InvalidOperationException("EIP-4 tokens cannot be minted from outside of the minting box.");
                }
            }

            _outputs.ForEach(output =>
            {
                output.SetCreationHeight(_creationHeight, false);
            });

            var outputsLocal = new List<OutputBuilder>(_outputs.ToArray());//clone

            if (_feeAmount != null)
            {
                outputsLocal.Add(new OutputBuilder(_feeAmount ?? 0, ErgoAddress.fromErgoTree(FEE_CONTRACT, Network.Mainnet)));
            }

            var selector = new BoxSelector<ErgoUnsignedInput>(_inputs);
            
            //Make sure any forced boxes get included.
            selector = selector.ensureInclusion(_inputsForcedInclusionBoxIds);

            //todo: selector callbacks

            var target = _burning?.Count > 0 ? OutputSum(outputsLocal, new SelectionTarget<long> { tokens = _burning?.Select(x => new TokenTargetAmount<long> { tokenId = x.tokenId, amount = x.amount }).ToList() }) : OutputSum(outputsLocal);
            var inputs = selector.Select(new SelectionTarget<long> { nanoErgs = target.nanoErgs, tokens = target.tokens.Select(x => new TokenTargetAmount<long> { tokenId = x.tokenId, amount = x.amount }).ToList() });

            if (_changeAddress != null)
            {
                BoxAmounts change = BoxUtils.UtxoSumResultDiff(BoxUtils.UtxoSum(inputs.Select(x => new MinimalBoxAmounts { value = x.value, assets = x.assets.Select(y => new TokenAmount<long> { tokenId = y.tokenId, amount = y.amount }).ToList() })), target);
                var changeBoxes = new List<OutputBuilder>();

                if (change.tokens?.Count > 0)
                {
                    var minRequiredNanoErgs = estimateMinChangeValue(new ChangeEstimationParams { changeAddress = _changeAddress, creationHeight = _creationHeight, tokens = change.tokens, maxTokensPerBox = settings().maxTokensPerChangeBox(), baseIndex = outputsLocal.Count + 1 });

                    while (minRequiredNanoErgs > change.nanoErgs)
                    {
                        inputs = selector.Select(new SelectionTarget<long> { nanoErgs = target.nanoErgs + minRequiredNanoErgs, tokens = target.tokens.Select(x => new TokenTargetAmount<long> { tokenId = x.tokenId, amount = x.amount }).ToList() });

                        change = BoxUtils.UtxoSumResultDiff(BoxUtils.UtxoSum(inputs.Select(x => new MinimalBoxAmounts { value = x.value, assets = x.assets.Select(y => new TokenAmount<long> { tokenId = y.tokenId, amount = y.amount }).ToList() })), target);
                        minRequiredNanoErgs = estimateMinChangeValue(new ChangeEstimationParams { changeAddress = _changeAddress, creationHeight = _creationHeight, tokens = change.tokens, maxTokensPerBox = settings().maxTokensPerChangeBox(), baseIndex = outputsLocal.Count + 1 });
                    }

                    var chunkedTokens = Tools.ChunkBy(change.tokens, this._settings.maxTokensPerChangeBox());
                    foreach (var tokens in chunkedTokens)
                    {
                        var output = new OutputBuilder(
                            OutputBuilder.SAFE_MIN_BOX_VALUE,
                            this._changeAddress,
                            this._creationHeight
                        ).AddTokens(tokens.ToList());
                        output.SetValue(output.estimateMinBoxValue());

                        change.nanoErgs -= output.GetValue();
                        changeBoxes.Add(output);
                    }
                }

                if (change.nanoErgs > 0)
                {
                    if (changeBoxes.Any())
                    {
                        if (settings().shouldIsolateErgOnChange())
                        {
                            outputsLocal.Add(new OutputBuilder(change.nanoErgs, _changeAddress));
                        }
                        else
                        {
                            var firstChangeBox = changeBoxes.First();
                            firstChangeBox.SetValue(firstChangeBox.GetValue() + change.nanoErgs);
                        }

                        outputsLocal.AddRange(changeBoxes);
                    }
                    else
                    {
                        outputsLocal.Add(new OutputBuilder(change.nanoErgs, _changeAddress));
                    }
                }
            }

            foreach (var input in inputs)
            {
                var temp = new ErgoBox(new Box<long> { boxId = input.boxId, transactionId = input.transactionId, index = input.index, ergoTree = input.ergoTree, creationHeight = input.creationHeight, value = input.value, assets = input.assets, additionalRegisters = input.additionalRegisters });
                if (!temp.isValid())
                {
                    throw new Exception($"Invalid input {input.boxId}");
                }
            }

            var unsignedTransaction = new ErgoUnsignedTransaction(
                inputs
                , dataInputs()
                , outputsLocal.Select(x => x.SetCreationHeight(_creationHeight, false).build(inputs)));

            var burning = unsignedTransaction.burning();
            if (burning.nanoErgs > 0)
            {
                throw new Exception("It's not possible to burn ERG!");
            }
            if (burning.tokens.Any() && _burning.Any())
            {
                burning = BoxUtils.UtxoSumResultDiff(burning, new BoxAmounts { nanoErgs = 0, tokens = _burning });
            }

            if (!settings().canBurnTokens() && burning.tokens.Any())
            {
                throw new Exception("Token burning not allowed!");
            }

            return unsignedTransaction;
        }
        /*
        public ErgoUnsignedTransaction test()
        {
            var changeAddress = ErgoAddress.fromBase58("9fwTmAApbyYTfv6ZPELpC5iFkyNBtt3XNfsauUMX4Jm7mfYtb4p");


            var tx = new TransactionBuilder(993851)
                .from()
                .to(new List<OutputBuilder> { new OutputBuilder(1000000, ErgoAddress.fromBase58("9hGgu7AqR9ki7zsHqMkzQW7egvMPYF7unzjpG5Ru4K57JT2grHr")) })
                .sendChangeTo(changeAddress)
                .payMinFee()
                .build();
        }*/
    }
}
