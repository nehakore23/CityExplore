using System.ComponentModel.DataAnnotations;

namespace CityExplorer.Models.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Number of people is required")]
        [Range(1, 20, ErrorMessage = "Number of people must be between 1 and 20")]
        public int NumberOfPeople { get; set; }

        [StringLength(500, ErrorMessage = "Special requests cannot exceed 500 characters")]
        public string? SpecialRequests { get; set; }

        public string? CityName { get; set; }
        public string? Country { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
