using EVStationRental.Common.DTOs.ReportDTOs;
using EVStationRental.Services.Base;

namespace EVStationRental.Services.InternalServices.IServices.IReportServices
{
    public interface IReportService
    {
        Task<IServiceResult> GetAllReportsAsync();
        Task<IServiceResult> GetReportByIdAsync(Guid reportId);
        Task<IServiceResult> GetReportsByAccountIdAsync(Guid accountId);
    }
}
