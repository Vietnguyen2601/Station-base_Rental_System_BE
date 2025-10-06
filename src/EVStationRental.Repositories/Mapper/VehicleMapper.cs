using EVStationRental.Common.DTOs.VehicleDTOs;
using EVStationRental.Repositories.Models;

namespace EVStationRental.Repositories.Mapper
{
    public static class VehicleMapper
    {
        public static ViewVehicleResponse ToViewVehicleDTO(this Vehicle vehicle)
        {
            return new ViewVehicleResponse
            {
                VehicleId = vehicle.VehicleId,
                SerialNumber = vehicle.SerialNumber,
                ModelId = vehicle.ModelId,
                StationId = vehicle.StationId,
                BatteryLevel = vehicle.BatteryLevel,
                LocationLat = vehicle.LocationLat,
                LocationLong = vehicle.LocationLong,
                LastMaintenance = vehicle.LastMaintenance,
                CreatedAt = vehicle.CreatedAt,
                UpdatedAt = vehicle.UpdatedAt
            };
        }
    }
}
