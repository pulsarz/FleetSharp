using FleetSharp.Models;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder
{
    public class TransactionBuilder
    {
        public const long RECOMMENDED_MIN_FEE_VALUE = 1100000;
        public const string FEE_CONTRACT = "1005040004000e36100204a00b08cd0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798ea02d192a39a8cc7a701730073011001020402d19683030193a38cc7b2a57300000193c2b2a57301007473027303830108cdeeac93b1a57304";

        //should be ErgoUnsignedInput or Box<Amount>
        private List<dynamic> _inputs { get; set; }
        private List<dynamic> _dataInputs { get; set; }
        private List<OutputBuilder> _outputs { get; set; }
        private TransactionBuilderSettings _settings { get; set; }
        private long _creationHeight { get; set; }

        private ErgoAddress? _changeAddress { get; set; }
        private long? _feeAmount { get; set; }
        private List<TokenAmount<long>>? _burning { get; set; }

        public TransactionBuilder(long creationHeight)
        {
            _inputs = new List<dynamic>();
            _dataInputs = new List<dynamic>();
            _outputs = new List<OutputBuilder>();
            _settings = new TransactionBuilderSettings();
            _creationHeight = creationHeight;
        }

        public List<dynamic> inputs()
        {
            return _inputs;
        }
        public List<dynamic> dataInputs()
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
        public TransactionBuilder and()
        {
            return this;
        }

        public TransactionBuilder from(List<dynamic> inputs)
        {
            _inputs.AddRange(inputs);

            return this;
        }

        public TransactionBuilder to(List<OutputBuilder> outputs)
        {
            _outputs.AddRange(outputs);

            return this;
        }

        public TransactionBuilder withDataFrom(List<dynamic> dataInputs)
        {
            _dataInputs.AddRange(dataInputs);

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

       /* public ErgoUnsignedTransaction build()
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

            var outputsLocal = _outputs.Clone();

            if (_feeAmount != null)
            {
                outputsLocal.Add(new OutputBuilder(_feeAmount, ErgoAddress.fromErgoTree(FEE_CONTRACT, Network.Mainnet)));
            }

            var selector = 
        }*/
    }
}
