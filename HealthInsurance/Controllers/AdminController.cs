using HealthInsurance.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthInsurance.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await GetViewModel();
            return View(viewModel);
        }

        private async Task<DashboardViewModel> GetViewModel()
        {
            var totalPolicies = await _context.Policies.CountAsync();
            var claimsOverview = await _context.PolicyRequests.CountAsync(a => a.Status == "Pending");
            var activePolicies = await _context.Policies.CountAsync(p => p.PolicyAmount > 0);
            var pendingApprovals = await _context.PolicyApprovals.CountAsync(a => !a.Approved);

            var totalClaimsAmount = await _context.PolicyRequests.SumAsync(r => r.PolicyAmount);
            var averagePolicyAmount = await _context.Policies.AnyAsync()
                ? Math.Round(await _context.Policies.AverageAsync(p => p.PolicyAmount), 2)
                : 0;

            var employees = await _context.Employees
                .Select(e => new EmployeeViewModel
                {
                    EmployeeName = e.FirstName + " " + e.LastName,
                    Designation = e.Designation,
                    JoinDate = e.JoinDate,
                    Salary = e.Salary,
                    ContactNo = e.ContactNo
                })
                .Take(8) // Limit to 8 employees
                .ToListAsync();

            var recentPolicyRequests = await _context.PolicyRequests
                .OrderByDescending(r => r.RequestDate)
                .Take(8) // Limit to 8 recent policy requests
                .Select(r => new PolicyRequestViewModel
                {
                    PolicyName = _context.Policies.FirstOrDefault(p => p.PolicyId == r.PolicyId).PolicyName,
                    Status = r.Status,
                    RequestDate = r.RequestDate
                })
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalPolicies = totalPolicies,
                ClaimsOverview = claimsOverview,
                ActivePolicies = activePolicies,
                PendingApprovals = pendingApprovals,
                CompanyLabels = await _context.Companies.Select(c => c.CompanyName).ToListAsync(),
                CompanyData = await _context.Companies
                    .Select(c => _context.Policies.Count(p => p.CompanyId == c.CompanyId))
                    .ToListAsync(),
                PolicyNames = await _context.Policies.Select(p => p.PolicyName).ToListAsync(),
                PolicyAmounts = await _context.Policies.Select(p => p.PolicyAmount).ToListAsync(),
                EMIData = await _context.Policies.Select(p => p.EMI).ToListAsync(),
                RecentPolicyRequests = recentPolicyRequests,
                PolicyRequestStatuses = new List<string> { "Pending", "Rejected", "Approved" },
                PolicyRequestCounts = new List<int>
                {
                    await _context.PolicyRequests.CountAsync(r => r.Status == "Pending"),
                    await _context.PolicyRequests.CountAsync(r => r.Status == "Rejected"),
                    await _context.PolicyRequests.CountAsync(r => r.Status == "Approved")
                },
                TotalClaimsAmount = totalClaimsAmount,
                AveragePolicyAmount = averagePolicyAmount,

             
                Employees = employees
            };
        }
    }

    public class DashboardViewModel
    {
        public int TotalPolicies { get; set; }
        public int ClaimsOverview { get; set; }
        public int ActivePolicies { get; set; }
        public int PendingApprovals { get; set; }

        public List<string> CompanyLabels { get; set; }
        public List<int> CompanyData { get; set; }
        public List<string> PolicyNames { get; set; }
        public List<decimal> PolicyAmounts { get; set; }
        public List<decimal> EMIData { get; set; }
        public List<PolicyRequestViewModel> RecentPolicyRequests { get; set; }
        public List<string> PolicyRequestStatuses { get; set; }
        public List<int> PolicyRequestCounts { get; set; }

        public decimal TotalClaimsAmount { get; set; }
        public decimal AveragePolicyAmount { get; set; }

        // Data for hospitals and employees
        public List<EmployeeViewModel> Employees { get; set; }
    }


    public class EmployeeViewModel
    {
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public DateTime JoinDate { get; set; }
        public decimal Salary { get; set; }
        public string ContactNo { get; set; }
    }

    public class PolicyRequestViewModel
    {
        public string PolicyName { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
