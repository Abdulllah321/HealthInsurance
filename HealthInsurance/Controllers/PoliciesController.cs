using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthInsurance.Entities;

namespace HealthInsurance.Controllers
{
    [Route("admin/[controller]")]
    public class PoliciesController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10; // Set the default page size

        public PoliciesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /admin/Policies
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AmountSortParam"] = sortOrder == "Amount" ? "amount_desc" : "Amount";

            IQueryable<Policy> policies = _context.Policies
                .Include(p => p.CompanyDetails)
                .Include(p => p.HospitalInfo);

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                policies = policies.Where(p => p.PolicyName.Contains(searchString) ||
                                               p.CompanyDetails.CompanyName.Contains(searchString) ||
                                               p.HospitalInfo.HospitalName.Contains(searchString));
            }

            // Sorting functionality
            switch (sortOrder)
            {
                case "name_desc":
                    policies = policies.OrderByDescending(p => p.PolicyName);
                    break;
                case "Amount":
                    policies = policies.OrderBy(p => p.Amount);
                    break;
                case "amount_desc":
                    policies = policies.OrderByDescending(p => p.Amount);
                    break;
                default:
                    policies = policies.OrderBy(p => p.PolicyName);
                    break;
            }

            // Pagination
            var totalItems = await policies.CountAsync();
            var policiesPaged = await policies
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var model = new PolicyIndexViewModel
            {
                Policies = policiesPaged,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize),
                SearchString = searchString,
                SortOrder = sortOrder
            };

            return View(model);
        }

        // GET: /admin/Policies/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _context.Policies
                .Include(p => p.CompanyDetails)
                .Include(p => p.HospitalInfo)
                .FirstOrDefaultAsync(m => m.PolicyId == id);
            if (policy == null)
            {
                return NotFound();
            }

            return View(policy);
        }

        // GET: /admin/Policies/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.CompanyDetails, "CompanyId", "CompanyName");
            ViewData["MedicalId"] = new SelectList(_context.HospitalInfo, "HospitalId", "HospitalName");
            return View();
        }

        // POST: /admin/Policies/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PolicyId,PolicyName,PolicyDescription,Amount,Emi,CompanyId,MedicalId")] Policy policy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(policy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.CompanyDetails, "CompanyId", "CompanyName", policy.CompanyId);
            ViewData["MedicalId"] = new SelectList(_context.HospitalInfo, "HospitalId", "HospitalName", policy.MedicalId);
            return View(policy);
        }

        // GET: /admin/Policies/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _context.Policies.FindAsync(id);
            if (policy == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.CompanyDetails, "CompanyId", "CompanyName", policy.CompanyId);
            ViewData["MedicalId"] = new SelectList(_context.HospitalInfo, "HospitalId", "HospitalName", policy.MedicalId);
            return View(policy);
        }

        // POST: /admin/Policies/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PolicyId,PolicyName,PolicyDescription,Amount,Emi,CompanyId,MedicalId")] Policy policy)
        {
            if (id != policy.PolicyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(policy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PolicyExists(policy.PolicyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.CompanyDetails, "CompanyId", "CompanyName", policy.CompanyId);
            ViewData["MedicalId"] = new SelectList(_context.HospitalInfo, "HospitalId", "HospitalName", policy.MedicalId);
            return View(policy);
        }

        // GET: /admin/Policies/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policy = await _context.Policies
                .Include(p => p.CompanyDetails)
                .Include(p => p.HospitalInfo)
                .FirstOrDefaultAsync(m => m.PolicyId == id);
            if (policy == null)
            {
                return NotFound();
            }

            return View(policy);
        }

        // POST: /admin/Policies/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var policy = await _context.Policies.FindAsync(id);
            if (policy != null)
            {
                _context.Policies.Remove(policy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PolicyExists(int id)
        {
            return _context.Policies.Any(e => e.PolicyId == id);
        }
    }
}
