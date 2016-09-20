using ResotelApp.Models;

namespace ResotelApp.ViewModels.Entities
{
    /// <summary>
    /// ViewModel pendant of Pack with changes notifications
    /// </summary>
    class PackEntity
    {
        private Pack pack;

        public PackEntity(Pack pack)
        {
            this.pack = pack;
        }

        public Pack Pack { get; internal set; }
    }
}