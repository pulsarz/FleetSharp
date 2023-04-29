using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace FleetSharp.Sigma
{
    internal class SigmaWriter
    {
        //Stolen from https://github.com/fleet-sdk/fleet/blob/master/packages/core/src/serializer/sigma/sigmaWriter.ts
        private byte[] _bytes;
        private int _cursor;
        public SigmaWriter(int maxLength)
        {
            _bytes = new byte[maxLength];
            _cursor = 0;
        }

        public int length()
        {
            return _cursor;
        }

        public SigmaWriter write(byte @byte)
        {
            _bytes[_cursor++] = @byte;
            return this;
        }

        public SigmaWriter writeBytes(byte[] bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
            {
                _bytes[_cursor++] = bytes[i];
            }
            return this;
        }

        public SigmaWriter writeBits(bool[] bits)
        {
            var bitOffset = 0;

            for (var i=0; i < bits.Length; i++)
            {
                if (bits[i]) _bytes[_cursor] |= (byte)(1 << bitOffset++);
                else _bytes[_cursor] &= (byte)~(1 << bitOffset++);

                if (bitOffset == 8)
                {
                    bitOffset = 0;
                    _cursor++;
                }
            }

            if (bitOffset > 0) _cursor++;

            return this;
        }

        public SigmaWriter writeBoolean(bool value)
        {
            this.write((byte)(value == true ? 0x01 : 0x00));
            return this;
        }

        public SigmaWriter writeVlq(uint value)
        {
            return VLQ.WriteVlqInt32(this, value);
        }

        public SigmaWriter writeVlqInt64(ulong value)
        {
            return VLQ.WriteVlqInt64(this, value);
        }

        public SigmaWriter writeShort(short value)
        {
            writeVlq(ZigZag.int_to_zigzag(value));
            return this;
        }

        public SigmaWriter writeLong(long value)
        {
            writeVlqInt64(ZigZag.long_to_zigzag(value));
            return this;
        }

        public SigmaWriter writeInt(int value)
        {
            this.writeLong(value);
            return this;
        }

        public SigmaWriter writeBigInt(BigInteger value)
        {
            //Convert LE to BE
            var bytes = Tools.BigIntegerToBytes(value).Reverse().ToArray();
            writeVlq((uint)bytes.Length);
            writeBytes(bytes);

            return this;
        }

        public string toHex()
        {
            return Tools.BytesToHex(_bytes.Take(_cursor).ToArray());
        }

        public byte[] toBytes()
        {
            return _bytes.Take(_cursor).ToArray();
        }
    }
}
