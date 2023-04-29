using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FleetSharp.Sigma
{
    internal static class ZigZag
    {
        public static int zigzag_to_int(uint zigzag)
        {
            int abs = (int)(zigzag >> 1);

            if (zigzag % 2 > 0) return ~abs;
            else return abs;
        }

        public static uint int_to_zigzag(int signed)
        {
            uint abs = (uint)signed << 1;

            if (signed < 0) return ~abs;
            else return abs;
        }

        public static long zigzag_to_long(ulong zigzag)
        {
            var abs = (long)(zigzag >> 1);

            if (zigzag % 2 > 0) return ~abs;
            else return abs;
        }

        public static ulong long_to_zigzag(long signed)
        {
            var abs = (ulong)signed << 1;

            if (signed < 0) return ~abs;
            else return abs;
        }
    }
}
