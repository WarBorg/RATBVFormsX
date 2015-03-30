using Acr.MvvmCross.Plugins.Network;
using Acr.MvvmCross.Plugins.UserDialogs;
using Cirrious.MvvmCross.ViewModels;
using RATBVFormsX.Constants;
using RATBVFormsX.Models;
using RATBVFormsX.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RATBVFormsX.ViewModels
{
    public class BusLinesViewModel
        : BaseViewModel
    {
        #region Memebers

        private List<BusLineModel> _busLines;
        private List<BusLineModel> _midiBusLines;
        private List<BusLineModel> _trolleybusLines;

        private bool isBusy;

        private MvxCommand _refreshCommand;

        #endregion Memebers

        #region Properties

        public List<BusLineModel> BusLines
        {
            get { return _busLines; }
            set 
            { 
                _busLines = value; 
                RaisePropertyChanged(() => BusLines); 
            }
        }

        public List<BusLineModel> MidiBusLines
        {
            get { return _midiBusLines; }
            set
            {
                _midiBusLines = value;
                RaisePropertyChanged(() => MidiBusLines);
            }
        }

        public List<BusLineModel> TrolleybusLines
        {
            get { return _trolleybusLines; }
            set
            {
                _trolleybusLines = value;
                RaisePropertyChanged(() => TrolleybusLines);
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public ICommand ShowSelectedBusLineStationsCommand
        {
            get
            {
                return new MvxCommand<BusLineModel>(busLine =>
                    {
                        if (!CheckInternetAvailability())
                            return;

                        ShowViewModel<BusStationsViewModel>(new BusStationsViewModel.Navigation() { Id = busLine.Id }); 
                    });
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new MvxCommand(DoRefreshCommand, () => { return !IsBusy; });
                return _refreshCommand;
            }
        }

        #endregion Properties

        #region Constructors

        public BusLinesViewModel(IBusDataService busDataService, IBusWebService busWebService, IUserDialogService dialogService, INetworkService networkService)
        {
            _busDataService = busDataService;
            _busWebService = busWebService;

            _dialogService = dialogService;
            _networkService = networkService;
        }
        
        #endregion Constructors

        #region Methods

        #region Commands

        private async void DoRefreshCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            _refreshCommand.RaiseCanExecuteChanged();

            await GetBusLinesAsync();

            IsBusy = false;
            _refreshCommand.RaiseCanExecuteChanged();
        }
        
        #endregion Commands

        public override async void Start()
        {
            base.Start();

            if (_busDataService.CountBusLines == 0)
                await GetBusLinesAsync();
            else
                GetBusLines();
        }

        private async Task GetBusLinesAsync()
        {
            if (!CheckInternetAvailability())
                return;

            List<BusLineModel> busLines = await _busWebService.GetBusLinesAsync();

            SetBusLines(busLines);

            await AddBusLinesToDatabaseAsync(busLines);
        }

        private void GetBusLines()
        {
            List<BusLineModel> busLines = _busDataService.GetBusLinesByName();

            SetBusLines(busLines);
        }

        private void SetBusLines(List<BusLineModel> busLines)
        {
            BusLines = busLines.Where(bl => bl.Type == BusTypes.Bus).ToList();
            MidiBusLines = busLines.Where(bl => bl.Type == BusTypes.Midibus).ToList();
            TrolleybusLines = busLines.Where(bl => bl.Type == BusTypes.Trolleybus).ToList();
        }

        private async Task AddBusLinesToDatabaseAsync(List<BusLineModel> busLines)
        {
            await Task.Factory.StartNew(() =>
                {
                    // Reset the whole database
                    _busDataService.DropAllTables();
                    _busDataService.CreateAllTables();

                    foreach (var busLine in busLines)
                        _busDataService.InsertBusLine(busLine);
                });
        }

        #endregion Methods

    }
}
        
    

