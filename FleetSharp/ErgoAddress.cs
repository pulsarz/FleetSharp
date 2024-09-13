using Blake2Fast;
using FleetSharp.Exceptions;
using FleetSharp.Interface;
using FleetSharp.Types;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace FleetSharp
{
    public class ErgoAddress : IErgoAddress
    {
        private byte[] _ergoTree;
        private Network? _network;
        private AddressType _type;

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

        public Network? GetNetworkType()
        {
            return _network;
		}

        public static bool validateBytes(byte[] addressBytes)
        {
            if (addressBytes.Length < CHECKSUM_LENGTH) return false;

            var script = addressBytes.Take(addressBytes.Length - CHECKSUM_LENGTH).ToArray();
            var checksum = addressBytes.Skip(addressBytes.Length - CHECKSUM_LENGTH).ToArray();
            var blakeHash = Blake2b.ComputeHash(32, script);
            var calculatedChecksum = blakeHash.Take(CHECKSUM_LENGTH).ToArray();

            if (_getEncodedAddressType(addressBytes) == AddressType.P2PK)
            {
                var pk = addressBytes.Skip(1).Take(addressBytes.Length - 1 - CHECKSUM_LENGTH).ToArray();

                //Check for valid ec points
                if (!_validateCompressedEcPoint(pk)) return false;
            }

            return (calculatedChecksum.SequenceEqual(checksum));
        }

        public static bool validateBase58(string address)
        {
            var bytes = SimpleBase.Base58.Bitcoin.Decode(address);
            return validateBytes(bytes);
        }

        public static bool _validateCompressedEcPoint(byte[] pointBytes)
        {
            if (pointBytes.Length == 0 || pointBytes.Length != 33) return false;
            return (pointBytes[0] == 0x02 || pointBytes[0] == 0x03);
        }

        private static Network _getEncodedNetworkType(byte[] addressBytes)
        {
            return (Network)(addressBytes.First() & 0xf0);
        }

        private static AddressType _getEncodedAddressType(byte[] addressBytes)
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
            if (!_validateCompressedEcPoint(publicKeyBytes)) throw new Exception("The public key is invalid!");
            return new ErgoAddress(P2PK_ERGOTREE_PREFIX.Concat(publicKeyBytes).ToArray(), network);
        }

        public static ErgoAddress fromPublicKeyHex(string publicKeyHex, Network? network)
        {
            return fromPublicKeyBytes(Tools.HexToBytes(publicKeyHex), network);
        }

        public static ErgoAddress fromHash(byte[] hashBytes, Network? network)
        {
            if (hashBytes.Length == 32)
            {
                hashBytes = hashBytes.Take(P2SH_HASH_LENGTH).ToArray();
            }
            else if (hashBytes.Length != P2SH_HASH_LENGTH)
            {
                throw new Exception($"Invalid hash length: {hashBytes.Length}");
            }

            var ergoTree = P2SH_ERGOTREE_PREFIX.Concat(hashBytes).Concat(P2SH_ERGOTREE_SUFFIX).ToArray();
            return new ErgoAddress(ergoTree, network);
        }

        public static ErgoAddress fromBase58(string encodedAddress, bool skipCheck = false)
        {
            var bytes = SimpleBase.Base58.Bitcoin.Decode(encodedAddress);

            if (!skipCheck && !validateBytes(bytes))
            {
                throw new InvalidAddressException(encodedAddress);
            }

            var network = _getEncodedNetworkType(bytes);
            var type = _getEncodedAddressType(bytes);
            var body = bytes.Skip(1).Take(bytes.Length - 1 - CHECKSUM_LENGTH).ToArray();

            if (type == AddressType.P2PK)
            {
                return fromPublicKeyBytes(body, network);
            }
            else if (type == AddressType.P2SH)
            {
                return fromHash(body, network);
            }
            else
            {
                return new ErgoAddress(body, network);
            }
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

        public string GetErgoTreeHex()
        {
            return Tools.BytesToHex(_ergoTree);
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
