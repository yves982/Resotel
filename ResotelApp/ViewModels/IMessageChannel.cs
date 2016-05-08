using ResotelApp.ViewModels.Utils;
using System;

namespace ResotelApp.ViewModels
{
    public interface IMessageChannel
    {
        event Action<IMessageChannel, MessageTypes, object> MessageReceived;
    }
}