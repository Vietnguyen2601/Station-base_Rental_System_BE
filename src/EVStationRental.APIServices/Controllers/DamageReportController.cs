using EVStationRental.Common.DTOs.DamageReportDTOs;
using EVStationRental.Common.DTOs.FeedbackDTOs;
using EVStationRental.Common.DTOs.VehicleTypeDTOs;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IDamageReportServices;
using EVStationRental.Services.InternalServices.IServices.IFeedbackServices;
using EVStationRental.Services.InternalServices.Services.FeedbackServices;
using Microsoft.AspNetCore.Mvc;

namespace EVStationRental.APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DamageReportController : ControllerBase
    {
        private readonly IDamageReportService _damageReportService;

        public DamageReportController(IDamageReportService damageReportService)
        {
            _damageReportService = damageReportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDamageReports()
        {
            var result = await _damageReportService.GetAllDamageReports();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{damageReportId}")]
        public async Task<IActionResult> GetDamageReportById(Guid damageReportId)
        {
            var result = await _damageReportService.GetDamageReportByIdAsync(damageReportId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetDamageReportsByVehicleId(Guid vehicleId)
        {
            var result = await _damageReportService.GetDamageReportsByVehicleIdAsync(vehicleId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetDamageReportByOrderId(Guid orderId)
        {
            var result = await _damageReportService.GetDamageReportByOrderIdAsync(orderId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateDamageReport([FromBody] CreateDamageReportRequestDTO dto)
        {
            var result = await _damageReportService.CreateDamageReportAsync(dto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<IServiceResult>> UpdateDamageReport(Guid id, [FromBody] UpdateDamageReportRequestDTO dto)
        {
            var result = await _damageReportService.UpdateDamageReportAsync(id, dto);
            return StatusCode((int)result.StatusCode, new { Message = result.Message, Data = result.Data });
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<IServiceResult>> SoftDeleteDamageReportAsync(Guid id)
        {
            var result = await _damageReportService.SoftDeleteDamageReportAsync(id);
            return StatusCode((int)result.StatusCode, new { Message = result.Message, Data = result.Data });
        }

        [HttpDelete("hardDelete/{id}")]
        public async Task<ActionResult<IServiceResult>> HardDeleteDamageReportAsync(Guid id)
        {
            var result = await _damageReportService.HardDeleteDamageReportAsync(id);
            return StatusCode((int)result.StatusCode, new { Message = result.Message, Data = result.Data });
        }
    }
}
