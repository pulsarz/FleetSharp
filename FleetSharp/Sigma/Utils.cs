using System;
using System.Collections.Generic;
using System.Text;

namespace FleetSharp.Sigma
{
    internal static class Utils
    {
        public static bool isConstructorTypeCode(SigmaTypeCode type)
        {
            return (type >= (SigmaTypeCode)0x0c && type <= (SigmaTypeCode)0x60);
        }

        public static bool isCollTypeCode(SigmaTypeCode type)
        {
            return (type >= (SigmaTypeCode)0x0c && type <= (SigmaTypeCode)0x23);
        }
        public static bool isColl(ISigmaType data)
        {
            return isCollTypeCode(data.type);
        }

        public static bool isEmbeddableTypeCode(SigmaTypeCode type)
        {
            return (type >= (SigmaTypeCode)0x01 && type <= (SigmaTypeCode)0x0b);
        }
        public static bool isPrimitiveTypeCode(SigmaTypeCode type)
        {
            return !isConstructorTypeCode(type);
        }

        public static bool isPrimitiveType(ISigmaType data)
        {
            return isPrimitiveTypeCode(data.type);
        }
        public static bool isEmbeddable(ISigmaCollection data)
        {
            return isEmbeddableTypeCode(data.elementsType);
        }
    }
}
