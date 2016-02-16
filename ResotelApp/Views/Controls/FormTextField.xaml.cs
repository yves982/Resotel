using System.Windows;
using System.Windows.Controls;

namespace ResotelApp.Views.Controls
{
    /// <summary>
    /// Logique d'interaction pour FormField.xaml
    /// </summary>
    public partial class FormTextField : UserControl
    {
        public static readonly DependencyProperty FieldLabelProp 
            = DependencyProperty.Register("FieldLabel", typeof(string), typeof(FormTextField), new FrameworkPropertyMetadata(""));
        public static readonly DependencyProperty RequiredProp
             = DependencyProperty.Register("Required", typeof(bool), typeof(FormTextField), new FrameworkPropertyMetadata(false));

        public static DependencyProperty FieldValueProp
            = DependencyProperty.Register("FieldValue", typeof(string), typeof(FormTextField), new PropertyMetadata(""));
        
        public string FieldLabel
        {
            get { return (string)GetValue(FieldLabelProp); }
            set { SetValue(FieldLabelProp, value); }
        }
        public bool Required
        {
            get { return (bool)GetValue(RequiredProp); }
            set { SetValue(RequiredProp, value); }
        }

        public string RequiredContent
        {
            get { return "(*) " +FieldLabel; }
        }

        public string FieldValue
        {
            get { return (string)GetValue(FieldValueProp); }
            set { SetValue(FieldValueProp, value); }
        }

        public FormTextField()
        {
            InitializeComponent();
            
        }
    }
}
