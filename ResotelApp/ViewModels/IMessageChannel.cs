using System;
using ResotelApp.ViewModels.Utils;

namespace ResotelApp.ViewModels
{
    public interface IMessageChannel
    {
        event Action<IMessageChannel, MessageTypes, object> MessageReceived;
    }
}