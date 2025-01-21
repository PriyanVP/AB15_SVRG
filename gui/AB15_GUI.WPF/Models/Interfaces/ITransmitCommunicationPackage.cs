using System;
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
        int? MsgID { get; set; }

        /// <summary>
        /// ASIC ID
        /// </summary>
        int ASICID { get; set; }

        /// <summary>
        /// Command to send in package
        /// </summary>
        MCUCommand Cmd { get; set; }

        /// <summary>
        /// Flag to indicate if few responses can be received for this package
        /// </summary>
        public bool IsContinuous { get; set; }

        /// <summary>
        /// Receive package payload type
        /// </summary>
        public Type? PayloadType { get; set; }

        /// <summary>
        /// Delegate that will be called for received msg
        /// </summary>
        public Action<IReceiveCommunicationPackage>? Deleg { get; set; }

        /// <summary> 
        /// Flag to indicate if package is valid
        /// </summary>
        bool IsPackageValid { get; }
       
        /// <summary>
        /// Format data to send in byte list suitable for use in communication
        /// </summary>
        /// <returns>List with bytes to be send to MCU</returns>
        List<byte> GetPackage();
    }
}