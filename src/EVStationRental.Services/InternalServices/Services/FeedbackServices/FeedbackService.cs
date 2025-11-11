using EVStationRental.Common.DTOs.FeedbackDTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Mapper;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IFeedbackServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Services.InternalServices.Services.FeedbackServices
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateFeedbackAsync(CreateFeedbackRequestDTO dto)
        {
            var hasCompleatedOrder = await _unitOfWork.OrderRepository
                .GetOrderByIdAsync(dto.OrderId);

            if (hasCompleatedOrder == null )
            {
                return new ServiceResult
                {
                    StatusCode = Const.ERROR_VALIDATION_CODE,
                    Message = "Khách hàng chỉ có thể đánh giá sau khi hoàn thành đơn hàng."
                };
            }

            var feedback = dto.ToFeedback();
            var result =  await _unitOfWork.FeedbackRepository.CreateFeedbackAsync(feedback);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_CREATE_CODE,
                Message = Const.SUCCESS_CREATE_MSG,
                Data = result.ToViewFeedbackDTO()
            };

        }

        public async Task<IServiceResult> GetAllFeedbackAsync()
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetAllFeedbacksAsync();
            var feedbackDtos = feedbacks.Select(f => f.ToViewFeedbackDTO()).ToList();
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = "Lấy danh sách đánh giá thành công.",
                Data = feedbackDtos
            };
        }

        public async Task<IServiceResult> GetFeedbackByIdAsync(Guid feedbackId)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetFeedbackByIdAsync(feedbackId);
            if (feedback == null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_NO_DATA_CODE,
                    Message = "Đánh giá không tồn tại."
                };
            }
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = "Lấy đánh giá thành công.",
                Data = feedback.ToViewFeedbackDTO()
            };
        }

        public async Task<IServiceResult> GetFeedbackByOrderIdAsync(Guid orderId)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetFeedbackByOrderIdAsync(orderId);
            if (feedback == null)
            {
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_NO_DATA_CODE,
                    Message = "Đánh giá không tồn tại."
                };
            }
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = "Lấy đánh giá thành công.",
                Data = feedback.ToViewFeedbackDTO()
            };
        }

        public async Task<IServiceResult> GetFeedbacksByCustomerIdAsync(Guid customerId)
        {
            var feedbacks = await _unitOfWork.FeedbackRepository.GetFeedbacksByCustomerIdAsync(customerId);
            var feedbackDtos = feedbacks.Select(f => f.ToViewFeedbackDTO()).ToList();
            if (feedbacks == null )
            {
                return new ServiceResult
                {
                    StatusCode = Const.WARNING_NO_DATA_CODE,
                    Message = "Khách hàng chưa có đánh giá nào."
                };
            }
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_READ_CODE,
                Message = "Lấy danh sách đánh giá của khách hàng thành công.",
                Data = feedbackDtos
            };

        }
    }
}
