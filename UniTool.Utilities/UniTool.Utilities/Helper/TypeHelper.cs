using System;

namespace UniTool.Utilities
{
    public static class TypeHelper
    {

        public static readonly TypeCode[] IntegerTypes =
        {
            TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32,
            TypeCode.Int64, TypeCode.UInt64
        };

        public static readonly TypeCode[] FloatingPointTypes =
        {
            TypeCode.Double, TypeCode.Single, TypeCode.Decimal
        };
    }
}