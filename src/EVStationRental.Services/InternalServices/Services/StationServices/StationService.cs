using EVStationRental.Common.DTOs.StationDTOs;
using EVStationRental.Common.Enums.ServiceResultEnum;
using EVStationRental.Repositories.Mapper;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.Base;
using EVStationRental.Services.InternalServices.IServices.IStationServices;
using System.Threading.Tasks;

namespace EVStationRental.Services.InternalServices.Services.StationServices
{
    public class StationService : IStationService
    {
        private readonly IUnitOfWork unitOfWork;

        public StationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IServiceResult> CreateStationAsync(CreateStationRequestDTO dto)
        {
            var station = dto.ToStation();
            var result = await unitOfWork.StationRepository.CreateStationAsync(station);
            return new ServiceResult
            {
                StatusCode = Const.SUCCESS_CREATE_CODE,
                Message = Const.SUCCESS_CREATE_MSG,
                Data = result
            };
        }
    }
}
