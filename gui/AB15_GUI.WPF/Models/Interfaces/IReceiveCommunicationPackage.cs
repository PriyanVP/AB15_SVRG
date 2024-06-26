using System.Collections.Generic;

namespace AB15_GUI.WPF.Models.Interfaces
{
    /// <summary>
    /// Interface for packages that are received
    /// </summary>
    public interface IReceiveCommunicationPackage
    {
        /// <summary>
        /// Message ID
        /// </summary>
        int MsgID { get; }

        /// <summary>
        /// ASIC ID
        /// </summary>
        int ASICID { get; }

        /// <summary>
        /// Status received in package
        /// </summary>
        MCUStatus Status { get; set; }

        /// <summary>
        /// Flag to indicate if package is valid
        /// </summary>
        bool IsPackageValid { get; }
       
        /// <summary>
        /// Unpack data from received byte array to format suitable for further use in application
        /// <param name="receivedPackage">array with one full package received by serial communication</param>
        /// <returns>Flag signalizing if unpacked data is valid</returns>
        bool UnpackPackage(List<byte> receivedPackage);
    }
}