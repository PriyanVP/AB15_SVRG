using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AB15_GUI.WPF.Models
{
    /// <summary>
    /// Class that used for commands without payload
    /// </summary>
    public class EmptyPayload : IByteListSerializable
    {
        /// <summary>
        /// Convert byte list to field values
        /// </summary>
        /// <param name="rawData">Data that should be converted to parameters values</param>
        public void Deserialize(List<byte> rawData)
        {
            // empty
        }

        /// <summary>
        /// Converts payload data to byte list
        /// </summary>
        /// <returns>Empty list</returns>
        public List<byte> Serialize()
        {
            return new List<byte>();
        }
    }
}
