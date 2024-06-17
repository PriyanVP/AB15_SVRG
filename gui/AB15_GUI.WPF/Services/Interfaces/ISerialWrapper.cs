using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Services.Interfaces
{
    /// <summary>
    /// Interface for serial communication wrapper
    /// </summary>
    public interface ISerialWrapper
    {
        /// <summary>
        /// List of COM ports with Shieldbuddy connected to them
        /// </summary>
        List<string> AvailableCOMPorts { get; }
        
        /// <summary>
        /// Checks if MCU COM port present in COM ports list
        /// </summary>
        bool IsCOMPortPresent { get; }
        
        /// <summary>
        /// COM port name when selected manually
        /// Default value null corresponds to automatic port detection
        /// </summary>
        string? ManualComPortName { get; set; }

        /// <summary>
        /// Serial wrapper destruction method. Required for testability and correct timer handling
        /// </summary>
        void Dispose();
        
        /// <summary>
        /// Reconnect to USB COM port
        /// </summary>
        /// <returns>true if connected, false if failed</returns>
        bool ReconnectCOMPort();
        
        /// <summary>
        /// Removes item from waitlist by msg_id
        /// </summary>
        /// <param name="msgID">message ID of command that should be removed</param>
        /// <returns>true if removal was successful, false - otherwise</returns>        
        bool RemoveWaitlistItem(int? msgID);
        
        /// <summary>
        /// Looks for delegates that will handle package and removes relevant items from waitlist
        /// </summary>
        /// <param name="packageToSend">package that will be send via serial port</param>
        /// <param name="deleg">delegate that will be called after receiving response from MCU</param>
        /// <param name="isContinuous">flag to define if MCU response can be received few time</param>
        /// <returns>true if all operations were performed succesfully, false - otherwise (message wasn't send)</returns>
        bool SerialWrite(ITransmitCommunicationPackage packageToSend, Action<IReceiveCommunicationPackage>? deleg = null, bool isContinuous = false);
    }
}