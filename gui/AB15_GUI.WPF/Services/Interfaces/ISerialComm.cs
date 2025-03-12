using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Services.Interfaces
{
    /// <summary>
    /// Low-level class interface to work with COM port
    /// </summary>
    public interface ISerialComm
    {
        /// <summary>
        /// Tread-safe input buffer (data from MCU)
        /// </summary>
        ConcurrentQueue<byte> ReceiveBuffer { get; }

        /// <summary>
        /// Property used for overwriting automatic port detection
        /// Default value corresponds to automatic COM port selection
        /// </summary>
        string? ManualCOMPortName { get; set; }

        /// <summary>
        /// Connect/reconnect to USB COM port
        /// </summary>
        /// <returns>true if connected, false if failed</returns>
        bool ConnectCOMPort();

        /// <summary>
        /// Disconnect from USB COM port
        /// </summary>
        /// <returns>true if disconnected, false if failed</returns>
        bool DisconnectCOMPort();
        
        /// <summary>
        /// Automatically detect all COM ports with ShieldBuddy's connected to them
        /// </summary>
        /// <returns>Port numbers list in format COMX (X number); empty list if port wasn't found</returns>
        List<string> GetCOMPorts();
        
        /// <summary>
        /// Send array of bytes via virtualCOM port (must be already open)
        /// </summary>
        /// <param name="dataToSend">array with bytes to send to MCU</param>
        /// <param name="length">length of valid bytes in array</param>
        void Write(byte[] sendData, int length);
    }
}