﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace ResotelApp.Views.Utils
{
    [ValueConversion(typeof(string), typeof(UserControl))]
    public class UserControlLoader : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UserControl control = null;
            if (value != null)
            {
                Type controlType = Type.GetType("ResotelApp.Views.Controls." + value.ToString());
                Type controlModelType = Type.GetType("ResotelApp.ViewModels." + value.ToString() + "Model");

                if (controlType != null)
                {
                    control = (UserControl)Activator.CreateInstance(controlType.Assembly.FullName, controlType.FullName).Unwrap();
                }

                if (controlModelType != null)
                {
                    object model = Activator.CreateInstance(controlModelType.Assembly.FullName, controlModelType.FullName).Unwrap();
                    control.DataContext = model;
                }
            }
            return control;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}