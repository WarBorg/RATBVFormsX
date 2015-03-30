using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using RATBVFormsX.Models;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RATBVFormsX.Services
{
    public class BusDataService : IBusDataService
    {
        #region Memebers

        private readonly ISQLiteConnection _connection;

        private bool _isBusStationsDirty;

        #endregion Memebers

        #region Constructor

        public BusDataService(ISQLiteConnectionFactory factory)
        {
            _connection = factory.Create("ratbv.sql");


            CreateAllTables();
        }

        #endregion Constructor

        #region Methods

        #region Universal

        public void CreateAllTables()
        {
            _connection.CreateTable<BusLineModel>();
            _connection.CreateTable<BusStationModel>();
            _connection.CreateTable<BusTimeTableModel>();
        }

        public void DropAllTables()
        {
            _connection.DropTable<BusLineModel>();
            _connection.DropTable<BusStationModel>();
            _connection.DropTable<BusTimeTableModel>();
        }

        #endregion Universal

        #region Bus Lines

        public int CountBusLines
        {
            get 
            {
                return _connection.Table<BusLineModel>().Count();
            }
        }

        public List<BusLineModel> GetBusLinesByName(string nameFilter = null)
        {
            if (nameFilter == null)
                nameFilter = String.Empty;

            return (from busLineTable in _connection.Table<BusLineModel>()
                    where busLineTable.Name.Contains(nameFilter)
                    orderby busLineTable.Id
                    select busLineTable).ToList();
        }

        public BusLineModel GetBusLineById(int Id)
        {
            return _connection.Get<BusLineModel>(Id);
        }

        public void InsertBusLine(BusLineModel busLine)
        {
            _connection.Insert(busLine);
        }

        public void UpdateBusLine(BusLineModel busLine)
        {
            _connection.Update(busLine);
        }

        public void DeleteBusLine(BusLineModel busLine)
        {
            _connection.Delete(busLine);
        }

        #endregion Bus Lines

        #region Bus Stations

        public int CountBusStations(BusLineModel busLine, string direction)
        {
            // Possibility to use the List stored in the BusLineModel full of Bus Stations
            if (_isBusStationsDirty || busLine.BusStations == null)
            {
                _isBusStationsDirty = false;

                _connection.GetChildren<BusLineModel>(busLine);
            }

            return (from busStationTable in busLine.BusStations
                    where busStationTable.Direction == direction
                    select busStationTable).Count();
        }

        public List<BusStationModel> GetBusStationsByName(BusLineModel busLine, string direction, string nameFilter = null)
        {
            if (nameFilter == null)
                nameFilter = String.Empty;

            // Possibility to use the List stored in the BusLineModel full of Bus Stations
            if (_isBusStationsDirty || busLine.BusStations == null)
            {
                _isBusStationsDirty = false;

                _connection.GetChildren<BusLineModel>(busLine);
            }

            return (from busStationTable in busLine.BusStations
                    where busStationTable.Direction == direction
                    && busStationTable.Name.Contains(nameFilter)
                    orderby busStationTable.Id
                    select busStationTable).ToList();

            //return (from busStationTable in _connection.Table<BusStationModel>()
            //        where busStationTable.BusLineId == busId
            //        && busStationTable.Direction == direction
            //        && busStationTable.Name.Contains(nameFilter)
            //        orderby busStationTable.Id
            //        select busStationTable).ToList();
        }

        public BusStationModel GetBusStationById(int Id)
        {
            return _connection.Get<BusStationModel>(Id);
        }

        public void InsertBusStation(BusStationModel busStation)
        {
            _isBusStationsDirty = true;

            _connection.Insert(busStation);
        }

        public void UpdateBusStation(BusStationModel busStation)
        {
            _isBusStationsDirty = true;

            _connection.Update(busStation);
        }

        public void DeleteBusStation(BusStationModel busStation)
        {
            _isBusStationsDirty = true;

            _connection.Delete(busStation);
        }

        public void DeleteBusStationsByBusLineAndDirection(BusLineModel busLine, string direction)
        {
            _isBusStationsDirty = true;

            _connection.GetChildren<BusLineModel>(busLine);

            foreach (var busStation in busLine.BusStations.Where(bs => bs.Direction == direction))
                DeleteBusStation(busStation);
        }

        #endregion Bus Stations

        #region Bus Time Table

        public int CountBusTimeTable(BusStationModel busStation)
        {
            // Possibility to use the List stored in the BusLineModel full of Bus Stations
            if (busStation.BusTimeTables == null)
                _connection.GetChildren<BusStationModel>(busStation);

            return (from busStationTable in busStation.BusTimeTables
                    select busStationTable).Count();
        }

        public List<BusTimeTableModel> GetBusTimeTableByBusStation(BusStationModel busStation)
        {
            // Possibility to use the List stored in the BusStationModel full of Bus Time Tables
            if (busStation.BusTimeTables == null)
                _connection.GetChildren<BusStationModel>(busStation);

            return (from busTimeTable in busStation.BusTimeTables
                    orderby busTimeTable.Id
                    select busTimeTable).ToList();
        }

        public BusTimeTableModel GetBusTimeTableById(int Id)
        {
            return _connection.Get<BusTimeTableModel>(Id);
        }

        public void InsertBusTimeTable(BusTimeTableModel busTimeTable)
        {
            _connection.Insert(busTimeTable);
        }

        public void UpdateBusTimeTable(BusTimeTableModel busTimeTable)
        {
            _connection.Update(busTimeTable);
        }

        public void DeleteBusTimeTable(BusTimeTableModel busTimeTable)
        {
            _connection.Delete(busTimeTable);
        }

        #endregion Bus Time Table

        #endregion Methods
    }
}
