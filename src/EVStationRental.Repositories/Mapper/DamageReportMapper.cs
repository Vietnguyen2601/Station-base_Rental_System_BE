using EVStationRental.Common.DTOs.DamageReportDTOs;
using EVStationRental.Common.DTOs.FeedbackDTOs;
using EVStationRental.Common.DTOs.VehicleModelDTOs;
using EVStationRental.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Repositories.Mapper
{
    public static class DamageReportMapper
    {
        public static ViewDamageReportResponseDTO ToViewDamageReportDTO(this DamageReport damageReport)
        {
            return new ViewDamageReportResponseDTO
            {
                DamageId = damageReport.DamageId,
                OrderId = damageReport.OrderId,
                VehicleId = damageReport.VehicleId,
                Description = damageReport.Description,
                EstimatedCost = damageReport.EstimatedCost,
                DamageLevel = damageReport.DamageLevel,
                Img = damageReport.Img,
                CreatedAt = damageReport.CreatedAt,
                UpdatedAt = damageReport.UpdatedAt,
                Isactive = damageReport.Isactive
            };
        }
        public static DamageReport ToDamageReport(this CreateDamageReportRequestDTO dto)
        {
            return new DamageReport
            {
                DamageId = Guid.NewGuid(),
                OrderId = dto.OrderId,
                VehicleId = dto.VehicleId,
                Description = dto.Description,
                EstimatedCost = dto.EstimatedCost,
                DamageLevel = dto.DamageLevel,
                Img = dto.Img,
                CreatedAt = DateTime.Now
            };
        }
        public static void MapToDamageReportModel(this UpdateDamageReportRequestDTO dto, DamageReport damageReport)
        {
            if (dto.Description != null) damageReport.Description = dto.Description;
            if (dto.EstimatedCost != null) damageReport.EstimatedCost = dto.EstimatedCost;
            if (dto.DamageLevel != null) damageReport.DamageLevel = dto.DamageLevel;
            if (dto.Img != null) damageReport.Img = dto.Img;
            damageReport.UpdatedAt = DateTime.Now;
        }
    }
}
