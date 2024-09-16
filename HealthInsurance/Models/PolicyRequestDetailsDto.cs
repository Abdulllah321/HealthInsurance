using HealthInsurance.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HealthInsurance.Models
{
    public class PolicyRequestDetailsDto
    {
        [Key]
        public int RequestId { get; set; }

        public DateTime RequestDate { get; set; }

        public int EmpNo { get; set; }
        
        public int PolicyId { get; set; }

        public decimal PolicyAmount { get; set; }
        public decimal EMI { get; set; }

        public int CompanyId { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
    }
}
