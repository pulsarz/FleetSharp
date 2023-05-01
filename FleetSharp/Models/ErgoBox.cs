using Blake2Fast;
using FleetSharp.Sigma;
using FleetSharp.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Models
{
    public class ErgoBox
    {
        public string boxId;
        public long value;
        public string ergoTree;
        public long creationHeight;
        public List<TokenAmount<long>> assets;
        public NonMandatoryRegisters additionalRegisters;
        public string transactionId;
        public int index;

        public ErgoBox(Box<long> box)
        {
            boxId = box.boxId;
            ergoTree = box.ergoTree;
            creationHeight = box.creationHeight;
            value = box.value;
            assets = box.assets.ConvertAll(asset => new TokenAmount<long>
            {
                tokenId = asset.tokenId,
                amount = asset.amount
            });
            additionalRegisters = box.additionalRegisters;
            transactionId = box.transactionId;
            index = box.index;
        }

        public bool isValid()
        {
            return validate(this);
        }

        public static bool validate(dynamic box)
        {
            byte[] bytes = BoxSerializer.serializeBox(box).toBytes();
            string hash =  Tools.BytesToHex(Blake2b.ComputeHash(32, bytes));

            return box.boxId == hash;
        }
    }

}
