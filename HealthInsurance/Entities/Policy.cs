using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual CompanyDetails CompanyDetails { get; set; }

        [Required]
        public Guid MedicalId { get; set; }

        [ForeignKey("MedicalId")]
        public virtual HospitalInfo HospitalInfo { get; set; }
    }

}
