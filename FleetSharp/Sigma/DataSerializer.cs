using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;

namespace FleetSharp.Sigma
{
    internal static class DataSerializer
    {
        private static int GROUP_ELEMENT_LENGTH = 33;
        private static int PROVE_DLOG_OP = 0xcd;

        public static dynamic Deserialize(SigmaTypeCode typeCode, SigmaReader reader)
        {

            if (Utils.isPrimitiveTypeCode(typeCode))
            {
                switch (typeCode)
                {
                    case SigmaTypeCode.Boolean:
                        return reader.readBoolean();
                    case SigmaTypeCode.Byte:
                        return reader.readByte();
                    case SigmaTypeCode.Short:
                        return reader.readShort();
                    case SigmaTypeCode.Int:
                        return reader.readInt();
                    case SigmaTypeCode.Long:
                        return reader.readLong();
                    case SigmaTypeCode.GroupElement:
                        return reader.readBytes(GROUP_ELEMENT_LENGTH);
                    case SigmaTypeCode.SigmaProp:
                        if (reader.readByte() == PROVE_DLOG_OP) return Deserialize(SigmaTypeCode.GroupElement, reader);
                        break;
                    default:
                        break;

                }
            }
            else if (Utils.isCollTypeCode(typeCode))
            {
                var embeddedType = (SigmaTypeCode)(typeCode - SigmaTypeCode.Coll);
                var length = reader.readVlq();

                switch (embeddedType)
                {
                    case SigmaTypeCode.Boolean:
                        return reader.readBits((int)length);
                    case SigmaTypeCode.Byte:
                        return reader.readBytes((int)length);
                    default:
                        var elements = new List<dynamic>();

                        for (var i = 0; i < length; i++)
                        {
                            elements.Add(Deserialize(embeddedType, reader));
                        }

                        return elements.ToArray();
                }
            }

            throw new InvalidDataException("Parsing error: type not implemented.");
        }
    }
}
