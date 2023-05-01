using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Text.Json;
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
                    case SigmaTypeCode.BigInt:
                        return reader.readBigInt();
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
                        var elements = new dynamic[length];

                        for (var i = 0; i < length; i++)
                        {
                            elements[i] = (Deserialize(embeddedType, reader));
                        }

                        return elements;
                }
            }

            throw new InvalidDataException("Parsing error: type not implemented.");
        }

        //COMPLETELY UNTESTED! PROBABLY DOESN'T WORK!!!!
        public static void serialize(dynamic data, SigmaWriter writer)
        {

            if (Utils.isPrimitiveType(data))
            {
                switch (data.type)
                {
                    case SigmaTypeCode.Boolean:
                        writer.writeBoolean((bool)((IPrimitiveSigmaType)data).value);
                        break;
                    case SigmaTypeCode.Byte:
                        writer.write((byte)((IPrimitiveSigmaType)data).value);
                        break;
                    case SigmaTypeCode.Short:
                        writer.writeShort((short)((IPrimitiveSigmaType)data).value);
                        break;
                    case SigmaTypeCode.Int:
                        writer.writeInt((int)((IPrimitiveSigmaType)data).value);
                        break;
                    case SigmaTypeCode.Long:
                        writer.writeLong((long)((IPrimitiveSigmaType)data).value);
                        break;
                    case SigmaTypeCode.BigInt:
                        writer.writeBigInt((BigInteger)((IPrimitiveSigmaType)data).value);
                        break;
                    case SigmaTypeCode.GroupElement:
                        writer.writeBytes(((IPrimitiveSigmaType)data).value);
                        break;
                    case SigmaTypeCode.SigmaProp:
                        var node = (IPrimitiveSigmaType)((IPrimitiveSigmaType)data).value;
                        if (node.type == SigmaTypeCode.GroupElement)
                        {
                            writer.write((byte)PROVE_DLOG_OP);
                            serialize(node, writer);
                        }
                        else
                        {
                            throw new Exception("Not implemented");
                        }
                        break;
                    case SigmaTypeCode.Unit:
                        break;
                    default:
                        throw new Exception("Not implemented");

                }
            }
            else if (Utils.isColl(data))
            {
                if (((ISigmaCollection)data).value.GetType() == typeof(string)) writer.writeVlq((uint)(((ISigmaCollection)data).value.Length / 2));
                else writer.writeVlq((uint)((ISigmaCollection)data).value.Length);

                switch (((ISigmaCollection)data).elementsType)
                {
                    case SigmaTypeCode.Boolean:
                        writer.writeBits(((ISigmaCollection)data).value);
                        break;
                    case SigmaTypeCode.Byte:
                        if (((ISigmaCollection)data).value.GetType() == typeof(string)) writer.writeBytes(Tools.HexToBytes(((ISigmaCollection)data).value));
                        else writer.writeBytes(((ISigmaCollection)data).value);
                        break;
                    default:
                        for (var i = 0; i < ((ISigmaCollection)data).value.Length; i++)
                        {
                            serialize(((ISigmaCollection)data).value[i], writer);
                        }
                        break;
                }
            }
            else
            {
                throw new Exception("Not implemented");
            }
        }
    }
}
