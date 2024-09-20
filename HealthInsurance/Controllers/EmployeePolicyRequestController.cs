using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthInsurance.Entities;
using HealthInsurance.Models;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;


namespace HealthInsurance.Controllers
{
    [Route("Employee/policyRequest")]
    public class EmployeePolicyRequestController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10; // Set the page size

        public EmployeePolicyRequestController(AppDbContext context)
        {
            _context = context;
        }

        // GET: admin/PolicyRequest
        [HttpGet("")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber = 1)
        {
            // Set up ViewData for sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParam"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["CompanySortParam"] = sortOrder == "Company" ? "company_desc" : "Company";

            var username = User.Identity.Name;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == username);

            // For keeping the search string in the view
            ViewData["CurrentFilter"] = searchString;

            // Get all policy requests
            var policyRequests = from p in _context.PolicyRequests
                                 .Where(pe=> pe.EmpNo == employee.EmpNo)
                                 .Include(p => p.Company)
                                 .Include(p => p.Employee)
                                 .Include(p => p.Policy)
                                 select p;

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                policyRequests = policyRequests.Where(p => p.Policy.PolicyName.Contains(searchString)
                                                        || p.Employee.FirstName.Contains(searchString)
                                                        || p.Company.CompanyName.Contains(searchString));
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "date_desc":
                    policyRequests = policyRequests.OrderByDescending(p => p.RequestDate);
                    break;
                case "Company":
                    policyRequests = policyRequests.OrderBy(p => p.Company.CompanyName);
                    break;
                case "company_desc":
                    policyRequests = policyRequests.OrderByDescending(p => p.Company.CompanyName);
                    break;
                default:
                    policyRequests = policyRequests.OrderBy(p => p.RequestDate);
                    break;
            }

            // Apply pagination
            var totalItems = await policyRequests.CountAsync();
            var policyRequestsToDisplay = await policyRequests
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new EmployeePolicyRequestIndexViewModel
            {
                PolicyRequests = policyRequestsToDisplay,
                SearchString = searchString,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
            };

            return View(viewModel);
        }

        // GET: admin/PolicyRequest/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyRequestDetails = await _context.PolicyRequests
                .Include(p => p.Company)
                .Include(p => p.Employee)
                .Include(p => p.Policy)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (policyRequestDetails == null)
            {
                return NotFound();
            }

            return View(policyRequestDetails);
        }

        // GET: admin/PolicyRequest/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {

            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName");
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyName");
            return View();
        }

        // POST: admin/PolicyRequest/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,RequestDate,PolicyId,PolicyAmount,EMI,CompanyId,Status")] EmployeePolicyRequestDetailsDto policyRequestDetailsdto)
        {
            if (ModelState.IsValid)
            {

                var username = User.Identity.Name;
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == username);
                var policyRequest = new PolicyRequestDetails
                {
                    RequestDate = policyRequestDetailsdto.RequestDate,
                    EmpNo = employee.EmpNo,
                    PolicyId = policyRequestDetailsdto.PolicyId,
                    PolicyAmount = policyRequestDetailsdto.PolicyAmount,
                    EMI = policyRequestDetailsdto.EMI,
                    CompanyId = policyRequestDetailsdto.CompanyId,
                };

                _context.PolicyRequests.Add(policyRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
   
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName");
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName");
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyName");
            return View(policyRequestDetailsdto);
        }

        // GET: admin/PolicyRequest/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyRequestDetails = await _context.PolicyRequests.FindAsync(id);
            if (policyRequestDetails == null)
            {
                return NotFound();
            }
            ViewBag.Status = new SelectList(new List<SelectListItem>
    {
        new SelectListItem { Value = "Pending", Text = "Pending" },
        new SelectListItem { Value = "Approved", Text = "Approved" },
        new SelectListItem { Value = "Rejected", Text = "Rejected" }
    }, "Value", "Text");
            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName", policyRequestDetails.CompanyId);
            ViewData["EmpNo"] = new SelectList(_context.Employees, "EmpNo", "FirstName", policyRequestDetails.EmpNo);
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyName", policyRequestDetails.PolicyId);
            return View(policyRequestDetails);
        }

        // POST: admin/PolicyRequest/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,RequestDate,EmpNo,PolicyId,PolicyAmount,EMI,CompanyId,Status")] EmployeePolicyRequestDetailsUpdateDto policyRequestDetails)
        {
            if (id != policyRequestDetails.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPolicyRequest = await _context.PolicyRequests.FindAsync(id);
                    if (existingPolicyRequest == null)
                    {
                        return NotFound();
                    }
                    var username = User.Identity.Name;
                    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == username);

                    existingPolicyRequest.RequestDate = policyRequestDetails.RequestDate;
                    existingPolicyRequest.EmpNo = employee.EmpNo;
                    existingPolicyRequest.PolicyId = policyRequestDetails.PolicyId;
                    existingPolicyRequest.PolicyAmount = policyRequestDetails.PolicyAmount;
                    existingPolicyRequest.EMI = policyRequestDetails.EMI;
                    existingPolicyRequest.CompanyId = policyRequestDetails.CompanyId;
                    existingPolicyRequest.Status = policyRequestDetails.Status;

                    _context.PolicyRequests.Update(existingPolicyRequest);
                    await _context.SaveChangesAsync();

                    // Redirect to PolicyApproval Create page if status is Approved or Rejected
                    if (policyRequestDetails.Status == "Approved" || policyRequestDetails.Status == "Rejected")
                    {
                        bool isApproved = policyRequestDetails.Status == "Approved";
                        return RedirectToAction("Create", "PolicyApproval", new { requestId = policyRequestDetails.RequestId, isApproved });
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PolicyRequestDetailsExists(policyRequestDetails.RequestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Status = new SelectList(new List<SelectListItem>
    {
        new SelectListItem { Value = "Pending", Text = "Pending" },
        new SelectListItem { Value = "Approved", Text = "Approved" },
        new SelectListItem { Value = "Rejected", Text = "Rejected" }
    }, "Value", "Text");

            ViewData["CompanyId"] = new SelectList(_context.Companies, "CompanyId", "CompanyName", policyRequestDetails.CompanyId);
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyName", policyRequestDetails.PolicyId);

            return View(policyRequestDetails);
        }


        // GET: admin/PolicyRequest/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyRequestDetails = await _context.PolicyRequests
                .Include(p => p.Company)
                .Include(p => p.Employee)
                .Include(p => p.Policy)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (policyRequestDetails == null)
            {
                return NotFound();
            }

            return View(policyRequestDetails);
        }

        // POST: admin/PolicyRequest/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var policyRequestDetails = await _context.PolicyRequests.FindAsync(id);
            if (policyRequestDetails != null)
            {
                _context.PolicyRequests.Remove(policyRequestDetails);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PolicyRequestDetailsExists(int id)
        {
            return _context.PolicyRequests.Any(e => e.RequestId == id);
        }
    }

    public class EmployeePolicyRequestDetailsUpdateDto
    {
        [Key]
        public int RequestId { get; set; }

        public DateTime RequestDate { get; set; }

        public int PolicyId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Policy Amount must be a valid number.")]
        public decimal PolicyAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "EMI must be a valid number.")]
        public decimal EMI { get; set; }

        public int CompanyId { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
    }

    public class EmployeePolicyRequestDetailsDto
    {

        [Key]
        public int RequestId { get; set; }

        public DateTime RequestDate { get; set; }
        public int PolicyId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Policy Amount must be a valid number.")]
        public decimal PolicyAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "EMI must be a valid number.")]
        public decimal EMI { get; set; }

        public int CompanyId { get; set; }
    }

    public class EmployeePolicyRequestIndexViewModel
    {
        public IEnumerable<PolicyRequestDetails> PolicyRequests { get; set; }
        public string SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
