using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    public interface IMessageHandler
    {
       void HandleMessage(IMessageChannel source, MessageTypes type, Object data);
    }
}
