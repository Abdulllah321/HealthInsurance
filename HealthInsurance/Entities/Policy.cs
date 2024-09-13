using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthInsurance.Entities
{
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required]
        public string PolicyName { get; set; }

        public string PolicyDescription { get; set; }

        [Required]
        public string Amount { get; set; }

        [Required]
        public string Emi { get; set; }

        // Foreign key to CompanyDetails
        [Required] // Add this attribute to ensure validation
        [ForeignKey("CompanyDetails")]
        public int CompanyId { get; set; }

        public CompanyDetails CompanyDetails { get; set; }

        // Foreign key to HospitalInfo
        [Required] // Add this attribute to ensure validation
        [ForeignKey("HospitalInfo")]
        public string MedicalId { get; set; }

        public HospitalInfo HospitalInfo { get; set; }
    }
}