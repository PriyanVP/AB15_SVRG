using AB15_GUI.WPF.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AB15_GUI.WPF.Services.Interfaces
{
    /// <summary>
    /// Waitlist interface
    /// </summary>
    public interface IWaitlist
    {
        /// <summary>
        /// Add item to waitlist. Will return message id and awaitable task
        /// </summary>
        /// <param name="payloadType">payload type</param>
        /// <param name="isContinuous">flag to define if MCU response can be received few times</param>
        /// <returns>pair of msgID and task; will be nulls if unsuccessful</returns>
        (int? msgID, Task<IReceiveCommunicationPackage>? task) AddItemToWaitlist(Type payloadType, bool isContinuous = false);

        /// <summary>
        /// Handle responses from MCU
        /// </summary>
        /// <param name="package">raw package from MCU</param>
        void HandleResponse(List<byte> package);

        /// <summary>
        /// Removes outdated items from waitlist
        /// </summary>
        /// <returns>Nothing</returns>
        void RemoveOutdatedItems();

        /// <summary>
        /// Remove item from waitlist by ID. Intended for removing continuous items
        /// </summary>
        /// <param name="msgID">message ID</param>
        /// <returns>true if removal was successful, false - otherwise</returns>
        bool RemoveItemFromWaitlist(int? msgID);

        /// <summary>
        /// Get task instance from waitlist by ID
        /// To be used for continuous communication (few answers on one request)
        /// </summary>
        /// <param name="msgID">message ID</param>
        /// <returns>Task instance or null if not found</returns>
        Task<IReceiveCommunicationPackage?> GetContinuousTaskInstance(int msgID);

        /// <summary>
        /// Remove all items from waitlist
        /// WARNING: may cause stuck of await operations
        /// </summary>
        void ClearWaitlist();
    }
}