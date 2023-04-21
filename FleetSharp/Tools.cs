using System;
using System.Globalization;
using System.Linq;
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
            return Convert.ToHexString(bytes);
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
    }
}
