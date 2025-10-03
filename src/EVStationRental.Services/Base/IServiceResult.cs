using EVStationRental.Common.Enums;
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
        ResultStatus StatusCode { get; set; }
        string? Message { get; set; }
        object? Data { get; set; }
        bool IsSuccess { get; }
        List<ErrorDetail>? Errors { get; set; }
    }
}
