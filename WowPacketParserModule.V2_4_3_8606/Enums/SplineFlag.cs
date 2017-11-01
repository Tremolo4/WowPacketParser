﻿using System;

namespace WowPacketParserModule.V2_4_3_8606.Enums
{
    [Flags]
    public enum SplineFlag : uint
    {
        None         = 0x00000000,
        Done         = 0x00000001,
        Falling      = 0x00000002,           // Affects elevation computation
        Unknown3     = 0x00000004,
        Unknown4     = 0x00000008,
        Unknown5     = 0x00000010,
        Unknown6     = 0x00000020,
        Unknown7     = 0x00000040,
        Unknown8     = 0x00000080,
        Runmode      = 0x00000100,
        Flying       = 0x00000200,           // Smooth movement(Catmullrom interpolation mode), flying animation
        No_Spline    = 0x00000400,
        Unknown12    = 0x00000800,
        Unknown13    = 0x00001000,
        Unknown14    = 0x00002000,
        Unknown15    = 0x00004000,
        Unknown16    = 0x00008000,
        Final_Point  = 0x00010000,
        Final_Target = 0x00020000,
        Final_Angle  = 0x00040000,
        Unknown19    = 0x00080000,           // exists, but unknown what it does
        Cyclic       = 0x00100000,           // Movement by cycled spline
        Enter_Cycle  = 0x00200000,           // Everytimes appears with cyclic flag in monster move packet, erases first spline vertex after first cycle done
        Frozen       = 0x00400000,           // Will never arrive
        Unknown23    = 0x00800000,
        Unknown24    = 0x01000000,
        Unknown25    = 0x02000000,          // exists, but unknown what it does
        Unknown26    = 0x04000000,
        Unknown27    = 0x08000000,
        Unknown28    = 0x10000000,
        Unknown29    = 0x20000000,
        Unknown30    = 0x40000000,
        Unknown31    = 0x80000000,
    }
}
