using EVStationRental.Common.DTOs.StationDTOs;
using EVStationRental.Common.Enums.EnumModel;
using EVStationRental.Repositories.Models;
using System;
using System.Linq;

namespace EVStationRental.Repositories.Mapper
{
    public static class StationMapper
    {
        public static Station ToStation(this CreateStationRequestDTO dto)
        {
            return new Station
            {
                StationId = Guid.NewGuid(),
                Name = dto.Name,
                Address = dto.Address,
                Lat = dto.Lat,
                Long = dto.Long,
                Capacity = dto.Capacity,
            };
        }

        public static void MapToStation(this UpdateStationRequestDTO dto, Station station)
        {
            if (!string.IsNullOrEmpty(dto.Name)) station.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Address)) station.Address = dto.Address;
            if (dto.Lat != null) station.Lat = dto.Lat.Value;
            if (dto.Long != null) station.Long = dto.Long.Value;
            if (dto.Capacity != null) station.Capacity = dto.Capacity.Value;
            if (dto.ImageUrl != null) station.ImageUrl = dto.ImageUrl;
            if (dto.Isactive != null) station.Isactive = dto.Isactive.Value;
            station.UpdatedAt = DateTime.Now;
        }

        /// <summary>
        /// Map Station to ViewStationDTO (prevents circular reference)
        /// </summary>
        public static ViewStationDTO ToViewStationDTO(this Station station)
        {
            return new ViewStationDTO
            {
                StationId = station.StationId,
                Name = station.Name,
                Address = station.Address,
                Lat = station.Lat,
                Long = station.Long,
                Capacity = station.Capacity,
                ImageUrl = station.ImageUrl,
                IsActive = station.Isactive,
                CreatedAt = station.CreatedAt,
                UpdatedAt = station.UpdatedAt,
                TotalVehicles = station.Vehicles?.Count(v => v.Isactive) ?? 0,
                AvailableVehicles = station.Vehicles?.Count(v => v.Status == VehicleStatus.AVAILABLE) ?? 0,
                RentedVehicles = station.Vehicles?.Count(v => v.Status == VehicleStatus.RENTED) ?? 0,
                MaintenanceVehicles = station.Vehicles?.Count(v => v.Status == VehicleStatus.MAINTENANCE) ?? 0,
                ChargingVehicles = station.Vehicles?.Count(v => v.Status == VehicleStatus.CHARGING) ?? 0
            };
        }
    }
}
