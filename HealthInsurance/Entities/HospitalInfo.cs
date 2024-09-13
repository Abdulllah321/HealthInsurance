using System.ComponentModel.DataAnnotations;

namespace HealthInsurance.Entities
{
    public class HospitalInfo
    {
        [Key]
        [Required]
        public string HospitalId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Hospital name is required.")]
        [StringLength(100, ErrorMessage = "Hospital name cannot be longer than 100 characters.")]
        public string HospitalName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string PhoneNo { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200, ErrorMessage = "Location cannot be longer than 200 characters.")]
        public string Location { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string Url { get; set; }

        // Navigation property (optional)
         public ICollection<Policy> Policy { get; set; }
    }
}
