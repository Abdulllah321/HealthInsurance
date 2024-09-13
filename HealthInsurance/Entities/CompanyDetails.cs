using System.ComponentModel.DataAnnotations;

namespace HealthInsurance.Entities
{
    public class CompanyDetails
    {
        [Key]
        public int CompanyId { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Phone { get; set; }

        public string CompanyURL { get; set; }
    }
}
