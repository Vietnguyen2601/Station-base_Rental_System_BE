using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IAccountServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace EVStationRental.APIServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Staff,Admin")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }

        /// <summary>
        /// Lấy thông tin tài khoản theo ID
        /// </summary>
        /// <param name="id">ID của tài khoản</param>
        /// <returns>Thông tin chi tiết tài khoản</returns>
        /// <response code="200">Trả về thông tin tài khoản thành công</response>
        /// <response code="400">ID không hợp lệ</response>
        /// <response code="404">Không tìm thấy tài khoản</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResult>> GetAccountByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "ID tài khoản không hợp lệ"
                });
            }

            var result = await _accountService.GetAccountByIdAsync(id);
            if (result.StatusCode == Const.WARNING_NO_DATA_CODE)
            {
                return NotFound(new
                {
                    Message = result.Message
                });
            }
            return StatusCode((int)result.StatusCode, new
            {
                Message = result.Message,
                Data = result.Data
            });
        }

        /// <summary>
        /// Lấy danh sách tất cả tài khoản
        /// </summary>
        /// <returns>Danh sách tài khoản trong hệ thống</returns>
        /// <response code="200">Trả về danh sách tài khoản thành công</response>
        /// <response code="404">Không tìm thấy dữ liệu</response>
        /// <response code="500">Lỗi server khi xử lý yêu cầu</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IServiceResult>> GetAllAccountsAsync()
        {
            var result = await _accountService.GetAllAccountsAsync();
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
    }
}
