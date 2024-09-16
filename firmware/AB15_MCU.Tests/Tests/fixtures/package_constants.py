class PakageConstantsSource:
    '''Source class for package constants'''
    # Fields position in package (both send and receive)
    # NOTE: CRC, END byte positions are defined from end of the package
    START_BYTE_POSITION      = 0
    MSG_ID_POSITION          = 1
    ASIC_ID_POSITION         = 2
    CMD_STATUS_POSITION      = 3
    PAYLOAD_LENGTH_POSITION  = 4
    PAYLOAD_POSITION         = 5
    CRC_POSITION             = -2 
    END_BYTE_POSITION        = -1

    # Fields length in bytes in package (both send and receive)
    # NOTE: for variable length field (payload) max length is provided
    START_BYTE_LENGTH        = 1
    MSG_ID_LENGTH            = 1
    ASIC_ID_LENGTH           = 1
    CMD_STATUS_LENGTH        = 1
    PAYLOAD_LENGTH_LENGTH    = 1
    PAYLOAD_LENGTH           = 255
    CRC_LENGTH               = 1
    END_BYTE_LENGTH          = 1

    # Minimum length of full package in bytes
    MIN_PACKAGE_LENGTH       = 7

    # Constant values
    START_BYTE_VALUE         = 0xAB
    END_BYTE_VALUE           = 0xBA

    # Avoid changing constants
    def __setattr__(self, name, value):
        raise TypeError("Constants are immutable")


# Use this variable to get package constants
pkg_const = PakageConstantsSource()