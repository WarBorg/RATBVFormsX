using Cirrious.MvvmCross.ViewModels;
using RATBVFormsX.Constants;
using RATBVFormsX.Models;
using RATBVFormsX.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace RATBVFormsX.ViewModels
{
    public class BusTimeTableViewModel
        : BaseViewModel
    {
        #region Members

        private BusStationModel _busStation;
        private List<BusTimeTableModel> _busTimeTableWeekdays;
        private List<BusTimeTableModel> _busTimeTableSaturday;
        private List<BusTimeTableModel> _busTimeTableSunday;
        private List<BusTimeTableModel> _busTimeTableHolidayWeekdays;

        private string _lastUpdated = "never";

        private bool _isBusy;

        private MvxCommand _refreshCommand;

        #endregion Members

        #region Properties

        //TODO add bus line number
        public string BusLineAndStation
        {
            get { return BusStation.Name; }
        }

        public BusStationModel BusStation
        {
            get { return _busStation; }
            set
            {
                _busStation = value;
                RaisePropertyChanged(() => BusStation);
            }
        }

        public List<BusTimeTableModel> BusTimeTableWeekdays
        {
            get { return _busTimeTableWeekdays; }
            set
            {
                _busTimeTableWeekdays = value;
                RaisePropertyChanged(() => BusTimeTableWeekdays);
            }
        }

        public List<BusTimeTableModel> BusTimeTableSaturday
        {
            get { return _busTimeTableSaturday; }
            set
            {
                _busTimeTableSaturday = value;
                RaisePropertyChanged(() => BusTimeTableSaturday);
            }
        }

        public List<BusTimeTableModel> BusTimeTableSunday
        {
            get { return _busTimeTableSunday; }
            set
            {
                _busTimeTableSunday = value;
                RaisePropertyChanged(() => BusTimeTableSunday);
            }
        }

        public List<BusTimeTableModel> BusTimeTableHolidayWeekdays
        {
            get { return _busTimeTableHolidayWeekdays; }
            set
            {
                _busTimeTableHolidayWeekdays = value;
                RaisePropertyChanged(() => BusTimeTableHolidayWeekdays);
            }
        }

        public string Title
        {
            get
            {
                if (Device.OS == TargetPlatform.WinPhone)
                    return String.Format("{0} - Updated on {1}", BusLineAndStation, LastUpdated);

                return BusLineAndStation;
            }
        }

        public string LastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                _lastUpdated = value;
                RaisePropertyChanged(() => LastUpdated);
                RaisePropertyChanged(() => Title);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy == value)
                    return;

                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        #region Commands

        public ICommand RefreshCommand
        {
            get
            {
                _refreshCommand = _refreshCommand ?? new MvxCommand(DoRefreshCommand, () => { return !IsBusy; });
                return _refreshCommand;
            }
        }

        #endregion Commands

        #endregion Properties

        #region Constructors

        public BusTimeTableViewModel(IBusDataService busDataService, IBusWebService busWebService)
        {
            _busDataService = busDataService;
            _busWebService = busWebService;
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

            await GetBusTimeTableAsync(BusStation.SchedualLink);

            IsBusy = false;
            _refreshCommand.RaiseCanExecuteChanged();
        }

        #endregion Commands

        #region Navigation

        public class Navigation
        {
            public int Id { get; set; }
        }

        public async void Init(Navigation navigation)
        {
            BusStation = _busDataService.GetBusStationById(navigation.Id);

            if (_busDataService.CountBusTimeTable(BusStation) == 0)
                await GetBusTimeTableAsync(BusStation.SchedualLink);
            else
                GetBusTimeTable();
        }

        #endregion Navigation

        private async Task GetBusTimeTableAsync(string schedualLink)
        {
            List<BusTimeTableModel> busTimetable = await _busWebService.GetBusTimeTableAsync(schedualLink);

            GetTimeTableByTimeOfWeek(busTimetable);

            LastUpdated = String.Format("{0:d} {1:HH:mm}", DateTime.Now.Date, DateTime.Now);

            await AddBusStationsToDatabaseAsync(busTimetable);
        }

        private void GetBusTimeTable()
        {
            List<BusTimeTableModel> busTimetable = _busDataService.GetBusTimeTableByBusStation(BusStation);

            LastUpdated = busTimetable.FirstOrDefault().LastUpdateDate;

            GetTimeTableByTimeOfWeek(busTimetable);
        }

        private void GetTimeTableByTimeOfWeek(List<BusTimeTableModel> butTimetable)
        {
            BusTimeTableWeekdays = butTimetable.Where(btt => btt.TimeOfWeek == TimeOfTheWeek.WeekDays).ToList();
            BusTimeTableSaturday = butTimetable.Where(btt => btt.TimeOfWeek == TimeOfTheWeek.Saturday).ToList();
            BusTimeTableSunday = butTimetable.Where(btt => btt.TimeOfWeek == TimeOfTheWeek.Sunday).ToList();
        }

        private async Task AddBusStationsToDatabaseAsync(List<BusTimeTableModel> busTimetable)
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (var busTimetableHour in busTimetable)
                {
                    // Add foreign key before inserting in database
                    busTimetableHour.BusStationId = BusStation.Id;
                    busTimetableHour.LastUpdateDate = LastUpdated;

                    _busDataService.InsertBusTimeTable(busTimetableHour);
                }
            });
        }

        #endregion Methods
    }
}
