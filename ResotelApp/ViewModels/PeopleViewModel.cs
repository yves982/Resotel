using ResotelApp.ViewModels.Utils;

namespace ResotelApp.ViewModels
{
    class PeopleViewModel
    {
        public string Title { get; set; }
        public int AdultsCount { get; set; }
        public int BabiesCount { get; set; }

        public PeopleViewModel()
        {
            Title = TranslationHandler.GetString("PeopleTitle");
        }
    }
}
