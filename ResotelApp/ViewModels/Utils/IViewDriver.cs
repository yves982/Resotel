namespace ResotelApp.ViewModels.Utils
{
    interface IViewDriver
    {
        void ShowView<T>(T viewModel) where T : class;
        void CloseAndShowNewMainWindow<T>(T viewModel) where T : class;
    }
}
