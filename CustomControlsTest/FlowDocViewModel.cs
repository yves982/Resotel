using ResotelApp.ViewModels.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace CustomControlsTest
{
    class FlowDocViewModel
    {
        private ObservableCollection<FlowDocDataItem> _items;
        private ICollectionView _cv;

        public ICollectionView Datas
        {
            get { return _cv; }
        }

        public string CanIBindStuffToRunNowadays
        {
            get { return "Most certainly sir!"; }
        }

        public string Coucou
        {
            get { return "Coucou"; }
        }

        public string Dupond
        {
            get { return "Dupond"; }
        }

        public FlowDocViewModel(params FlowDocDataItem [] items)
        {
            _items = new ObservableCollection<FlowDocDataItem>(items);
            _cv = CollectionViewSource.GetDefaultView(_items);
        }
    }
}
