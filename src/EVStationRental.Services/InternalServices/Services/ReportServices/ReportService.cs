using EVStationRental.Common.DTOs.ReportDTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Mapper;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IReportServices;

namespace EVStationRental.Services.InternalServices.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> GetAllReportsAsync()
        {
            try
            {
                var reports = await _unitOfWork.ReportRepository.GetAllAsync();

                if (reports == null || !reports.Any())
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }

                var reportResponses = reports.Select(r => r.ToViewReportResponse()).ToList();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = reportResponses
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y danh sách báo cáo: {ex.Message}"
                };
            }
        }

        public async Task<IServiceResult> GetReportByIdAsync(Guid reportId)
        {
            try
            {
                var report = await _unitOfWork.ReportRepository.GetByIdAsync(reportId);

                if (report == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }

                var reportResponse = report.ToViewReportResponse();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = reportResponse
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y thông tin báo cáo: {ex.Message}"
                };
            }
        }

        public async Task<IServiceResult> GetReportsByAccountIdAsync(Guid accountId)
        {
            try
            {
                // Ki?m tra account có t?n t?i không
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
                if (account == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Tài kho?n không t?n t?i"
                    };
                }

                var reports = await _unitOfWork.ReportRepository.GetByAccountIdAsync(accountId);

                if (reports == null || !reports.Any())
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Không tìm th?y báo cáo nào c?a tài kho?n này"
                    };
                }

                var reportResponses = reports.Select(r => r.ToViewReportResponse()).ToList();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = reportResponses
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"L?i khi l?y danh sách báo cáo theo tài kho?n: {ex.Message}"
                };
            }
        }
    }
}
