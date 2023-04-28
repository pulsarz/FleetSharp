using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace FleetSharp.Sigma
{
    internal class SigmaReader
    {
        //Stolen from https://github.com/fleet-sdk/fleet/blob/33159be30de3c28f38b09707c4c36e967530b1ea/packages/core/src/serializer/sigma/sigmaReader.ts
        private byte[] _bytes;
        private int _cursor;
        public SigmaReader(byte[] bytes)
        {
            _bytes = bytes;
            _cursor = 0;
        }

        public byte readByte()
        {
            return _bytes[_cursor++];
        }

        public byte[] readBytes(int length)
        {
            var ret = _bytes.Skip(_cursor).Take(length).ToArray();
            _cursor += length;
            return ret;
        }

        public bool[] readBits(int length)
        {
            var bits = new bool[length];
            var bitOffset = 0;

            for (var i = 0; i < length; i++)
            {
                var bit = (_bytes[_cursor] >> bitOffset++) & 1;
                bits[i] = bit == 1;

                if (bitOffset == 8)
                {
                    bitOffset = 0;
                    _cursor++;
                }
            }

            if (bitOffset > 0) _cursor++;

            return bits;
        }

        public bool readBoolean()
        {
            return (this.readByte() == 0x01);
        }

        public SigmaTypeCode readType()
        {
            return (SigmaTypeCode)Enum.ToObject(typeof(SigmaTypeCode), readByte());
        }

        public uint readVlq()
        {
            return (uint)VLQ.ReadVlqInt32(this);
        }

        public ulong readLongVlq()
        {
            return (ulong)VLQ.ReadVlqInt64(this);
        }

        public short readShort()
        {
            return (short)ZigZag.zigzag_to_int(readVlq());
        }

        public long readLong()
        {
            return (long)ZigZag.zigzag_to_long(readLongVlq());
        }

        public int readInt()
        {
            return (int)readLong();
        }

        public BigInteger readBigInt()
        {
            var len = (int)VLQ.ReadVlqInt32(this);
            return Tools.BytesToBigInteger(readBytes(len));
        }
    }
}
