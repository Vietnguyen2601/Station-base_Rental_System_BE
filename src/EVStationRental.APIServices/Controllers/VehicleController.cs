using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IVehicleServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EVStationRental.Common.DTOs.VehicleDTOs;
using System;

namespace EVStationRental.APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Staff,Admin")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
        }

        /// <summary>
        /// Lấy danh sách tất cả xe
        /// </summary>
        /// <returns>Danh sách xe trong hệ thống</returns>
        /// <response code="200">Trả về danh sách xe thành công</response>
        /// <response code="404">Không tìm thấy dữ liệu</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResult>> GetAllVehiclesAsync()
        {
            var result = await _vehicleService.GetAllVehiclesAsync();
            if (result.Data == null)
            {
                return NotFound(new
                {
                    Message = Const.WARNING_NO_DATA_MSG
                });
            }
            return Ok(new
            {
                Message = Const.SUCCESS_READ_MSG,
                Data = result.Data
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IServiceResult>> GetVehicleByIdAsync(Guid id)
        {
            var result = await _vehicleService.GetVehicleByIdAsync(id);
            if (result.Data == null)
                return NotFound(new { Message = result.Message });
            return Ok(new { Message = result.Message, Data = result.Data });
        }

        [HttpPost]
        public async Task<ActionResult<IServiceResult>> CreateVehicleAsync([FromBody] CreateVehicleRequestDTO dto)
        {
            var result = await _vehicleService.CreateVehicleAsync(dto);
            return StatusCode((int)result.StatusCode, new { Message = result.Message, Data = result.Data });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IServiceResult>> UpdateVehicleAsync(Guid id, [FromBody] UpdateVehicleRequestDTO dto)
        {
            var result = await _vehicleService.UpdateVehicleAsync(id, dto);
            return StatusCode((int)result.StatusCode, new { Message = result.Message, Data = result.Data });
        }
    }
}
