using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

namespace ResotelApp.ViewModels
{
    class SumUpViewModel : INavigableViewModel, INotifyPropertyChanged
    {
        private PropertyChangeSupport _pcs;
        private string _title;
        private DateRange _dates;
        private LinkedList<INavigableViewModel> _navigation;
        private List<AppliedPackEntity> _appliedPackEntities;
        private List<OptionChoiceEntity> _optionChoiceEntities;
        private ClientEntity _clientEntity;
        private XpsDocument _xpsDoc;
        private double _optionsTotal;
        private double _roomsTotal;

        private DelegateCommandAsync<SumUpViewModel> _editBookingCommand;
        private DelegateCommand<XpsDocument> _printBookingCommand;
        private Booking _booking;

        public LinkedList<INavigableViewModel> Navigation
        {
            get { return _navigation; }
        }

        public string Title
        {
            get { return _title; }
        }

        public DateRange Dates
        {
            get { return _dates; }
        }

        public IList<AppliedPackEntity> AppliedPackEntities
        {
            get { return _appliedPackEntities; }
        }

        public IList<OptionChoiceEntity> OptionChoiceEntities
        {
            get { return _optionChoiceEntities; }
        }

        public ClientEntity ClientEntity
        {
            get { return _clientEntity; }
        }

        public double OptionsTotal
        {
            get
            {
                _optionsTotal = 0;
                foreach(OptionChoiceEntity optChoice in _optionChoiceEntities)
                {
                    _optionsTotal += optChoice.ActualPrice;
                }
                return _optionsTotal;
            }
        }

        public double RoomsTotal
        {

            get
            {
                _roomsTotal = 0;
                foreach (AppliedPackEntity appliedPackEnity in _appliedPackEntities)
                {
                    _roomsTotal += appliedPackEnity.Price * appliedPackEnity.Count;
                }
                return _roomsTotal;
            }
        }

        public double Total
        {
            get { return _roomsTotal + _optionsTotal; }
        }

        public string Siret
        {
            get { return "774 082 010 00034"; }
        }

        public ICommand PrintBookingCommand
        {
            get { return _printBookingCommand; }
        }

        public ICommand EditBookingCommand
        {
            get { return _editBookingCommand; }
        }

        public event EventHandler<INavigableViewModel> NextCalled;
        public event EventHandler<INavigableViewModel> PreviousCalled;
        public event EventHandler<INavigableViewModel> Shutdown;
        public event EventHandler<string> MessageReceived;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _pcs.Handler += value; }
            remove { _pcs.Handler -= value; }
        }

        public SumUpViewModel(LinkedList<INavigableViewModel> navigation, Booking booking)
        {
            _pcs = new PropertyChangeSupport(this);
            _navigation = navigation;
            _booking = booking;
            _dates = booking.Dates;
            _clientEntity = new ClientEntity(booking.Client);
            _appliedPackEntities = new List<AppliedPackEntity>();
            _optionChoiceEntities = new List<OptionChoiceEntity>(booking.OptionChoices.Count);
            
            _title = $"Réservation de {_clientEntity.FirstName} {_clientEntity.LastName} du {booking.Dates.Start:dd/MM/yyyy}";

            List<OptionChoiceEntity> optChoiceEntities = booking.OptionChoices.ConvertAll(
                optChoice => new OptionChoiceEntity(optChoice)
            );

            _optionChoiceEntities.AddRange(optChoiceEntities);

            foreach (Room room in booking.Rooms)
            {
                BookedRoomEntity bookedRoomEntity = new BookedRoomEntity(booking, room);
                foreach(AppliedPackEntity appliedPackEntity in bookedRoomEntity.AppliedPackEntities)
                {
                    _appliedPackEntities.Add(appliedPackEntity);
                }
            }

            _editBookingCommand = new DelegateCommandAsync<SumUpViewModel>(_editBooking);
            _printBookingCommand = new DelegateCommand<XpsDocument>(_printBooking);

            _navigation.AddLast(this);
        }

        private void _printBooking(XpsDocument xpsDoc)
        {
            
                Thread printThread = new Thread(() =>
                {
                    try
                    {
                        PrintQueue defaultPrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                        string jobName = $"Facture - {_clientEntity.FirstName} {_clientEntity.LastName} - {_clientEntity.BirthDate:dd/MM/yyyy}";
                        PrintSystemJobInfo xpsPrintJob = defaultPrintQueue.AddJob(jobName, "flowDocument.xps", false);
                    }
                    catch (PrintJobException e)
                    {
                        Console.WriteLine("\n\t{0} could not be added to the print queue.", xpsDoc.Uri.AbsoluteUri);
                        if (e.InnerException.Message == "File contains corrupted data.")
                        {
                            Console.WriteLine("\tIt is not a valid XPS file. Use the isXPS Conformance Tool to debug it.");
                        }
                    }
                });

            printThread.SetApartmentState(ApartmentState.STA);
            printThread.Start();
        }

        private async Task _editBooking(SumUpViewModel sumUpVM)
        {
            BookingViewModel bookingVM = await BookingViewModel.LoadAsync(_navigation, _booking);
            NextCalled?.Invoke(this, sumUpVM);
        }
    }
}
