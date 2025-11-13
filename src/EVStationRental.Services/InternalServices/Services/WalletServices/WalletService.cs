using System;
using System.Linq;
using System.Threading.Tasks;
using EVStationRental.Common.DTOs.PaymentDTOs;
using EVStationRental.Common.DTOs.WalletDTOs;
using EVStationRental.Common.Enums.EnumModel;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Models;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.ExternalService.IServices;
using EVStationRental.Services.InternalServices.IServices.IWalletServices;
using Microsoft.Extensions.Logging;

namespace EVStationRental.Services.InternalServices.Services.WalletServices
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WalletService> _logger;
        private readonly IVNPayService _vnpayService;

        public WalletService(IUnitOfWork unitOfWork, ILogger<WalletService> logger, IVNPayService vnpayService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _vnpayService = vnpayService;
        }

        /// <summary>
        /// Get wallet balance for an account
        /// </summary>
        public async Task<IServiceResult> GetWalletBalanceAsync(Guid accountId)
        {
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetByAccountIdAsync(accountId);
                
                if (wallet == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Ví chưa được tạo cho tài khoản này"
                    };
                }

                var response = new WalletBalanceDTO
                {
                    WalletId = wallet.WalletId,
                    AccountId = wallet.AccountId,
                    Balance = wallet.Balance,
                    CreatedAt = wallet.CreatedAt,
                    UpdatedAt = wallet.UpdatedAt
                };

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = "Lấy thông tin ví thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting wallet balance for account {AccountId}", accountId);
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy thông tin ví: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Top up wallet with transaction tracking
        /// </summary>
        public async Task<IServiceResult> TopUpWalletAsync(Guid accountId, TopUpWalletDTO request)
        {
            try
            {
                // Get or create wallet
                var wallet = await _unitOfWork.WalletRepository.GetByAccountIdAsync(accountId);
                
                if (wallet == null)
                {
                    // Auto-create wallet if not exists
                    var createResult = await CreateWalletForAccountAsync(accountId);
                    if (createResult.StatusCode != Const.SUCCESS_CREATE_CODE)
                    {
                        return createResult;
                    }
                    wallet = await _unitOfWork.WalletRepository.GetByAccountIdAsync(accountId);
                }

                // For simplicity, we'll process CASH/BANK_TRANSFER as immediate success
                // For VNPAY, you would generate a payment URL (similar to order payment)
                
                // Create transaction record first (PENDING for VNPay, COMPLETED for direct)
                var transaction = new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    OrderId = null, // Top-up is not related to any order
                    Amount = request.Amount,
                    TransactionType = TransactionType.DEPOSIT,
                    Description = request.Description ?? $"Nạp tiền vào ví qua {request.PaymentMethod}",
                    CreatedAt = DateTime.Now,
                    Isactive = true
                };

                await _unitOfWork.WalletRepository.CreateTransactionAsync(transaction);

                // Update wallet balance (for non-gateway methods)
                if (request.PaymentMethod.ToUpper() != "VNPAY")
                {
                    wallet.Balance += request.Amount;
                    wallet.UpdatedAt = DateTime.Now;
                    await _unitOfWork.WalletRepository.UpdateWalletAsync(wallet);

                    var response = new TopUpResponseDTO
                    {
                        TransactionId = transaction.TransactionId,
                        Amount = request.Amount,
                        PaymentMethod = request.PaymentMethod,
                        Status = "COMPLETED",
                        PaymentUrl = null,
                        Message = $"Nạp tiền thành công. Số dư hiện tại: {wallet.Balance:N0} VNĐ"
                    };

                    return new ServiceResult
                    {
                        StatusCode = Const.SUCCESS_CREATE_CODE,
                        Message = "Nạp tiền vào ví thành công",
                        Data = response
                    };
                }
                else
                {
                    // For VNPay: would need to integrate with VNPayService to generate payment URL
                    // For now, return a placeholder
                    var response = new TopUpResponseDTO
                    {
                        TransactionId = transaction.TransactionId,
                        Amount = request.Amount,
                        PaymentMethod = request.PaymentMethod,
                        Status = "PENDING",
                        PaymentUrl = "https://sandbox.vnpayment.vn/...", // Would be generated by VNPayService
                        Message = "Vui lòng hoàn tất thanh toán qua VNPay"
                    };

                    return new ServiceResult
                    {
                        StatusCode = Const.SUCCESS_CREATE_CODE,
                        Message = "Yêu cầu nạp tiền đã được tạo. Vui lòng thanh toán qua VNPay.",
                        Data = response
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error topping up wallet for account {AccountId}", accountId);
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi nạp tiền: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get transaction history for wallet
        /// </summary>
        public async Task<IServiceResult> GetTransactionHistoryAsync(Guid accountId, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var wallet = await _unitOfWork.WalletRepository.GetByAccountIdAsync(accountId);
                
                if (wallet == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Ví chưa được tạo cho tài khoản này"
                    };
                }

                var transactions = await _unitOfWork.WalletRepository.GetTransactionHistoryAsync(
                    wallet.WalletId, pageNumber, pageSize);

                var response = transactions.Select(t => new WalletTransactionDTO
                {
                    TransactionId = t.TransactionId,
                    OrderId = t.OrderId,
                    OrderCode = t.Order?.OrderCode,
                    Amount = t.Amount,
                    TransactionType = t.TransactionType.ToString(),
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                }).ToList();

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE,
                    Message = "Lấy lịch sử giao dịch thành công",
                    Data = new
                    {
                        WalletId = wallet.WalletId,
                        CurrentBalance = wallet.Balance,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        Transactions = response
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction history for account {AccountId}", accountId);
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi lấy lịch sử giao dịch: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Create wallet for a new account
        /// </summary>
        public async Task<IServiceResult> CreateWalletForAccountAsync(Guid accountId)
        {
            try
            {
                // Check if wallet already exists
                var existingWallet = await _unitOfWork.WalletRepository.GetByAccountIdAsync(accountId);
                if (existingWallet != null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_DATA_EXISTED_CODE,
                        Message = "Ví đã tồn tại cho tài khoản này"
                    };
                }

                var wallet = new Wallet
                {
                    WalletId = Guid.NewGuid(),
                    AccountId = accountId,
                    Balance = 0,
                    CreatedAt = DateTime.Now,
                    Isactive = true
                };

                await _unitOfWork.WalletRepository.CreateWalletAsync(wallet);

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_CREATE_CODE,
                    Message = "Tạo ví thành công",
                    Data = new WalletBalanceDTO
                    {
                        WalletId = wallet.WalletId,
                        AccountId = wallet.AccountId,
                        Balance = wallet.Balance,
                        CreatedAt = wallet.CreatedAt,
                        UpdatedAt = wallet.UpdatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating wallet for account {AccountId}", accountId);
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi tạo ví: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Create VNPay payment URL by WalletId
        /// Creates a PENDING transaction and returns VNPay payment URL
        /// </summary>
        public async Task<IServiceResult> CreateVNPayUrlByWalletIdAsync(
            Guid walletId, 
            decimal amount, 
            string returnUrl, 
            string cancelUrl, 
            string ipAddress)
        {
            try
            {
                // Validate wallet exists
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);
                if (wallet == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Ví không tồn tại"
                    };
                }

                // Validate amount
                if (amount <= 0)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.FAIL_CREATE_CODE,
                        Message = "Số tiền nạp phải lớn hơn 0"
                    };
                }

                // Create PENDING transaction
                var transaction = new WalletTransaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = walletId,
                    OrderId = null, // Wallet top-up không liên quan đến order
                    Amount = amount,
                    TransactionType = TransactionType.DEPOSIT,
                    Description = $"Nạp tiền vào ví qua VNPay - {amount:N0} VNĐ",
                    CreatedAt = DateTime.Now,
                    Isactive = true
                };

                await _unitOfWork.WalletRepository.CreateTransactionAsync(transaction);

                // Generate VNPay payment URL
                var vnpayRequest = new VNPayRequestDTO
                {
                    OrderId = transaction.TransactionId, // Use TransactionId as OrderId
                    Amount = amount,
                    OrderInfo = $"Nap tien vi {walletId}",
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                };

                var paymentUrl = _vnpayService.CreatePaymentUrl(vnpayRequest, ipAddress);

                var response = new TopUpResponseDTO
                {
                    TransactionId = transaction.TransactionId,
                    Amount = amount,
                    PaymentMethod = "VNPAY",
                    Status = "PENDING",
                    PaymentUrl = paymentUrl,
                    Message = "Vui lòng hoàn tất thanh toán qua VNPay"
                };

                return new ServiceResult
                {
                    StatusCode = Const.SUCCESS_READ_CODE, // Use 200 instead of 201
                    Message = "Tạo URL thanh toán VNPay thành công",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating VNPay URL for wallet {WalletId}", walletId);
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi tạo URL thanh toán: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Handle VNPay return callback for wallet top-up
        /// Validates signature and updates wallet balance if payment successful
        /// </summary>
        public async Task<IServiceResult> HandleVNPayWalletReturnAsync(VNPayReturnDTO callback)
        {
            try
            {
                // Validate VNPay signature
                if (!_vnpayService.ValidateSignature(callback))
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.FAIL_CREATE_CODE,
                        Message = "Chữ ký VNPay không hợp lệ"
                    };
                }

                // Parse transaction ID from vnp_TxnRef
                // Format is: {transactionId}_{timestamp}
                var txnRefParts = callback.vnp_TxnRef?.Split('_');
                if (txnRefParts == null || txnRefParts.Length == 0 || !Guid.TryParse(txnRefParts[0], out var transactionId))
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.FAIL_CREATE_CODE,
                        Message = "Mã giao dịch không hợp lệ"
                    };
                }

                // Get transaction
                var transaction = await _unitOfWork.WalletRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Giao dịch không tồn tại"
                    };
                }

                // Get wallet
                var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(transaction.WalletId);
                if (wallet == null)
                {
                    return new ServiceResult
                    {
                        StatusCode = Const.WARNING_NO_DATA_CODE,
                        Message = "Ví không tồn tại"
                    };
                }

                // Check if payment successful (vnp_ResponseCode = "00")
                if (callback.vnp_ResponseCode == "00")
                {
                    // Update wallet balance
                    wallet.Balance += transaction.Amount;
                    wallet.UpdatedAt = DateTime.Now;
                    await _unitOfWork.WalletRepository.UpdateWalletAsync(wallet);

                    // Update transaction description with VNPay details
                    transaction.Description = $"{transaction.Description} - Thanh toán thành công qua VNPay (TxnNo: {callback.vnp_TransactionNo})";

                    return new ServiceResult
                    {
                        StatusCode = Const.SUCCESS_READ_CODE,
                        Message = "Nạp tiền thành công",
                        Data = new
                        {
                            Success = true,
                            TransactionId = transactionId,
                            Amount = transaction.Amount,
                            NewBalance = wallet.Balance,
                            VnpTransactionNo = callback.vnp_TransactionNo
                        }
                    };
                }
                else
                {
                    // Payment failed
                    transaction.Description = $"{transaction.Description} - Thanh toán thất bại (Mã lỗi: {callback.vnp_ResponseCode})";

                    return new ServiceResult
                    {
                        StatusCode = Const.FAIL_CREATE_CODE,
                        Message = $"Thanh toán thất bại: {GetVNPayResponseMessage(callback.vnp_ResponseCode)}",
                        Data = new
                        {
                            Success = false,
                            TransactionId = transactionId,
                            VnpResponseCode = callback.vnp_ResponseCode
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling VNPay wallet return");
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_EXCEPTION,
                    Message = $"Lỗi khi xử lý callback VNPay: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Get VNPay response code message
        /// </summary>
        private string GetVNPayResponseMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường)",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP)",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày",
                "75" => "Ngân hàng thanh toán đang bảo trì",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định",
                _ => "Giao dịch thất bại"
            };
        }
    }
}

