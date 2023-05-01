using FleetSharp.Models;
using FleetSharp.Sigma;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Builder
{
    //Partially loaned from https://github.com/fleet-sdk/fleet/blob/master/packages/core/src/builder/outputBuilder.ts
    public class OutputBuilder
    {
        public const long BOX_VALUE_PER_BYTE = 360;
        public const long SAFE_MIN_BOX_VALUE = 1000000;

        private ErgoAddress _address { get; set; }
        private List<TokenAmount<long>> _assets { get; set; }
        private long _value { get; set; }
        private long? _creationHeight { get; set; }
        private NonMandatoryRegisters? _registers { get; set; }

        private NewToken<long>? _minting { get; set; }


        public long estimateMinBoxValue(long valuePerByte = BOX_VALUE_PER_BYTE)
        {
            long valuePerByteBigInt = valuePerByte;
            return BoxSerializer.estimateBoxSize(build(), SAFE_MIN_BOX_VALUE) * valuePerByteBigInt;
        }


        public OutputBuilder(long value, ErgoAddress recipient, long? creationHeight = null)
        {
            SetValue(value);
            _creationHeight = creationHeight;
            _assets = new List<TokenAmount<long>>();
            _registers = new NonMandatoryRegisters { };
            _address = recipient;
        }

        public long GetValue()
        {
            return _value;
        }

        public ErgoAddress GetAddress()
        {
            return _address;
        }

        public string GetErgoTree()
        {
            return Tools.BytesToHex(_address.GetErgoTree());
        }

        public long? GetCreationHeight()
        {
            return _creationHeight;
        }

        public List<TokenAmount<long>> GetAssets()
        {
            return _assets;
        }

        public NonMandatoryRegisters? GetAdditionalRegisters()
        {
            return _registers;
        }

        public NewToken<long>? minting()
        {
            return _minting;
        }

        public OutputBuilder SetValue(long value)
        {
            _value = value;

            if (_value <= 0) throw new Exception("An UTxO cannot be created without a minimum required amount.");

            return this;
        }

        public OutputBuilder AddToken(TokenAmount<long> token)
        {
            _assets.Add(token);

            return this;
        }

        public OutputBuilder AddTokens(List<TokenAmount<long>> tokens)
        {
            _assets.AddRange(tokens);

            return this;
        }

        //ToDo: Use tokenIds or the objects here?
        public OutputBuilder RemoveTokens(string tokenId)
        {
            _assets = _assets.Where(x => x.tokenId != tokenId).ToList();

            return this;
        }

        public OutputBuilder RemoveTokens(List<string> tokenIds)
        {
            _assets = _assets.Where(x => !tokenIds.Contains(x.tokenId)).ToList();

            return this;
        }

        public OutputBuilder mintToken(NewToken<long> token)
        {
            _minting = new NewToken<long> { tokenId = token.tokenId, name = token.name, decimals = token.decimals, description = token.description, amount = token.amount };

            return this;
        }

        public OutputBuilder SetCreationHeight(long height, bool replace = true)
        {
            if (_creationHeight == null || replace == true)
            {
                _creationHeight = height;
            }

            return this;
        }

        public OutputBuilder SetAdditionalRegisters(NonMandatoryRegisters registers)
        {
            _registers = registers;

            return this;
        }

        public BoxCandidate<long> build(List<ErgoUnsignedInput>? transactionInputs = null)
        {
            var tokens = GetAssets();

            if (minting() != null)
            {
                if (transactionInputs == null || transactionInputs.Count == 0) throw new ArgumentException("Undefined minting context!");

                var registers = GetAdditionalRegisters();
                if (registers == null || (registers.R4 == null && registers.R5 == null && registers.R6 == null && registers.R7 == null && registers.R8 == null && registers.R9 == null))
                {
                    SetAdditionalRegisters(new NonMandatoryRegisters {
                        R4 = ConstantSerializer.SConstant(ISigmaCollection.SColl(SigmaTypeCode.Byte, Tools.UTF8StringToBytes(minting().name ?? ""))),
                        R5 = ConstantSerializer.SConstant(ISigmaCollection.SColl(SigmaTypeCode.Byte, Tools.UTF8StringToBytes(minting().description ?? ""))),
                        R6 = ConstantSerializer.SConstant(ISigmaCollection.SColl(SigmaTypeCode.Byte, Tools.UTF8StringToBytes(minting().decimals?.ToString() ?? "0")))
                    });
                }

                tokens.Insert(0, new TokenAmount<long>
                {
                    tokenId = (transactionInputs.First()).boxId,
                    amount = minting().amount
                });
            }

            if (GetCreationHeight() == null)
            {
                throw new Exception("Undefined creationHeight!");
            }

            return new BoxCandidate<long>
            {
                value = GetValue(),
                ergoTree = GetErgoTree(),
                creationHeight = GetCreationHeight() ?? 0,
                assets = tokens.Select(x => new TokenAmount<long> { tokenId = x.tokenId, amount = x.amount }).ToList(),
                additionalRegisters = GetAdditionalRegisters()
            };
        }
    }
}
