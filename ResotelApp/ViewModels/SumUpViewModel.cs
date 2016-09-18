using ResotelApp.DAL;
using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        private double _optionsTotal;
        private double _roomsTotal;

        private DelegateCommandAsync<SumUpViewModel> _editBookingCommand;
        private DelegateCommandAsync<SumUpViewModel> _validateBookingCommand;
        private DelegateCommand<XpsDocument> _printBookingCommand;
        private Booking _booking;
        private double _tva;

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

        public double TotalHT
        {
            get { return _roomsTotal + _optionsTotal; }
        }

        public double Total
        {
            get { return (_roomsTotal + _optionsTotal) * (1d + _tva / 100d); }
        }

        public double Tva
        {
            get { return _tva; }
        }

        public bool NeedsValidation
        {
            get
            {
                return _booking.State == BookingState.Draft;
            }
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

        public ICommand ValidateBookingCommand
        {
            get { return _validateBookingCommand; }
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

        public SumUpViewModel(LinkedList<INavigableViewModel> navigation, Booking booking, LinkedListNode<INavigableViewModel> prevNode = null)
        {
            _pcs = new PropertyChangeSupport(this);
            _navigation = navigation;
            _booking = booking;
            _dates = booking.Dates;
            _clientEntity = new ClientEntity(booking.Client);
            _appliedPackEntities = new List<AppliedPackEntity>();
            _optionChoiceEntities = new List<OptionChoiceEntity>(booking.OptionChoices.Count);

            foreach(OptionChoice optChoiceEntity in booking.OptionChoices)
            {
                OptionChoiceEntity optCEntity = new Entities.OptionChoiceEntity(optChoiceEntity);
                _optionChoiceEntities.Add(optCEntity);
            }
            
            _title = $"Réservation de {_clientEntity.FirstName} {_clientEntity.LastName} du {booking.Dates.Start:dd/MM/yyyy}";

            _tva = double.Parse(ConfigurationManager.AppSettings["Tva"], CultureInfo.CreateSpecificCulture("en-US"));

            foreach (Room room in booking.Rooms)
            {
                BookedRoomEntity bookedRoomEntity = new BookedRoomEntity(booking, room);
                foreach(AppliedPackEntity appliedPackEntity in bookedRoomEntity.AppliedPackEntities)
                {
                    _appliedPackEntities.Add(appliedPackEntity);
                }
            }

            _editBookingCommand = new DelegateCommandAsync<SumUpViewModel>(_editBooking);
            _validateBookingCommand = new DelegateCommandAsync<SumUpViewModel>(_validateBooking);
            _printBookingCommand = new DelegateCommand<XpsDocument>(_printBooking);

            if (prevNode != null)
            {
                _navigation.AddAfter(prevNode, this);
            }else
            {
                _navigation.AddLast(this);
            }
        }

        private async Task _editBooking(SumUpViewModel sumUpVM)
        {
            LinkedListNode<INavigableViewModel> prevNode = _navigation.Find(sumUpVM);
            BookingViewModel bookingVM = await BookingViewModel.LoadAsync(_navigation, _booking, prevNode);
            NextCalled?.Invoke(this, sumUpVM);
        }

        private async Task _validateBooking(SumUpViewModel sumUpVM)
        {
            _booking.State = BookingState.Validated;
            await BookingRepository.Save(sumUpVM._booking);
            PromptViewModel successPromptVM = new PromptViewModel("Succés", "La commande a réussi", false);
            ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(successPromptVM);
            _pcs.NotifyChange(nameof(NeedsValidation));
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
                    object placeHolder = new object();
                    PromptViewModel successPromptVM = new PromptViewModel("Succés", "L'impression a été effectuée.", false);
                    ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(successPromptVM);
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
    }
}
