using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FleetSharp.Sigma
{
    internal static class VLQ
    {
        //https://github.com/fleet-sdk/fleet/blob/33159be30de3c28f38b09707c4c36e967530b1ea/packages/core/src/serializer/vlq.ts
        public static uint ReadVlqInt32(SigmaReader r)
        {
            uint value = 0;
            int shift = 0;
            uint lower7bits = 0;

            do
            {
                lower7bits = r.readByte();
                value |= (lower7bits & 0x7f) << shift;
                shift += 7;
            }
            while ((lower7bits & 0x80) != 0);

            return value;
        }

        public static ulong ReadVlqInt64(SigmaReader r)
        {
            ulong value = 0;
            int shift = 0;
            ulong lower7bits = 0;

            do
            {
                lower7bits = r.readByte();
                value |= (lower7bits & 0x7f) << shift;
                shift += 7;
            }
            while ((lower7bits & 0x80) != 0);

            return value;
        }

        public static SigmaWriter WriteVlqInt32(SigmaWriter writer, uint value)
        {
            if (value == 0)
            {
                return writer.write(0);
            }
            else if (value < 0)
            {
                throw new InvalidDataException("Variable Length Quantity not supported for negative numbers.");
            }

            do
            {
                var lower7bits = value & 0x7f;
                value >>= 7;

                if (value > 0) lower7bits |= 0x80;

                writer.write((byte)lower7bits);
            }
            while (value > 0);

            return writer;
        }

        public static SigmaWriter WriteVlqInt64(SigmaWriter writer, ulong value)
        {
            if (value == 0)
            {
                return writer.write(0);
            }
            else if (value < 0)
            {
                throw new InvalidDataException("Variable Length Quantity not supported for negative numbers.");
            }

            do
            {
                var lower7bits = value & 0x7f;
                value >>= 7;

                if (value > 0) lower7bits |= 0x80;

                writer.write((byte)lower7bits);
            }
            while (value > 0);

            return writer;
        }
    }
}
