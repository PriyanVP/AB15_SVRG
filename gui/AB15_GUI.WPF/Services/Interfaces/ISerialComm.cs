using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AB15_GUI.WPF.Services.Interfaces
{
    /// <summary>
    /// Low-level class interface to work with COM port
    /// </summary>
    public interface ISerialComm
    {
        ConcurrentQueue<byte> ReceiveBuffer { get; }

        bool ConnectCOMPort();
        List<string> GetCOMPorts();
        void Write(byte[] sendData, int length);
    }
}