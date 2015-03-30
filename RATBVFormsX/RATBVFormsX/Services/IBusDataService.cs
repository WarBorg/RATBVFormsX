using RATBVFormsX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RATBVFormsX.Services
{
    public interface IBusDataService
    {
        void CreateAllTables();
        void DropAllTables();

        int CountBusLines { get; }
        List<BusLineModel> GetBusLinesByName(string nameFilter = null);
        BusLineModel GetBusLineById(int Id);
        void InsertBusLine(BusLineModel busLine);
        void UpdateBusLine(BusLineModel busLine);
        void DeleteBusLine(BusLineModel busLine);

        int CountBusStations(BusLineModel busLine, string direction);
        List<BusStationModel> GetBusStationsByName(BusLineModel busLine, string direction, string nameFilter = null);
        BusStationModel GetBusStationById(int Id);
        void InsertBusStation(BusStationModel busStation);
        void UpdateBusStation(BusStationModel busStation);
        void DeleteBusStation(BusStationModel busStation);
        void DeleteBusStationsByBusLineAndDirection(BusLineModel busLine, string direction);

        int CountBusTimeTable(BusStationModel busStation);
        List<BusTimeTableModel> GetBusTimeTableByBusStation(BusStationModel busStation);
        BusTimeTableModel GetBusTimeTableById(int Id);
        void InsertBusTimeTable(BusTimeTableModel busTimeTable);
        void UpdateBusTimeTable(BusTimeTableModel busTimeTable);
        void DeleteBusTimeTable(BusTimeTableModel busTimeTable);
    }
}
