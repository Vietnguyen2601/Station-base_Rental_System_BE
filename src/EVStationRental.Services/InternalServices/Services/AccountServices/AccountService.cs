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

        public async Task<IServiceResult> SetAccountRolesAsync(Guid accountId, List<Guid> roleIds)
        {
            try
            {
                // Validate account ID
                if (accountId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_VALIDATION_CODE,
                        Message = "ID tài khoản không hợp lệ"
                    };
                }

                // Validate role IDs
                if (roleIds == null || roleIds.Count == 0)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_VALIDATION_CODE,
                        Message = "Danh sách vai trò không được để trống"
                    };
                }

                // Check if any role ID is empty
                if (roleIds.Any(r => r == Guid.Empty))
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_VALIDATION_CODE,
                        Message = "ID vai trò không hợp lệ"
                    };
                }

                // Check if account exists
                var account = await unitOfWork.AccountRepository.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Không tìm thấy tài khoản"
                    };
                }

                // Set account roles
                var result = await unitOfWork.AccountRepository.SetAccountRolesAsync(accountId, roleIds);
                if (!result)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_EXCEPTION,
                        Message = "Lỗi khi cập nhật vai trò cho tài khoản"
                    };
                }

                // Get updated account with roles
                var updatedAccount = await unitOfWork.AccountRepository.GetAccountWithRolesAsync(accountId);
                var accountDto = updatedAccount?.ToViewAccountDTO();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_UPDATE_CODE,
                    Message = "Cập nhật vai trò cho tài khoản thành công",
                    Data = accountDto
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi cập nhật vai trò: {ex.Message} {innerMessage}"
                };
            }
        }

        public async Task<IServiceResult> SetAdminRoleAsync(Guid accountId)
        {
            try
            {
                // Validate account ID
                if (accountId == Guid.Empty)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.ERROR_VALIDATION_CODE,
                        Message = "ID tài khoản không hợp lệ"
                    };
                }

                // Check if account exists
                var account = await unitOfWork.AccountRepository.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Không tìm thấy tài khoản"
                    };
                }

                // Get Admin role
                var adminRole = await unitOfWork.RoleRepository.GetRoleByNameAsync("Admin");
                if (adminRole == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Không tìm thấy vai trò Admin trong hệ thống"
                    };
                }

                // Add admin role to account
                await unitOfWork.AccountRepository.AddAccountRoleAsync(accountId, adminRole.RoleId);

                // Get updated account with roles
                var updatedAccount = await unitOfWork.AccountRepository.GetAccountWithRolesAsync(accountId);
                var accountDto = updatedAccount?.ToViewAccountDTO();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_UPDATE_CODE,
                    Message = "Thêm vai trò Admin cho tài khoản thành công",
                    Data = accountDto
                };
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi thêm vai trò Admin: {ex.Message} {innerMessage}"
                };
            }
        }
    }
}
