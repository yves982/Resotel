﻿using ResotelApp.ViewModels.Utils;
using System.ComponentModel;
using System.Windows.Data;

namespace ResotelApp
{
    class CollectionViewSourceImpl : ICollectionViewSource
    {
        private CollectionViewSource _cvs;

        public ICollectionView View
        {
            get { return _cvs.View; }
        }

        public CollectionViewSourceImpl(CollectionViewSource cvs)
        {
            _cvs = cvs;
        }
    }
}
