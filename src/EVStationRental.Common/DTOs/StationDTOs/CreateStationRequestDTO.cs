using System.ComponentModel.DataAnnotations;

namespace EVStationRental.Common.DTOs.StationDTOs
{
    public class CreateStationRequestDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public decimal Lat { get; set; }
        public decimal Long { get; set; }
        public int Capacity { get; set; }
    }
}
