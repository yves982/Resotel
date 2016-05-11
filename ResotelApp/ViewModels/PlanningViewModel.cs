using ResotelApp.DAL;
using ResotelApp.Models;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ResotelApp.ViewModels
{
    class PlanningViewModel : INotifyPropertyChanged
    {

        public DelegateCommandAsync<object> OnLoadCommand { get; }
        
        public string Title { get; private set; }
        public DateRange TimeFrame { get; set; }
        public List<DateTime> BlackoutDates { get; set; }

        public PlanningViewModel()
        {
            Title = TranslationHandler.GetString("PlanningTitle");
            OnLoadCommand = new DelegateCommandAsync<object>(_load);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task _load(object obj)
        {
            BlackoutDates = new List<DateTime>();
            List<Booking> bookings = await BookingDAL.GetAllBookings();
            foreach(DateTime d in bookings.Select(b => b.Date))
            {
                BlackoutDates.Add(d);
            }
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("BlackoutDates"));
            }
        }
    }
}
