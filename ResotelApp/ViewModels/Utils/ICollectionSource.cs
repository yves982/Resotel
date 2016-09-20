using System.ComponentModel;

namespace ResotelApp.ViewModels.Utils
{
    /// <summary>
    /// Used to decouble ViewModel from View
    /// </summary>
    public interface ICollectionViewSource
    {
        ICollectionView View { get; }
    }
}