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

        public ulong readVlqInt64()
        {
            return VLQ.ReadVlqInt64(this);
        }

        public short readShort()
        {
            return (short)ZigZag.zigzag_to_int(readVlq());
        }

        public long readLong()
        {
            return ZigZag.zigzag_to_long(readVlqInt64());
        }

        public int readInt()
        {
            return (int)readLong();
        }

        public BigInteger readBigInt()
        {
            var len = (int)VLQ.ReadVlqInt32(this);
            //convert BE to LE
            return Tools.BytesToBigInteger(readBytes(len).Reverse().ToArray());
        }
    }
}
