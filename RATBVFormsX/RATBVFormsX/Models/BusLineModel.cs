﻿using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RATBVFormsX.Models
{
    public class BusLineModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public string LinkNormalWay { get; set; }
        public string LinkReverseWay { get; set; }

        [OneToMany]
        public List<BusStationModel> BusStations { get; set; }
    }
}
