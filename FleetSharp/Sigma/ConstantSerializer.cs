using System;
using System.Collections.Generic;
using System.Text;

namespace FleetSharp.Sigma
{
    public static class ConstantSerializer
    {
        public static dynamic SParse(byte[] bytes)
        {
            SigmaReader reader = new SigmaReader(bytes);
            var type = reader.readType();

            return DataSerializer.Deserialize(type, reader);
        }
    }
}
