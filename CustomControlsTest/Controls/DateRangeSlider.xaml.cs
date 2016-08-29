using System;
using System.Windows;
using System.Windows.Controls;

namespace CustomControlsTest.Controls
{
    /// <summary>
    /// Logique d'interaction pour DoubleSidedSlider.xaml
    /// </summary>
    public partial class DateRangeSlider : UserControl
    {

        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }

            set { SetValue(StartDateProperty, value); }
        }

        public DateTime EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }

            set { SetValue(EndDateProperty, value); }
        }

        public DateTime MinDate
        {
            get { return (DateTime)GetValue(MinDateProperty); }

            set { SetValue(MinDateProperty, value); }
        }

        public DateTime MaxDate
        {
            get { return (DateTime)GetValue(MaxDateProperty); }

            set { SetValue(MaxDateProperty, value); }
        }

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(DateRangeSlider), 
                new UIPropertyMetadata(DateTime.Now, _startDate_changed));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime), typeof(DateRangeSlider), 
                new UIPropertyMetadata(DateTime.Now.AddDays(1.0), _endDate_changed));

        public static readonly DependencyProperty MinDateProperty =
            DependencyProperty.Register("MinDate", typeof(DateTime), typeof(DateRangeSlider),
                new UIPropertyMetadata(default(DateTime)));

        public static readonly DependencyProperty MaxDateProperty =
            DependencyProperty.Register("MaxDate", typeof(DateTime), typeof(DateRangeSlider),
                new UIPropertyMetadata(default(DateTime)));

        public DateRangeSlider()
        {
            InitializeComponent();
        }

        private static void _swapDates(DateTime date1, DateTime date2)
        {
            DateTime tmpDate = date1;
            date1 = date2;
            date2 = tmpDate;
        }

        private static void _startDate_changed(DependencyObject sender, DependencyPropertyChangedEventArgs dpcea)
        {
            DateRangeSlider slider = (DateRangeSlider)sender;
            if (slider.StartDate > slider.EndDate)
            {
                _swapDates(slider.StartDate, slider.EndDate);
            }
            else if (slider.StartDate.Equals(slider.EndDate))
            {
                slider.StartDate = slider.EndDate - TimeSpan.FromSeconds(1);
            }
        }

        private static void _endDate_changed(DependencyObject sender, DependencyPropertyChangedEventArgs dpcea)
        {
            DateRangeSlider slider = (DateRangeSlider)sender;
            if (slider.StartDate > slider.EndDate)
            {
                _swapDates(slider.StartDate, slider.EndDate);
            }
            else if (slider.EndDate.Equals(slider.StartDate))
            {
                slider.EndDate = slider.StartDate + TimeSpan.FromSeconds(1);
            }
        }
    }
}
