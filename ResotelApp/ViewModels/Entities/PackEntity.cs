using ResotelApp.Models;

namespace ResotelApp.ViewModels.Entities
{
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