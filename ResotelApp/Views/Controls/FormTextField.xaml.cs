using System.Windows;
using System.Windows.Controls;

namespace ResotelApp.Views.Controls
{
    /// <summary>
    /// Logique d'interaction pour FormField.xaml
    /// </summary>
    public partial class FormTextField : UserControl
    {
        public static readonly DependencyProperty FieldContentProp 
            = DependencyProperty.Register("FieldContent", typeof(string), typeof(FormTextField), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty RequiredProp
             = DependencyProperty.Register("Required", typeof(bool), typeof(FormTextField), new FrameworkPropertyMetadata(false));
        
        public string FieldContent
        {
            get { return (string)GetValue(FieldContentProp); }
            set { SetValue(FieldContentProp, value); }
        }
        public bool Required
        {
            get { return (bool)GetValue(RequiredProp); }
            set { SetValue(RequiredProp, value); }
        }

        public string RequiredContent
        {
            get { return "(*) " +FieldContent; }
        }

        public FormTextField()
        {
            InitializeComponent();
            
        }
    }
}
