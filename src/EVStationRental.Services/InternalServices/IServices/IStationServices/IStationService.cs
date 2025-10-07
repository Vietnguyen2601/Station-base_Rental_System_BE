using System.Threading.Tasks;
using EVStationRental.Common.DTOs.StationDTOs;
using EVStationRental.Services.Base;

namespace EVStationRental.Services.InternalServices.IServices.IStationServices
{
    public interface IStationService
    {
        Task<IServiceResult> CreateStationAsync(CreateStationRequestDTO dto);
    }
}
