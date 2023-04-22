using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetSharp.Sigma
{
    internal static class TypeSerializer
    {
        public static void serialize(dynamic value, SigmaWriter buffer)
        {
            if (Utils.isPrimitiveType(value))
            {
                buffer.write((byte)((IPrimitiveSigmaType)value).type);
            }
            else if (Utils.isColl(value))
            {
                if (Utils.isEmbeddable(value))
                {
                    buffer.write((byte)((byte)value.type + (byte)value.elementsType));
                }
            }
        }
    }
}
