﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FleetSharp
{
    public class Tools
    {
        public static byte[] HexToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        public static string BytesToHex(byte[] bytes)
        {
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public static string HexToUTF8String(string hex)
        {
            byte[] bytes = HexToBytes(hex);
            return Encoding.UTF8.GetString(bytes);
        }

        public static long HexToLong(string hex)
        {
            return long.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public static BigInteger BytesToBigInteger(byte[] bytes)
        {
            return new BigInteger(bytes);
        }

        public static BigInteger HexToBigInteger(string hex)
        {
            return BytesToBigInteger(HexToBytes(hex));
        }

        public static byte[] BigIntegerToBytes(BigInteger value)
        {
            return value.ToByteArray();
        }

        public static string BigIntegerToHex(BigInteger value)
        {
            return BytesToHex(BigIntegerToBytes(value));
        }
    }
}
