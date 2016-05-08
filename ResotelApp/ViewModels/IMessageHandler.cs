using ResotelApp.ViewModels.Utils;
using System;

namespace ResotelApp.ViewModels
{
    public interface IMessageHandler
    {
       void HandleMessage(IMessageChannel source, MessageTypes type, Object data);
    }
}
