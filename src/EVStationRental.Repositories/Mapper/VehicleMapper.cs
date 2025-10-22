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
                BatteryCapacity = vehicle.BatteryCapacity,
                Range = vehicle.Range,
                Color = vehicle.Color,
                LastMaintenance = vehicle.LastMaintenance,
                Img = vehicle.Img,
                CreatedAt = vehicle.CreatedAt,
                UpdatedAt = vehicle.UpdatedAt,
                Isactive = vehicle.Isactive
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
                BatteryCapacity = dto.BatteryCapacity,
                Range = dto.Range,
                Color = dto.Color,
                LastMaintenance = dto.LastMaintenance,
                Img = dto.Img,
                CreatedAt = DateTime.Now,
                Isactive = dto.Isactive ?? true
            };
        }

        public static void MapToVehicle(this UpdateVehicleRequestDTO dto, Vehicle vehicle)
        {
            if (dto.StationId != null) vehicle.StationId = dto.StationId;
            if (dto.ModelId != null) vehicle.ModelId = dto.ModelId.Value;
            if (dto.BatteryLevel != null) vehicle.BatteryLevel = dto.BatteryLevel;
            if (dto.BatteryCapacity != null) vehicle.BatteryCapacity = dto.BatteryCapacity;
            if (dto.Range != null) vehicle.Range = dto.Range;
            if (dto.Color != null) vehicle.Color = dto.Color;
            if (dto.LastMaintenance != null) vehicle.LastMaintenance = dto.LastMaintenance;
            if (dto.Img != null) vehicle.Img = dto.Img;
            if (dto.Isactive != null) vehicle.Isactive = dto.Isactive.Value;
            vehicle.UpdatedAt = DateTime.Now;
        }
    }
}
