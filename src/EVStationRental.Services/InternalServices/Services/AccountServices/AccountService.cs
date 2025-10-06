using EVStationRental.Common.DTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Mapper;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IAccountServices;

namespace EVStationRental.Services.InternalServices.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> GetAccountByIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_VALIDATION_CODE,
                        Message = "ID tài khoản không hợp lệ"
                    };
                }

                var account = await unitOfWork.AccountRepository.GetAccountByIdAsync(id);
                if (account == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Không tìm thấy tài khoản"
                    };
                }

                var accountDto = account.ToViewAccountDTO();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = accountDto
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy thông tin tài khoản: {ex.Message} {innerMessage}"
                };
            }
        }

        public async Task<IServiceResult> GetAllAccountsAsync()
        {
            try
            {
                var accounts = await unitOfWork.AccountRepository.GetAllActiveAccountsAsync();
                if (accounts == null || accounts.Count == 0)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }

                var accountDtos = new List<ViewAccountDTO>();
                accountDtos.AddRange(accounts.Select(account => account.ToViewAccountDTO()));

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = accountDtos
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy danh sách tài khoản: {ex.Message} {innerMessage}"
                };
            }
        }

        public async Task<IServiceResult> GetAllCustomerAccountsAsync()
        {
            try
            {
                var accounts = await unitOfWork.AccountRepository.GetAllCustomersByRole();
                if (accounts == null || accounts.Count == 0)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }
                var accountDtos = new List<ViewAccountDTO>();
                accountDtos.AddRange(accounts.Select(account => account.ToViewAccountDTO()));
                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = accountDtos
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy danh sách tài khoản khách hàng: {ex.Message} {innerMessage}"
                };
            }
        }

        public async Task<IServiceResult> GetAllStaffAccountsAsync()
        {
            try
            {
                var accounts = await unitOfWork.AccountRepository.GetAllStaffByRole();
                if (accounts == null || accounts.Count == 0)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = Const.WARNING_NO_DATA_MSG
                    };
                }
                var accountDtos = new List<ViewAccountDTO>();
                accountDtos.AddRange(accounts.Select(account => account.ToViewAccountDTO()));
                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = Const.SUCCESS_READ_MSG,
                    Data = accountDtos
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy danh sách tài khoản nhân viên: {ex.Message} {innerMessage}"
                };
            }
        }
    }
}
