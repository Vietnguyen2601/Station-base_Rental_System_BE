using EVStationRental.Common.DTOs.StationDTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IStationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace EVStationRental.APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Staff,Admin")]
    public class StationController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationController(IStationService stationService)
        {
            _stationService = stationService ?? throw new ArgumentNullException(nameof(stationService));
        }

        [HttpPost]
        public async Task<ActionResult<IServiceResult>> CreateStationAsync([FromBody] CreateStationRequestDTO dto)
        {
            var result = await _stationService.CreateStationAsync(dto);
            return StatusCode((int)result.StatusCode, new { Message = result.Message, Data = result.Data });
        }

        [HttpGet]
        public async Task<ActionResult<IServiceResult>> GetAllStationsAsync()
        {
            var result = await _stationService.GetAllStationsAsync();
            if (result.Data == null)
            {
                return NotFound(new { Message = Const.WARNING_NO_DATA_MSG });
            }
            return Ok(new { Message = Const.SUCCESS_READ_MSG, Data = result.Data });
        }

        [HttpGet("{stationId}/vehicles")]
        public async Task<ActionResult<IServiceResult>> GetVehiclesByStationIdAsync(Guid stationId)
        {
            var result = await _stationService.GetVehiclesByStationIdAsync(stationId);
            if (result.Data == null)
                return NotFound(new { Message = result.Message });
            return Ok(new { Message = result.Message, Data = result.Data });
        }

        [HttpPost("{stationId}/vehicles")]
        public async Task<ActionResult<IServiceResult>> AddVehiclesToStationAsync(Guid stationId, [FromBody] List<Guid> vehicleIds)
        {
            var dto = new AddVehiclesToStationRequestDTO { StationId = stationId, VehicleIds = vehicleIds };
            var result = await _stationService.AddVehiclesToStationAsync(dto);
            return StatusCode(result.StatusCode, new { Message = result.Message });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IServiceResult>> UpdateStationAsync(Guid id, [FromBody] UpdateStationRequestDTO dto)
        {
            var result = await _stationService.UpdateStationAsync(id, dto);
            return StatusCode((int)result.StatusCode, new { Message = result.Message, Data = result.Data });
        }
    }
}
