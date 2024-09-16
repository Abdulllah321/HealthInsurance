﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthInsurance.Entities
{
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required]
        [StringLength(50)]
        public string PolicyName { get; set; }

        [StringLength(250)]
        public string PolicyDesc { get; set; }

        [DataType(DataType.Currency)]
        public decimal PolicyAmount { get; set; }

        [DataType(DataType.Currency)]
        public decimal EMI { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public CompanyDetails Company { get; set; }

        [StringLength(50)]
        public string Medicaid { get; set; }

        public ICollection<PoliciesOnEmployees> PoliciesOnEmployees { get; set; }
    }
}
