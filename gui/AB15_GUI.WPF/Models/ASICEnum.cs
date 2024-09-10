using System;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// State of ASIC - received from SYSTEM_STATE register
    /// </summary>
    public enum ASICState
    {
        pre_init_mode   = 1 << 0,   // PRE_INIT_MODE
        init_mode       = 1 << 1,   // INIT_MODE
        test_mode1      = 1 << 2,   // TEST_MODE1
        test_mode2      = 1 << 3,   // TEST_MODE2
        normal_mode     = 1 << 4,   // NORMAL_MODE
        bootloading     = 1 << 5,   // ?
        lbist_run       = 1 << 6,   // POWER_UP2 -> STart_LBIST
        vup_vas_rampup  = 1 << 7,   // START_VUP, START_VAS
        fbist_run       = 1 << 8,   // FBIST
        vst_vcore_rampup= 1 << 9,   // ?
        npor_release    = 1 << 10,  // N_POR_RELEASE
        jtag_ready      = 1 << 11,  // ?
        cftm            = 1 << 12   // INIT_MODE -> CFTM
    }

    // public static class ASICStateHelper
    // {
    //     public static ASICState GetEnumFromSystemState(UInt16 regValue)
    //     {
    //         // Converter is required due to specifics of enum implementation in C#
    //         switch (regValue)
    //         {
    //             case (1 << 0):
    //                 return ASICState.pre_init_mode;
    //             case (1 << 1):
    //                 return ASICState.init_mode;
    //             case (1 << 2):
    //                 return ASICState.test_mode1;
    //             case (1 << 3):
    //                 return ASICState.test_mode2;
    //             case (1 << 4):
    //                 return ASICState.normal_mode;
    //             case (1 << 5):
    //                 return ASICState.bootloading;
    //             case (1 << 6):
    //                 return ASICState.lbist_run;
    //             case (1 << 7):
    //                 return ASICState.vup_vas_rampup;
    //             case (1 << 8):
    //                 return ASICState.fbist_run;
    //             case (1 << 9):
    //                 return ASICState.vst_vcore_rampup;
    //             case (1 << 10):
    //                 return ASICState.npor_release;
    //             case (1 << 11):
    //                 return ASICState.jtag_ready;
    //             case (1 << 12):
    //                 return ASICState.cftm;
    //             default:
    //                 // Unexpected value
    //                 return ASICState.INVALID;
    //         }
    //     }
    // }
}