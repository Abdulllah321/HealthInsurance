using HealthInsurance.Entities;
using System.ComponentModel.DataAnnotations;

namespace HealthInsurance.Models
{
    public class PolicyRequestDetailsUpdateDto
    {
        [Key]
        public int RequestId { get; set; }

        public DateTime RequestDate { get; set; }

        public int EmpNo { get; set; }

        public int PolicyId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Policy Amount must be a valid number.")]
        public decimal PolicyAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "EMI must be a valid number.")]
        public decimal EMI { get; set; }

        public int CompanyId { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }
}
