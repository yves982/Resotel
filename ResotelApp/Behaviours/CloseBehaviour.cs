using System.Windows;

namespace ResotelApp.Behaviours
{
    static class CloseBehaviour
    {
        public static readonly DependencyProperty ShouldCloseProperty =
            DependencyProperty.RegisterAttached("ShouldClose", typeof(bool?), typeof(CloseBehaviour),
            new PropertyMetadata(_shouldCloseChanged));

        public static bool? GetShouldClose(DependencyObject dObj)
        {
            return (bool?)dObj.GetValue(ShouldCloseProperty);
        }

        public static void SetShouldClose(DependencyObject dObj, bool? value)
        {
            dObj.SetValue(ShouldCloseProperty, value);
        }

        private static void _shouldCloseChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs dpcea)
        {
            bool? shouldClose = GetShouldClose(dObj);
            if (dObj is Window && shouldClose.HasValue && shouldClose.Value)
            {
                ((Window)dObj).Close();
            }
        }
    }
}
