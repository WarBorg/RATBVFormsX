using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RATBVFormsX.Models
{
    public class BusStationModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [ForeignKey(typeof(BusLineModel))]
        public int BusLineId { get; set; }
        public string Name { get; set; }
        public string Direction { get; set; }
        public string SchedualLink { get; set; }
        public string LastUpdateDate { get; set; }
        
        [OneToMany]
        public List<BusTimeTableModel> BusTimeTables { get; set; }
    }
}
