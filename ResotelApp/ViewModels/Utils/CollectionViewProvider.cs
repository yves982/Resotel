using System;
using System.Collections;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Utils
{
    class CollectionViewProvider
    {
        public static Func<IEnumerable, ICollectionView> Provider { get; set; }
    }
}
