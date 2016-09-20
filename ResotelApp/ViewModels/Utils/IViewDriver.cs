namespace ResotelApp.ViewModels.Utils
{
    /// <summary>
    /// Used to decouple ViewModels from Views
    /// </summary>
    interface IViewDriver
    {
        void ShowView<T>(T viewModel) where T : class;
        void CloseAndShowNewMainWindow<T>(T viewModel) where T : class;
    }
}
