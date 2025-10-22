using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Services.Base
{
    public interface IServiceResult
    {
        int StatusCode { get; set; }
        string? Message { get; set; }
        object? Data { get; set; }
    }
}
