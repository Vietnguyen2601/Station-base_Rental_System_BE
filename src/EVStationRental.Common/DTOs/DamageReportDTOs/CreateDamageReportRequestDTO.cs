using EVStationRental.Common.Enums.EnumModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.DTOs.DamageReportDTOs
{
    public class CreateDamageReportRequestDTO
    {
        public Guid OrderId { get; set; }

        public Guid VehicleId { get; set; }

        public string Description { get; set; } = null!;

        public DamageLevelEnum DamageLevel { get; set; }

        public decimal EstimatedCost { get; set; }

        public string? Img { get; set; }
    }
}
