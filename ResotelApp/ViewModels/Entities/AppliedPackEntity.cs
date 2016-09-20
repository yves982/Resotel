using ResotelApp.Models;
using System.ComponentModel;

namespace ResotelApp.ViewModels.Entities
{
    /// <summary>
    /// ViewModel pendant of AppliedPack with changes notifications
    /// </summary>
    class AppliedPackEntity : IEntity, IDataErrorInfo
    {
        private AppliedPack _appliedPack;

        public AppliedPack AppliedPack
        {
            get { return _appliedPack; }
        }

        public int Count
        {
            get { return _appliedPack.Count; }
        }

        public int Id
        {
            get { return _appliedPack.Id; }
        }

        public string Label
        {
            get { return _appliedPack.Label; }
        }

        public double Price
        {
            get { return _appliedPack.Price; }
        }

        public Room Room
        {
            get { return _appliedPack.Room; }
        }

        public Pack RoomPack
        {
            get { return _appliedPack.RoomPack; }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                return ((IDataErrorInfo)AppliedPack).Error;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return ((IDataErrorInfo)AppliedPack)[columnName];
            }
        }

        public AppliedPackEntity(AppliedPack appliedPack)
        {
            _appliedPack = appliedPack;
        }
    }
}
