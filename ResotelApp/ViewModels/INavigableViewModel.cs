
using ResotelApp.ViewModels.Entities;
using System;
using System.Collections.Generic;

namespace ResotelApp.ViewModels
{
    /// <summary> Interface for top level viewmodels (tab or windows) </summary>
    interface INavigableViewModel : IEntity
    {
        string Title { get; }
        LinkedList<INavigableViewModel> Navigation { get; }
        event EventHandler<INavigableViewModel> NextCalled;
        event EventHandler<INavigableViewModel> PreviousCalled;
        event EventHandler<INavigableViewModel> Shutdown;
        event EventHandler<string> MessageReceived; 
    }
}
