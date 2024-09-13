using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthInsurance.Entities;
using HealthInsurance.Models;

namespace HealthInsurance.Controllers
{
    [Route("admin/hospital")]
    public class HospitalController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10; // Number of items per page



        public HospitalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: admin/hospital
        [HttpGet]
        public async Task<IActionResult> Index(
    string searchString,
    string sortOrder,
    int page = 1)
        {
            IQueryable<HospitalInfo> hospitals = _context.HospitalInfo;

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                hospitals = hospitals.Where(h => h.HospitalName.Contains(searchString));
            }

            // Sorting functionality
            switch (sortOrder)
            {
                case "name_desc":
                    hospitals = hospitals.OrderByDescending(h => h.HospitalName);
                    break;
                case "phone":
                    hospitals = hospitals.OrderBy(h => h.PhoneNo);
                    break;
                case "phone_desc":
                    hospitals = hospitals.OrderByDescending(h => h.PhoneNo);
                    break;
                case "location":
                    hospitals = hospitals.OrderBy(h => h.Location);
                    break;
                case "location_desc":
                    hospitals = hospitals.OrderByDescending(h => h.Location);
                    break;
                case "url":
                    hospitals = hospitals.OrderBy(h => h.Url);
                    break;
                case "url_desc":
                    hospitals = hospitals.OrderByDescending(h => h.Url);
                    break;
                default:
                    hospitals = hospitals.OrderBy(h => h.HospitalName);
                    break;
            }

            // Pagination
            var totalItems = await hospitals.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var hospitalsPage = await hospitals
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new HospitalIndexViewModel
            {
                Hospitals = hospitalsPage,
                CurrentPage = page,
                TotalPages = totalPages,
                SearchString = searchString,
                SortOrder = sortOrder
            };

            return View(viewModel);
        }


        // GET: admin/hospital/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospitalInfo = await _context.HospitalInfo
                .FirstOrDefaultAsync(m => m.HospitalId == id);
            if (hospitalInfo == null)
            {
                return NotFound();
            }

            return View(hospitalInfo);
        }

        // GET: admin/hospital/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: admin/hospital/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HospitalId,HospitalName,PhoneNo,Location,Url")] HospitalInfo hospitalInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hospitalInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hospitalInfo);
        }

        // GET: admin/hospital/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospitalInfo = await _context.HospitalInfo.FindAsync(id);
            if (hospitalInfo == null)
            {
                return NotFound();
            }
            return View(hospitalInfo);
        }

        // POST: admin/hospital/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("HospitalId,HospitalName,PhoneNo,Location,Url")] HospitalInfo hospitalInfo)
        {
            if (id != hospitalInfo.HospitalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hospitalInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HospitalInfoExists(hospitalInfo.HospitalId))
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
            return View(hospitalInfo);
        }

        // GET: admin/hospital/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospitalInfo = await _context.HospitalInfo
                .FirstOrDefaultAsync(m => m.HospitalId == id);
            if (hospitalInfo == null)
            {
                return NotFound();
            }

            return View(hospitalInfo);
        }

        // POST: admin/hospital/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hospitalInfo = await _context.HospitalInfo.FindAsync(id);
            if (hospitalInfo != null)
            {
                _context.HospitalInfo.Remove(hospitalInfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HospitalInfoExists(string id)
        {
            return _context.HospitalInfo.Any(e => e.HospitalId == id);
        }
    }
}
