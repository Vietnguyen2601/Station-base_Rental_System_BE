using EVStationRental.Services.InternalServices.IServices.IReportServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVStationRental.APIServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// L?y t?t c? báo cáo
        /// </summary>
        /// <returns>Danh sách báo cáo</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var result = await _reportService.GetAllReportsAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y báo cáo theo ID
        /// </summary>
        /// <param name="id">ID c?a báo cáo</param>
        /// <returns>Thông tin báo cáo</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReportById(Guid id)
        {
            var result = await _reportService.GetReportByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// L?y t?t c? báo cáo c?a m?t tài kho?n
        /// </summary>
        /// <param name="accountId">ID c?a tài kho?n</param>
        /// <returns>Danh sách báo cáo c?a tài kho?n</returns>
        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetReportsByAccountId(Guid accountId)
        {
            var result = await _reportService.GetReportsByAccountIdAsync(accountId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
