using System;
using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs.DashboardDTOs
{
    /// <summary>
    /// Filter DTO for Dashboard Reports
    /// </summary>
    public class DashboardFilterDTO
    {
        /// <summary>
        /// Filter by Month (1-12)
        /// </summary>
        public int? Month { get; set; }

        /// <summary>
        /// Filter by Year
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Filter by Quarter (1-4)
        /// </summary>
        [Range(1, 4)]
        public int? Quarter { get; set; }

        /// <summary>
        /// Custom start date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Custom end date
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
