using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FleetSharp.Models
{

    public class InputBox : Box<long>
    {
        public Dictionary<int, string?> extension { get; set; } = new Dictionary<int, string?>();
    }

    public class ErgoUnsignedInput : ErgoBox
    {
        private Dictionary<int, string?> _extension = new Dictionary<int, string?>();

        public Dictionary<int, string?> extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        public ErgoUnsignedInput(InputBox box) : base(box)
        {
            if (box.extension != null)
            {
                this.SetContextVars(box.extension);
            }
        }

        public ErgoUnsignedInput SetContextVars(Dictionary<int, string?> extension)
        {
            this._extension = extension;

            return this;
        }

        //inputs
        public dynamic toUnsignedInputObject(BuildOutputType type)
        {
            if (type.Equals(BuildOutputType.EIP12))
            {
                return new EIP12UnsignedInput
                {
                    boxId = boxId,
                    value = value.ToString(),
                    ergoTree = ergoTree,
                    creationHeight = creationHeight,
                    assets = assets,
                    additionalRegisters = additionalRegisters,
                    transactionId = transactionId,
                    index = index
                };
            }
            else
            {
                return new UnsignedInput {
                    boxId = boxId,
                    extension = extension,
                };
            }
        }

        //dataInputs
        public dynamic toPlainObject(BuildOutputType type)
        {
            if (type.Equals(BuildOutputType.EIP12))
            {
                return new EIP12UnsignedDataInput
                {
                    boxId = boxId,
                    value = value.ToString(),
                    ergoTree = ergoTree,
                    creationHeight = creationHeight,
                    assets = assets,
                    additionalRegisters = additionalRegisters,
                    transactionId = transactionId,
                    index = index
                };
            }
            else
            {
                return new DataInput
                {
                    boxId = boxId
                };
            }
        }
    }
}
