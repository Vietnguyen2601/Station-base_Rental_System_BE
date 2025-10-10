using EVStationRental.Common.DTOs.StationDTOs;
using EVStationRental.Repositories.Models;
using System;

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
                CreatedAt = DateTime.Now
            };
        }

        public static void MapToStation(this UpdateStationRequestDTO dto, Station station)
        {
            if (!string.IsNullOrEmpty(dto.Name)) station.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Address)) station.Address = dto.Address;
            if (dto.Lat != null) station.Lat = dto.Lat.Value;
            if (dto.Long != null) station.Long = dto.Long.Value;
            if (dto.Capacity != null) station.Capacity = dto.Capacity.Value;
            station.UpdatedAt = DateTime.Now;
        }
    }
}
