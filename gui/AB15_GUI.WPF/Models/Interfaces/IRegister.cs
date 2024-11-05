using System;

namespace AB15_GUI.WPF.Models.Interfaces
{
    /// <summary>
    /// Interface exposing common properties of Register classes
    /// </summary>
    public interface IRegister
    {
        /// <summary>
        /// Reset value for register
        /// </summary>
        ushort ResetValue { get; }

        /// <summary>
        /// Name of the register
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Absolute address of the register
        /// </summary>
        static ushort Address { get; }

        /// <summary>
        /// Description
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Access level of this register
        /// </summary>
        string? Access { get; }

        /// <summary>
        /// Data property. Constructs register value from fields on get. 
        /// Deconstruct register value by fields on set
        /// </summary>
        ushort Data { get; set; }
    }
}