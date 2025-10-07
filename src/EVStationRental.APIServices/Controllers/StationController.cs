using EVStationRental.Common.DTOs.StationDTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IStationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
    }
}
