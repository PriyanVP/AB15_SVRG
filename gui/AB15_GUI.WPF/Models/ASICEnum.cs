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
}