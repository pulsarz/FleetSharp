using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace FleetSharp.Sigma
{
    public enum SigmaTypeCode
    {
        Boolean = 0x01,
        Byte = 0x02,
        Short = 0x03,
        Int = 0x04,
        Long = 0x05,
        BigInt = 0x06,
        GroupElement = 0x07,
        SigmaProp = 0x08,
        Coll = 0x0c,
        NestedColl = 0x18,
        Option = 0x24,
        OptionColl = 0x30,
        Tuple2 = 0x3c,
        Tuple3 = 0x48,
        Tuple4 = 0x54,
        TupleN = 0x60,
        Any = 0x61,
        Unit = 0x62,
        Box = 0x63,
        AvlTree = 0x64,
        Context = 0x65,
        Header = 0x68,
        PreHeader = 0x69,
        Global = 0x6a
    }

    public class ISigmaType
    {
        public SigmaTypeCode type { get; set; }
    }

    public class IPrimitiveSigmaType : ISigmaType
    {
        public dynamic value { get; set; }

        public static dynamic _createPrimitiveType(SigmaTypeCode type, dynamic? value)
        {
            if (value != null) return new IPrimitiveSigmaType() { type = type, value = value };
            else return type;
        }
        public static dynamic SByte()
        {
            return _createPrimitiveType(SigmaTypeCode.Byte, null);
        }

        public static dynamic SByte(byte value)
        {
            return _createPrimitiveType(SigmaTypeCode.Byte, value);
        }
        public static dynamic SBool()
        {
            return _createPrimitiveType(SigmaTypeCode.Boolean, null);
        }

        public static dynamic SBool(bool value)
        {
            return _createPrimitiveType(SigmaTypeCode.Boolean, value);
        }
        public static dynamic SShort()
        {
            return _createPrimitiveType(SigmaTypeCode.Short, null);
        }

        public static dynamic SShort(short value)
        {
            return _createPrimitiveType(SigmaTypeCode.Short, value);
        }
        public static dynamic SInt()
        {
            return _createPrimitiveType(SigmaTypeCode.Int, null);
        }

        public static dynamic SInt(int value)
        {
            return _createPrimitiveType(SigmaTypeCode.Int, value);
        }

        public static dynamic SLong()
        {
            return _createPrimitiveType(SigmaTypeCode.Long, null);
        }

        public static dynamic SLong(long value)
        {
            return _createPrimitiveType(SigmaTypeCode.Long, value);
        }

        public static dynamic SBigInt()
        {
            return _createPrimitiveType(SigmaTypeCode.BigInt, null);
        }

        public static dynamic SBigInt(BigInteger value)
        {
            return _createPrimitiveType(SigmaTypeCode.BigInt, value);
        }
        public static dynamic SUnit()
        {
            return _createPrimitiveType(SigmaTypeCode.Unit, null);
        }
        public static dynamic SGroupElement()
        {
            return _createPrimitiveType(SigmaTypeCode.GroupElement, null);
        }
        public static dynamic SGroupElement(byte[] value)
        {
            return _createPrimitiveType(SigmaTypeCode.GroupElement, value);
        }
        public static dynamic SSigmaProp()
        {
            return _createPrimitiveType(SigmaTypeCode.SigmaProp, null);
        }
        public static dynamic SSigmaProp(IPrimitiveSigmaType value)
        {
            return _createPrimitiveType(SigmaTypeCode.SigmaProp, value);
        }

    }

    public class ISigmaCollection : ISigmaType
    {
        public dynamic value { get; set; }
        public SigmaTypeCode elementsType { get; set; }

        //elements should be an array, not a list!
        public static dynamic SColl(SigmaTypeCode type, dynamic elements)
        {
            return new ISigmaCollection() { type = SigmaTypeCode.Coll, elementsType = type, value = elements };
        }
    }

}
