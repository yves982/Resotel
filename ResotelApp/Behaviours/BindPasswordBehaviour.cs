using ResotelApp.Utils;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ResotelApp.Behaviours
{
    class BindPasswordBehaviour : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.Register("BoundPassword", typeof(SecureString), typeof(BindPasswordBehaviour),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, _onPasswordChanged));


        public SecureString BoundPassword
        {
            get { return (SecureString) GetValue(BoundPasswordProperty); }
            set { SetValue(BoundPasswordProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += _onPasswordBoxChanged;
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= _onPasswordBoxChanged;
        }

        private void _onPasswordBoxChanged(object sender, RoutedEventArgs e)
        {
            BoundPassword = AssociatedObject.SecurePassword;
        }

        private static void _onPasswordChanged(DependencyObject sender, DependencyPropertyChangedEventArgs dpcea)
        {
            BindPasswordBehaviour behaviour = sender as BindPasswordBehaviour;
            if (behaviour == null)
            {
                return;
            }

            if(behaviour.AssociatedObject != null && SecureStringUtil.Read((SecureString)dpcea.NewValue) != behaviour.AssociatedObject.Password)
            {
                behaviour.AssociatedObject.Password = SecureStringUtil.Read((SecureString)dpcea.NewValue);
            }
        }
    }
}
