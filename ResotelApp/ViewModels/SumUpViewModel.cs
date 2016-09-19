using ResotelApp.DAL;
using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using ResotelApp.ViewModels.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private PaymentEntity _paymentEntity;
        private double _optionsTotal;
        private double _roomsTotal;
        private bool _hasPayment;

        private ObservableCollection<string> _paymentModesCollection;
        private ICollectionViewSource _paymentModesCollectionSource;
        private ICollectionView _paymentModesCollectionView;
        private Dictionary<string, PaymentMode> _paymentModes;

        private DelegateCommandAsync<SumUpViewModel> _editBookingCommand;
        private DelegateCommandAsync<SumUpViewModel> _saveBookingCommand;
        private DelegateCommand<XpsDocument> _printBookingCommand;
        private Booking _booking;
        private bool _hadPayment;
        private bool _wasInTempState;
        private static string _legalNotice;

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

        public PaymentEntity PaymentEntity
        {
            get { return _paymentEntity; }
        }


        public double OptionsTotal
        {
            get { return _booking.OptionsTotal; }
        }

        public double RoomsTotal
        {

            get { return _booking.RoomsTotal; }
        }

        public double TotalHT
        {
            get { return _booking.TotalHT; }
        }

        public double Total
        {
            get { return _booking.Total; }
        }

        public double TvaVal
        {
            get { return Tva.Value; }
        }

        public string Siret
        {
            get { return "774 082 010 00034"; }
        }

        public bool HasPayment
        {
            get { return _hasPayment; }
            set
            {
                bool hadPaymentJustBefore = _hasPayment;
                _hasPayment = value;
                if(!_hasPayment && hadPaymentJustBefore && _wasInTempState)
                {
                    _paymentEntity.Ammount = 0d;
                    _paymentEntity.Date = null;
                    _paymentEntity.Mode = Models.PaymentMode.CreditCard;
                }
                _unlockSaveIfNeeded();
                _pcs.NotifyChange();
            }
        }

        public bool WasInTempState
        {
            get { return _wasInTempState; }
        }

        public string LegalNotice
        {
            get
            {
                return SumUpViewModel._legalNotice;
            }
        }

        public string PaymentMode
        {
            get
            {
                string paymentMode = "";
                if (_paymentModesCollectionView.CurrentPosition != -1)
                {

                    paymentMode = _paymentModesCollectionView.CurrentItem.ToString();
                }
                return paymentMode;
            }
        }

        public ICollectionView PaymentModesCollectionView
        {
            get { return _paymentModesCollectionView; }
        }

        public ICommand PrintBookingCommand
        {
            get { return _printBookingCommand; }
        }

        public ICommand EditBookingCommand
        {
            get { return _editBookingCommand; }
        }

        public ICommand SaveBookingCommand
        {
            get { return _saveBookingCommand; }
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

        static SumUpViewModel()
        {
            _legalNotice = ConfigurationManager.AppSettings["LegalNotice"];
        }

        public SumUpViewModel(LinkedList<INavigableViewModel> navigation, Booking booking, LinkedListNode<INavigableViewModel> prevNode = null)
        {
            _pcs = new PropertyChangeSupport(this);
            _navigation = navigation;
            _booking = booking;
            _dates = booking.Dates;
            _clientEntity = new ClientEntity(booking.Client);
            _paymentEntity = new PaymentEntity(booking);
            _paymentEntity.PropertyChanged += _payment_changed;
            _hasPayment = _booking.Payment != null && _booking.Payment.Ammount > 0d;
            _hadPayment = _hasPayment;
            _wasInTempState = _booking.State == BookingState.Validated;

            _appliedPackEntities = new List<AppliedPackEntity>();
            _optionChoiceEntities = new List<OptionChoiceEntity>(booking.OptionChoices.Count);

            foreach(OptionChoice optChoiceEntity in booking.OptionChoices)
            {
                OptionChoiceEntity optCEntity = new OptionChoiceEntity(booking, optChoiceEntity);
                _optionChoiceEntities.Add(optCEntity);
            }
            
            _title = $"Réservation de {_clientEntity.FirstName} {_clientEntity.LastName} du {booking.Dates.Start:dd/MM/yyyy}";

            foreach (Room room in booking.Rooms)
            {
                BookedRoomEntity bookedRoomEntity = new BookedRoomEntity(booking, room);
                foreach(AppliedPackEntity appliedPackEntity in bookedRoomEntity.AppliedPackEntities)
                {
                    _appliedPackEntities.Add(appliedPackEntity);
                }
            }

            bool canSave = _booking.State == BookingState.Validated;

            _editBookingCommand = new DelegateCommandAsync<SumUpViewModel>(_editBooking);
            _saveBookingCommand = new DelegateCommandAsync<SumUpViewModel>(_saveBooking, canSave);
            _printBookingCommand = new DelegateCommand<XpsDocument>(_printBooking);

            Dictionary<PaymentMode, string>  paymentModes = new Dictionary<PaymentMode, string>
            {
                { Models.PaymentMode.CreditCard, "Carte de crédit" },
                { Models.PaymentMode.Cheque, "Chèque" },
                { Models.PaymentMode.Cash, "Espèces" }
            };

            _paymentModes = new Dictionary<string, PaymentMode>
            {
                { "Carte de crédit", Models.PaymentMode.CreditCard },
                { "Chèque", Models.PaymentMode.Cheque },
                { "Espèces", Models.PaymentMode.Cash }
            };

            _paymentModesCollection = new ObservableCollection<string>(paymentModes.Values);
            _paymentModesCollectionSource = CollectionViewProvider.Provider(_paymentModesCollection);
            _paymentModesCollectionView = _paymentModesCollectionSource.View;

            _paymentModesCollectionView.CurrentChanged += _paymentModesCollectionView_CurrentChanged;

            _paymentModesCollectionView.MoveCurrentTo(paymentModes[_paymentEntity.Mode]);

            _unlockSaveIfNeeded();
            _unlockEditIfNeeded();




            if((_booking.State != BookingState.Validated && canSave) ||
                (_booking.State == BookingState.Validated && !canSave)
            )
            {
                _saveBookingCommand.ChangeCanExecute();
            }


            if (prevNode != null)
            {
                _navigation.AddAfter(prevNode, this);
            }else
            {
                _navigation.AddLast(this);
            }
        }

        ~SumUpViewModel()
        {
            _paymentModesCollectionView.CurrentChanged -= _paymentModesCollectionView_CurrentChanged;
        }

        private void _paymentModesCollectionView_CurrentChanged(object sender, EventArgs e)
        {
            
            if(_paymentModesCollectionView.CurrentPosition != -1)
            {
                string mode = _paymentModesCollectionView.CurrentItem.ToString();
                PaymentMode paymentMode = _paymentModes[mode];
                _paymentEntity.Mode = paymentMode;
            }
        }

        private void _unlockEditIfNeeded()
        {
            bool canEdit = _editBookingCommand.CanExecute(null);
            bool isFinalState = _booking.State != BookingState.Validated;

            if((canEdit && isFinalState) ||
                (!canEdit && !isFinalState)
            )
            {
                _editBookingCommand.ChangeCanExecute();
            }
        }

        private void _unlockSaveIfNeeded()
        {
            bool canSave = _saveBookingCommand.CanExecute(null);
            bool paymentIsValid = ((IDataErrorInfo)_paymentEntity).Error == null;
            bool isFinalState = _booking.State != BookingState.Validated;

            if ( (canSave && !paymentIsValid && _hasPayment) ||
                (!canSave && paymentIsValid && _wasInTempState) ||
                (!canSave && !_hasPayment && _wasInTempState)
            )
            {
                _saveBookingCommand.ChangeCanExecute();
            }
        }

        private void _payment_changed(object sender, PropertyChangedEventArgs pcea)
        {
            if(pcea.PropertyName == nameof(_paymentEntity.Ammount))
            {
                _unlockSaveIfNeeded();
            }
            _pcs.NotifyChange(nameof(PaymentMode));
        }

        private async Task _editBooking(SumUpViewModel sumUpVM)
        {
            LinkedListNode<INavigableViewModel> prevNode = _navigation.Find(sumUpVM);
            BookingViewModel bookingVM = await BookingViewModel.LoadAsync(_navigation, _booking, prevNode);
            NextCalled?.Invoke(this, sumUpVM);
        }

        private async Task _saveBooking(SumUpViewModel sumUpVM)
        {
            bool paymentSet = false;
            if(_hasPayment && !_hadPayment)
            {
                sumUpVM._booking.Payment.Date = DateTime.Now;
                paymentSet = true;
            }
            await BookingRepository.Save(sumUpVM._booking);

            _unlockEditIfNeeded();

            if(paymentSet)
            {
                _saveBookingCommand.ChangeCanExecute();
            }

            PromptViewModel successPromptVM = new PromptViewModel("Succés", "La commande a réussi", false);
            ViewDriverProvider.ViewDriver.ShowView<PromptViewModel>(successPromptVM);
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
