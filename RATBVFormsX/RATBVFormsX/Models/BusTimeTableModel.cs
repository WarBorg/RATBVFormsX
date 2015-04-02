using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RATBVFormsX.Models
{
    public class BusTimeTableModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(BusStationModel))]
        public int BusStationId { get; set; }
        public string Hour { get; set; }
        public string Minutes { get; set; }
        public string TimeOfWeek { get; set; }
        public string LastUpdateDate { get; set; }
    }
}
