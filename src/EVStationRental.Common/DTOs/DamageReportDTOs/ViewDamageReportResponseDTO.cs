using EVStationRental.Common.Enums.EnumModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVStationRental.Common.DTOs.DamageReportDTOs
{
    public class ViewDamageReportResponseDTO
    {
        public Guid DamageId { get; set; }

        public Guid OrderId { get; set; }

        public Guid VehicleId { get; set; }

        public string Description { get; set; } = null!;

        public decimal EstimatedCost { get; set; }

        public DamageLevelEnum DamageLevel { get; set; }

        public string? Img { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool Isactive { get; set; }
    }
}
