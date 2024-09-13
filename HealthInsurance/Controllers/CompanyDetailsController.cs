using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthInsurance.Entities;

namespace HealthInsurance.Controllers
{
    [Route("admin/[controller]")]
    public class CompanyDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public CompanyDetailsController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        // GET: CompanyDetails
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            const int pageSize = 10;

            IQueryable<CompanyDetails> companies = _context.CompanyDetails;

            if (!string.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(c => c.CompanyName.Contains(searchString) ||
                                                  c.Address.Contains(searchString) ||
                                                  c.Phone.Contains(searchString) ||
                                                  c.CompanyURL.Contains(searchString));
            }

            var totalItems = await companies.CountAsync();
            var companiesPaged = await companies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new CompanyListViewModel
            {
                Companies = companiesPaged,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                SearchString = searchString
            };

            return View(model);
        }

        [HttpGet("Details/{id}")]
        // GET: CompanyDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyDetails = await _context.CompanyDetails
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (companyDetails == null)
            {
                return NotFound();
            }

            return View(companyDetails);
        }

        // GET: CompanyDetails/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CompanyDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,CompanyName,Address,Phone,CompanyURL")] CompanyDetails companyDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(companyDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(companyDetails);
        }

        // GET: CompanyDetails/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyDetails = await _context.CompanyDetails.FindAsync(id);
            if (companyDetails == null)
            {
                return NotFound();
            }
            return View(companyDetails);
        }

        // POST: CompanyDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,CompanyName,Address,Phone,CompanyURL")] CompanyDetails companyDetails)
        {
            if (id != companyDetails.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyDetailsExists(companyDetails.CompanyId))
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
            return View(companyDetails);
        }

        [HttpGet("Delete/{id}")]
        // GET: CompanyDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyDetails = await _context.CompanyDetails
                .FirstOrDefaultAsync(m => m.CompanyId == id);
            if (companyDetails == null)
            {
                return NotFound();
            }

            return View(companyDetails);
        }

        // POST: CompanyDetails/Delete/5
        [HttpPost("Delete/{id}")]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var companyDetails = await _context.CompanyDetails.FindAsync(id);
            if (companyDetails != null)
            {
                _context.CompanyDetails.Remove(companyDetails);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyDetailsExists(int id)
        {
            return _context.CompanyDetails.Any(e => e.CompanyId == id);
        }
    }
}
