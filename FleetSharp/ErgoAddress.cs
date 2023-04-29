using Blake2Fast;
using FleetSharp.Types;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FleetSharp
{
    public class ErgoAddress
    {
        private static byte[] _ergoTree;
        private static Network? _network;
        private static AddressType _type;

        private static int CHECKSUM_LENGTH = 4;

        private static byte[] P2PK_ERGOTREE_PREFIX = Tools.HexToBytes("0008cd");
        private static int P2PK_ERGOTREE_LENGTH = 36;

        private static byte[] P2SH_ERGOTREE_SUFFIX = Tools.HexToBytes("d40801");
        private static byte[] P2SH_ERGOTREE_PREFIX = Tools.HexToBytes("00ea02d193b4cbe4e3010e040004300e18");
        private static int P2SH_ERGOTREE_LENGTH = 36;
        private static int P2SH_HASH_LENGTH = 36;

        public ErgoAddress(byte[] ergoTree, Network? network)
        {
            _ergoTree = ergoTree;
            _network = network;
            _type = _getErgoTreeType(ergoTree);
        }
        private Network _getEncodedNetworkType(byte[] addressBytes)
        {
            return (Network)(addressBytes.First() & 0xf0);
        }

        private AddressType _getEncodedAddressType(byte[] addressBytes)
        {
            return (AddressType)(addressBytes.First() & 0xf);
        }

        private AddressType _getErgoTreeType(byte[] ergoTree)
        {
            if (ergoTree.Length == P2PK_ERGOTREE_LENGTH && ergoTree.Take(P2PK_ERGOTREE_PREFIX.Length).SequenceEqual(P2PK_ERGOTREE_PREFIX)) return AddressType.P2PK;
            else if (ergoTree.Length == P2SH_ERGOTREE_LENGTH
                        && ergoTree.Take(P2SH_ERGOTREE_PREFIX.Length).SequenceEqual(P2SH_ERGOTREE_PREFIX)
                        && ergoTree.Skip(ergoTree.Length - P2SH_ERGOTREE_SUFFIX.Length).Take(P2SH_ERGOTREE_SUFFIX.Length).SequenceEqual(P2SH_ERGOTREE_SUFFIX)) return AddressType.P2SH;
            else return AddressType.P2S;
        }

        public static ErgoAddress fromErgoTree(string ergoTreeHex, Network? network)
        {
            return new ErgoAddress(Tools.HexToBytes(ergoTreeHex), network);
        }

        public static ErgoAddress fromPublicKeyBytes(byte[] publicKeyBytes, Network? network)
        {
            return new ErgoAddress(P2PK_ERGOTREE_PREFIX.Concat(publicKeyBytes).ToArray(), network);
        }

        public static ErgoAddress fromPublicKeyHex(string publicKeyHex, Network? network)
        {
            return new ErgoAddress(P2PK_ERGOTREE_PREFIX.Concat(Tools.HexToBytes(publicKeyHex)).ToArray(), network);
        }

        public byte[][] getPublicKeys()
        {
            if (_type == AddressType.P2PK) return new byte[][] { _ergoTree.Skip(P2PK_ERGOTREE_PREFIX.Length).ToArray() };
            return new byte[][] { };
        }

        public byte[] GetErgoTree()
        {
            return _ergoTree;
        }

        private string _encode(byte[] body, AddressType type, Network? network)
        {
            if (network == null)
            {
                if (_network != null) network = _network;
                else network = Network.Mainnet;
            }

            byte[] head = new byte[] { Convert.ToByte((byte)network + (byte)type) };
            body = head.Concat(body).ToArray();
            var checksum = Blake2b.ComputeHash(32, body).Take(CHECKSUM_LENGTH).ToArray();
            return SimpleBase.Base58.Bitcoin.Encode(body.Concat(checksum).ToArray());
        }

        public string encode(Network? network)
        {
            var body = new List<byte>();
            if (_type == AddressType.P2PK)
            {
                body = getPublicKeys().First().ToList();
            }
            else if (_type == AddressType.P2SH)
            {
                body = _ergoTree.Skip(P2SH_ERGOTREE_PREFIX.Length).Take(P2SH_HASH_LENGTH).ToList();
            }
            else
            {
                body = _ergoTree.ToList();
            }

            return _encode(body.ToArray(), _type, network);
        }

        public string ToString(Network? network)
        {
            return encode(network);
        }
    }
}
