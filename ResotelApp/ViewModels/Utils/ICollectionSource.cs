using System.ComponentModel;

namespace ResotelApp.ViewModels.Utils
{
    public interface ICollectionViewSource
    {
        ICollectionView View { get; }
    }
}