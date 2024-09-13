using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Sigma.Interface
{
    public interface ISigmaWriter
    {
        public int length();
        public SigmaWriter write(byte @byte);
        public SigmaWriter writeBytes(byte[] bytes);
        public SigmaWriter writeBits(bool[] bits);
        public SigmaWriter writeBoolean(bool value);
        public SigmaWriter writeVlq(uint value);
        public SigmaWriter writeVlqInt64(ulong value);
        public SigmaWriter writeShort(short value);
        public SigmaWriter writeLong(long value);
        public SigmaWriter writeInt(int value);
        public SigmaWriter writeBigInt(BigInteger value);
        public string toHex();
        public byte[] toBytes();
    }
}
