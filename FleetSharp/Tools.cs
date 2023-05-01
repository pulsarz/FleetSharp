using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

namespace FleetSharp
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T obj)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            for (int i = 0; i < source.Count(); i += chunkSize)
            {
                yield return source.Skip(i).Take(chunkSize);
            }
        }
    }

    public static class ListExtensions
    {
        public static bool ContainsDuplicates<T>(this IEnumerable<T> enumerable)
        {
            var knownKeys = new HashSet<T>();
            return enumerable.Any(item => !knownKeys.Add(item));
        }
    }

    public class Tools
    {
        //We need this since the Extension doesn't work with dynamic.
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(IEnumerable<T> source, int chunkSize)
        {
            for (int i = 0; i < source.Count(); i += chunkSize)
            {
                yield return source.Skip(i).Take(chunkSize);
            }
        }

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
        public static byte[] BigIntegerToBytes(BigInteger value)
        {
            return value.ToByteArray();
        }

        public static int HexByteSize(string hex)
        {
            return (hex.Length / 2);
        }

        public static byte[] UTF8StringToBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
