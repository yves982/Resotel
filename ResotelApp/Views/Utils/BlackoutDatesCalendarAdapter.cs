using ResotelApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ResotelApp.Views.Utils
{
    public class BlackoutDatesCalendarAdapter
    {
        public static List<DateTime> GetBlackOutDates(DependencyObject obj)
        {
            return (List<DateTime>)obj.GetValue(BlackOutDatesProperty);
        }
        public static void SetBlackOutDates(DependencyObject obj, List<DateTime> value)
        {
            obj.SetValue(BlackOutDatesProperty, value);

        }

        public static readonly DependencyProperty BlackOutDatesProperty =
            DependencyProperty.RegisterAttached("BlackOutDates", typeof(List<DateTime>), typeof(BlackoutDatesCalendarAdapter), new PropertyMetadata(null, _onBlackOutDatesPropertyChanged));


        private static void _onBlackOutDatesPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Calendar calendar = sender as Calendar;
            List<DateTime> dates = (List<DateTime>)e.NewValue;
            foreach(DateTime date in dates)
            {
                calendar.BlackoutDates.Add(new CalendarDateRange(date));
            }
        }
    }
}
