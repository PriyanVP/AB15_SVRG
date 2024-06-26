using System.Collections.Generic;

namespace AB15_GUI.WPF.Models.Interfaces
{
    /// <summary>
    /// Interface exposing methods for objects that hold payload data
    /// </summary>
    public interface IByteListSerializable
    {
        /// <summary>
        /// Convert information stored in object to List of bytes
        /// </summary>
        /// <returns>object data in form of List of bytes</returns>
        List<byte> Serialize();
      
        /// <summary>
        /// Convert List of bytes containing object information back to object fields
        /// </summary>
        /// <param name="status">status field of package</param>
        /// <param name="rawData">List of bytes containing object information</param>
        void Deserialize(MCUStatus status, List<byte> rawData);
    }
}