using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Sigma.Interface
{
    public interface ISigmaReader
    {
        public byte readByte();
        public byte[] readBytes(int length);
        public bool[] readBits(int length);
        public bool readBoolean();
        public SigmaTypeCode readType();
        public uint readVlq();
        public ulong readVlqInt64();
        public short readShort();
        public long readLong();
        public int readInt();
        public BigInteger readBigInt();
    }
}
