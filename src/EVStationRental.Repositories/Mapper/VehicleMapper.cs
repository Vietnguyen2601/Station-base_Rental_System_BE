using EVStationRental.Common.DTOs.VehicleDTOs;
using EVStationRental.Repositories.Models;
using System;

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

        public static Vehicle ToVehicle(this CreateVehicleRequestDTO dto)
        {
            return new Vehicle
            {
                VehicleId = Guid.NewGuid(),
                SerialNumber = dto.SerialNumber,
                ModelId = dto.ModelId,
                StationId = dto.StationId,
                BatteryLevel = dto.BatteryLevel,
                LocationLat = dto.LocationLat,
                LocationLong = dto.LocationLong,
                LastMaintenance = dto.LastMaintenance,
                CreatedAt = DateTime.Now
            };
        }

        public static void MapToVehicle(this UpdateVehicleRequestDTO dto, Vehicle vehicle)
        {
            // Không cho ch?nh s?a VehicleId
            if (dto.StationId != null) vehicle.StationId = dto.StationId;
            if (dto.ModelId != null) vehicle.ModelId = dto.ModelId.Value;
            if (dto.BatteryLevel != null) vehicle.BatteryLevel = dto.BatteryLevel;
            if (dto.LastMaintenance != null) vehicle.LastMaintenance = dto.LastMaintenance;
            if (dto.LocationLat != null) vehicle.LocationLat = dto.LocationLat;
            if (dto.LocationLong != null) vehicle.LocationLong = dto.LocationLong;
            vehicle.UpdatedAt = DateTime.Now;
        }
    }
}
