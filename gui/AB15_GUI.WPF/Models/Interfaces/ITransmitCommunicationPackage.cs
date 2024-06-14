using System.Collections.Generic;

namespace AB15_GUI.WPF.Models.Interfaces
{
    /// <summary>
    /// Interface for packages that are transmitted
    /// </summary>
    public interface ITransmitCommunicationPackage
    {
        /// <summary>
        /// Message ID
        /// </summary>
        int MsgID { get; set; }

        /// <summary>
        /// ASIC ID
        /// </summary>
        int ASICID { get; set; }

        /// <summary>
        /// Command to send in package
        /// </summary>
        MCUCommand Cmd { get; set; }

        /// <summary>
        /// TODO: redo?
        /// </summary>
        /// <value></value>
        bool IsPackageValid { get; }
       
        /// <summary>
        /// Format data to send in byte list suitable for use in communication
        /// </summary>
        /// <returns>List with bytes to be send to MCU</returns>
        List<byte> GetPackage();
    }
}