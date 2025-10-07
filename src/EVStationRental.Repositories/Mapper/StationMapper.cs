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
    }
}
