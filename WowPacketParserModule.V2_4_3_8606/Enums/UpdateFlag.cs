using System;

namespace WowPacketParser.Enums
{
    [Flags]
    enum UpdateFlag
    {
        None              = 0x000,
        Self              = 0x001,
        Transport         = 0x002,
        AttackingTarget   = 0x004,
        LowGuid           = 0x008,
        Unknown1          = 0x008,
        HighGuid          = 0x010,
        Living            = 0x020,
        StationaryObject  = 0x040,
    }
}
