using Acr.MvvmCross.Plugins.Network;
using Acr.MvvmCross.Plugins.UserDialogs;
using Cirrious.MvvmCross.ViewModels;
using RATBVFormsX.Constants;
using RATBVFormsX.Models;
using RATBVFormsX.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RATBVFormsX.ViewModels
{
    public class BusStationsViewModel
        : BaseViewModel
    {
        #region Members

        private string _direction = String.Empty;
        private string _linkDirection = String.Empty;

        private BusLineModel _busLine;
        private List<BusStationModel> _busStations;

        private MvxCommand _reverseCommand;
        private MvxCommand _refreshCommand;
        private MvxCommand _downloadCommand;

        #endregion Members

        #region Properties

        public string Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                RaisePropertyChanged(() => BusLineName);
            }
        }

        public string BusLineName
        {
            get { return String.Format("{0} - {1}", BusLine.Name, Direction); }
        }

        public BusLineModel BusLine
        {
            get { return _busLine; }
            set
            {
                _busLine = value;
                RaisePropertyChanged(() => BusLine);
            }
        }

        public List<BusStationModel> BusStations
        {
            get { return _busStations; }
            set
            {
                _busStations = value;
                RaisePropertyChanged(() => BusStations);
            }
        }

        #region Commands

        public ICommand ShowSelectedBusTimeTableCommand
        {
            get
            {
                return new MvxCommand<BusStationModel>(busStation => ShowViewModel<BusTimeTableViewModel>(new BusTimeTableViewModel.Navigation() { Id = busStation.Id }));
            }
        }

        public ICommand ReverseCommand
        {
            get
            {
                _reverseCommand = _reverseCommand ?? new MvxCommand(DoReverseCommand);
                return _reverseCommand;
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new MvxCommand(DoRefreshCommand);
                return _refreshCommand;
            }
        }

        public ICommand DownloadCommand
        {
            get
            {
                _downloadCommand = _downloadCommand ?? new MvxCommand(DoDownloadCommand);
                return _downloadCommand;
            }
        }

        #endregion Commands

        #endregion Properties

        #region Constructors

        public BusStationsViewModel(IBusDataService busDataService, IBusWebService busWebService, IUserDialogService dialogService, INetworkService networkService)
        {
            _busDataService = busDataService;
            _busWebService = busWebService;
            _dialogService = dialogService;
            _networkService = networkService;
        }
        
        #endregion Constructors

        #region Methods

        #region Commands

        private async void DoReverseCommand()
        {
            await CheckBusStations();
        }

        private async void DoRefreshCommand()
        {
            //TODO check if internet connection
            await CheckBusStations(true);
        }

        private async void DoDownloadCommand()
        {
            //TODO check if internet connection
            await DownloadAllStationsSchedualsAsync();

            _dialogService.Toast("Download complete for all bus stations");
        }
        
        #endregion Commands

        #region Navigation

        public class Navigation
        {
            public int Id { get; set; }
        }

        public async void Init(Navigation navigation)
        {
            BusLine = _busDataService.GetBusLineById(navigation.Id);

            await CheckBusStations();
        }
        
        #endregion Navigation

        private async Task CheckBusStations(bool isRefresh = false)
        {
            if (!isRefresh)
            {
                if (Direction == String.Empty || Direction == RouteDirections.Reverse)
                {
                    Direction = RouteDirections.Normal;

                    _linkDirection = BusLine.LinkNormalWay;
                }
                else // if direction is normal
                {
                    Direction = RouteDirections.Reverse;

                    _linkDirection = BusLine.LinkReverseWay;
                }
            }

            if (isRefresh || (_busDataService.CountBusStations(BusLine, Direction) == 0))
                await GetBusStationsAsync(_linkDirection, Direction);
            else
                GetBusStations();
        }

        private async Task GetBusStationsAsync(string linkDirection, string direction)
        {
            if (!CheckInternetAvailability())
                return;

            BusStations = await _busWebService.GetBusStationsAsync(linkDirection);

            await AddBusStationsToDatabaseAsync(direction);
        }

        private void GetBusStations()
        {
            BusStations = _busDataService.GetBusStationsByName(BusLine, Direction);
        }

        private async Task AddBusStationsToDatabaseAsync(string direction)
        {
            await Task.Factory.StartNew(() =>
            {
                // Delete prior instances of the bus stations for selected bus line and direction
                _busDataService.DeleteBusStationsByBusLineAndDirection(BusLine, direction);

                var test = _busDataService.CountBusStations(BusLine, direction);

                foreach (var busStation in BusStations)
                {
                    // Add foreign key and direction before inserting in database
                    busStation.BusLineId = BusLine.Id;
                    busStation.Direction = direction;

                    _busDataService.InsertBusStation(busStation);
                }
            });
        }

        private async Task DownloadAllStationsSchedualsAsync()
        {
            if (!CheckInternetAvailability())
                return;

            foreach (var busStation in BusStations)
            {
                List<BusTimeTableModel> busTimetable = await _busWebService.GetBusTimeTableAsync(busStation.SchedualLink);

                await Task.Factory.StartNew(() =>
                {
                    foreach (var busTimetableHour in busTimetable)
                    {
                        // Add foreign key before inserting in database
                        busTimetableHour.BusStationId = busStation.Id;

                        _busDataService.InsertBusTimeTable(busTimetableHour);
                    }
                });
            }
        }

        #endregion Methods
    }
}
