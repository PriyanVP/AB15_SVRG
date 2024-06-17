using AB15_GUI.WPF.Models.Interfaces;
using System;

namespace AB15_GUI.WPF.Services.Interfaces
{
    /// <summary>
    /// Waitlist interface
    /// </summary>
    public interface IWaitlist
    {
        /// <summary>
        /// Add item to waitlist. Will return message id
        /// </summary>
        /// <param name="msgID">message ID</param>
        /// <param name="isAddedSuccessfully">flag to tell if item was added to waitlist</param>
        /// <param name="deleg">delegate that will be called for received msg</param>
        /// <param name="payloadType">payload type</param>
        /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
        /// <returns>pair of msgID and isAddedSuccessfully</returns>
        (int msgID, bool isAddedSuccessfully) AddItemToWaitlist(Action<IReceiveCommunicationPackage> deleg, Type payloadType, bool isContinuous = false);
        
        /// <summary>
        /// Removs all items from waitlist
        /// </summary>
        /// <param name="msgID">message ID</param>
        /// <returns>true if removal was successful, false - otherwise</returns>    
        void ClearWaitlist();
        
        /// <summary>
        /// Looks for delegate that will handle package and removes relevant item from waitlist
        /// </summary>
        /// <param name="receivedPackage">package from MCU</param>
        /// <returns>Delegate that should be called for this package or null if delegate is not found</returns>    
        Action<IReceiveCommunicationPackage>? GetDelegate(IReceiveCommunicationPackage receivedPackage);
        
        /// <summary>
        /// Gets payload type stored in waitlist item
        /// </summary>
        /// <param name="msgID">message ID in package from MCU</param>
        /// <returns>Type of package payload or null if no waitlist item was found</returns>    
        Type? GetPayloadType(int msgID);
        
        /// <summary>
        /// Remove item from waitlist by ID. Intended for removing continuous items
        /// </summary>
        /// <param name="msgID">message ID</param>
        /// <returns>true if removal was successful, false - otherwise</returns>
        bool RemoveItemFromWaitlist(int? msgID);
    }
}