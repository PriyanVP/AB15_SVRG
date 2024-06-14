using AB15_GUI.WPF.Models.Interfaces;
using System;

namespace AB15_GUI.WPF.Services.Interfaces
{
    public interface IWaitlist
    {
        (int msgID, bool isAddedSuccessfully) AddItemToWaitlist(Action<IReceiveCommunicationPackage> deleg, Type payloadType, bool isContinuous = false);
        void ClearWaitlist();
        Action<IReceiveCommunicationPackage>? GetDelegate(IReceiveCommunicationPackage receivedPackage);
        Type? GetPayloadType(int msgID);
        bool RemoveItemFromWaitlist(int? msgID);
    }
}