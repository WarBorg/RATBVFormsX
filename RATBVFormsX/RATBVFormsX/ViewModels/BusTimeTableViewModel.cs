using RATBVFormsX.Constants;
using RATBVFormsX.Models;
using RATBVFormsX.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        #endregion Properties

        #region Constructors

        public BusTimeTableViewModel(IBusDataService busDataService, IBusWebService busWebService)
        {
            _busDataService = busDataService;
            _busWebService = busWebService;
        }
        
        #endregion Constructors

        #region Methods

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

        private async Task GetBusTimeTableAsync(string schedualLink)
        {
            List<BusTimeTableModel> busTimetable = await _busWebService.GetBusTimeTableAsync(schedualLink);

            SetTimeTable(busTimetable);

            await AddBusStationsToDatabaseAsync(busTimetable);
        }

        private void GetBusTimeTable()
        {
            List<BusTimeTableModel> busTimetable = _busDataService.GetBusTimeTableByBusStation(BusStation);

            SetTimeTable(busTimetable);
        }

        private void SetTimeTable(List<BusTimeTableModel> butTimetable)
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

                    _busDataService.InsertBusTimeTable(busTimetableHour);
                }
            });
        }

        #endregion Methods
    }
}
