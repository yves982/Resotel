using System.Globalization;
using System.Resources;

namespace ResotelApp.ViewModels.Utils
{
    static class TranslationHandler
    {
        private static ResourceManager _res;

        static TranslationHandler()
        {
            _res = new ResourceManager("Resources", typeof(TranslationHandler).Assembly);
        }

        public static string GetString(string msgKey)
        {
            return Properties.Resources.ResourceManager.GetString(msgKey);
        }
    }
}
