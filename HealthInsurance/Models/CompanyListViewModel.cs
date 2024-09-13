using System.Collections.Generic;

namespace HealthInsurance.Entities
{
    public class CompanyListViewModel
    {
        public IEnumerable<CompanyDetails> Companies { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchString { get; set; }
    }
}
