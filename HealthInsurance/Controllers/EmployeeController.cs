using System.Security.Claims;
using System.Threading.Tasks;
using HealthInsurance.Entities;
using HealthInsurance.Models; // Ensure the correct namespace for your view models
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthInsurance.Controllers
{
    [Authorize(Roles = "Employee")] 
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // Employee Dashboard
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == username);

            if (employee == null)
            {
                return RedirectToAction("EmpLogin", "auth");
            }

            var dashboardViewModel = new EmpRegisterDto
            {
                EmpNo = employee.EmpNo,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Username = employee.Username,
                Designation = employee.Designation,
                Salary = employee.Salary,
                JoinDate = employee.JoinDate,
                Address = employee.Address,
                ContactNo = employee.ContactNo,
                City = employee.City,
                State = employee.State,
                Country = employee.Country,
                PolicyStatus = employee.PolicyStatus,
                CompanyId = employee.CompanyId
            };

            // Get policies associated with the employee
            var policies = await _context.PoliciesOnEmployees
                .Include(pe => pe.Policy)
                .Where(pe => pe.EmpNo == employee.EmpNo)
                .Select(pe => new PolicyDto
                {
                    PolicyId = pe.Policy.PolicyId,
                    PolicyName = pe.Policy.PolicyName,
                    PolicyDesc = pe.Policy.PolicyDesc,
                    PolicyAmount = pe.Policy.PolicyAmount,
                    EMI = pe.Policy.EMI,
                    Medicaid = pe.Policy.Medicaid
                })
                .ToListAsync();

            // Get policy requests associated with the employee
            var policyRequests = await _context.PolicyRequests
                .Where(pr => pr.EmpNo == employee.EmpNo)
                .Select(pr => new PolicyRequestDto
                {
                    RequestId = pr.RequestId,
                    Status = pr.Status,
                    RequestDate = pr.RequestDate,
                    PolicyId = pr.PolicyId,
                    PolicyName = pr.Policy.PolicyName,
                    PolicyAmount = pr.PolicyAmount,
                    EMI = pr.EMI
                })
                .ToListAsync();

            var employeeViewModel = new EmployeeDashboardViewModel
            {
                EmployeeDetails = dashboardViewModel,
                Policies = policies,
                PolicyRequests = policyRequests
            };

            return View(employeeViewModel);
        }

    }

    public class EmployeeDashboardViewModel
    {
        public EmpRegisterDto EmployeeDetails { get; set; }
        public List<PolicyDto> Policies { get; set; }
        public List<PolicyRequestDto> PolicyRequests { get; set; }
        public List<PolicyApprovalDto> PolicyApprovals { get; set; } // Add this for approvals
    }


    public class PolicyRequestDto
    {
        public int RequestId { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public int PolicyId { get; set; }
        public string PolicyName { get; set; } // Add this line
        public decimal PolicyAmount { get; set; }
        public decimal EMI { get; set; }
    }

    public class PolicyApprovalDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public int PolicyId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public bool Approved { get; set; }
        public string Reason { get; set; }
    }


}
