using RATBVFormsX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RATBVFormsX.Services
{
    public interface IBusWebService
    {
        Task<List<BusLineModel>> GetBusLinesAsync();
        Task<List<BusStationModel>> GetBusStationsAsync(string lineNumberLink);
        Task<List<BusTimeTableModel>> GetBusTimeTableAsync(string schedualLink);
    }
}
