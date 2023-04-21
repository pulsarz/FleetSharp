﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FleetSharp.Sigma
{
    enum SigmaTypeCode
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
}
