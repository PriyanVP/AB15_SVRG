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
        /// TODO: add information
        /// </summary>
        List<string> AvailableCOMPorts { get; }
        bool IsCOMPortPresent { get; }
        string? ManualComPortName { get; set; }

        void Dispose();
        bool ReconnectCOMPort();
        bool RemoveWaitlistItem(int? msgID);
        bool SerialWrite(ITransmitCommunicationPackage packageToSend, Action<IReceiveCommunicationPackage>? deleg = null, bool isContinuous = false);
    }
}