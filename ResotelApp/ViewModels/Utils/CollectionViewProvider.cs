using System;
using System.Collections;

namespace ResotelApp.ViewModels.Utils
{
    /// <summary> Used to decouple ViewModels from Views</summary>
    /// <remarks>The Provider will return an ICollectionViewSource so we could keep a reference to it to avoid garbage collector erasing the source</remarks>
    class CollectionViewProvider
    {
        public static Func<IEnumerable, ICollectionViewSource> Provider { get; set; }
    }
}
