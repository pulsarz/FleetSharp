using System;
using System.Collections.Generic;
using System.Text;

namespace FleetSharp.Sigma
{
    public static class ConstantSerializer
    {
        private static int MAX_CONSTANT_TYPES_LENGTH = 100;
        private static int MAX_CONSTANT_CONTENT_LENGTH = 4096;
        private static int MAX_CONSTANT_LENGTH = MAX_CONSTANT_TYPES_LENGTH + MAX_CONSTANT_CONTENT_LENGTH;

        //Deserialize
        public static dynamic SParse(byte[] bytes)
        {
            SigmaReader reader = new SigmaReader(bytes);
            var type = reader.readType();

            return DataSerializer.Deserialize(type, reader);
        }
        public static dynamic SParse(string hexString)
        {
            return SParse(Tools.HexToBytes(hexString));
        }

        //Serialize
        public static string SConstant(dynamic content)
        {
            var writer = new SigmaWriter(MAX_CONSTANT_LENGTH);

            TypeSerializer.serialize(content, writer);
            DataSerializer.serialize(content, writer);

            return writer.toHex();
        }
    }
}
