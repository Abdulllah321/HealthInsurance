using HealthInsurance.Entities;
using System.Collections.Generic;

namespace HealthInsurance.Entities
{
    public class PolicyIndexViewModel
    {
        public IEnumerable<Policy> Policies { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchString { get; set; }
        public string SortOrder { get; set; }
        public string NameSortParam { get; set; }
        public string AmountSortParam { get; set; }
    }
}
