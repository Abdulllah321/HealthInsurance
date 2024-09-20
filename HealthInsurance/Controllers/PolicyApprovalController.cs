using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthInsurance.Entities;
using HealthInsurance.Models;
using Azure.Core;

namespace HealthInsurance.Controllers
{
    [Route("admin/[controller]")]
    public class PolicyApprovalController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10; // Set the page size

        public PolicyApprovalController(AppDbContext context)
        {
            _context = context;
        }

        // GET: admin/PolicyApproval
        [HttpGet("")]
        public async Task<IActionResult> Index(string sortOrder, string searchString, int pageNumber = 1)
        {
            // Set up ViewData for sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParam"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["AmountSortParam"] = sortOrder == "Amount" ? "amount_desc" : "Amount";

            // For keeping the search string in the view
            ViewData["CurrentFilter"] = searchString;

            // Get all policy approvals
            var policyApprovals = from p in _context.PolicyApprovals
                                  .Include(p => p.Policy)
                                  select p;

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                policyApprovals = policyApprovals.Where(p => p.Policy.PolicyName.Contains(searchString)
                                                            || p.Reason.Contains(searchString));
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "date_desc":
                    policyApprovals = policyApprovals.OrderByDescending(p => p.Date);
                    break;
                case "Amount":
                    policyApprovals = policyApprovals.OrderBy(p => p.Amount);
                    break;
                case "amount_desc":
                    policyApprovals = policyApprovals.OrderByDescending(p => p.Amount);
                    break;
                default:
                    policyApprovals = policyApprovals.OrderBy(p => p.Date);
                    break;
            }

            // Apply pagination
            var totalItems = await policyApprovals.CountAsync();
            var policyApprovalsToDisplay = await policyApprovals
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new PolicyApprovalIndexViewModel
            {
                PolicyApprovals = policyApprovalsToDisplay,
                SearchString = searchString,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize)
            };

            return View(viewModel);
        }

        // GET: admin/PolicyApproval/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyApproval = await _context.PolicyApprovals
                .Include(p => p.Policy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (policyApproval == null)
            {
                return NotFound();
            }

            return View(policyApproval);
        }

        // GET: admin/PolicyApproval/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create(int requestId, bool isApproved)
        {
            // Fetch all policies for dropdown
            ViewData["PolicyId"] = new SelectList(await _context.Policies.ToListAsync(), "PolicyId", "PolicyName");

            // Get all available request IDs
            var existingRequestIds = _context.PolicyApprovals.Select(pa => pa.RequestId);
            var availableRequests = await _context.PolicyRequests
                .Where(pr => !existingRequestIds.Contains(pr.RequestId))
                .ToListAsync();

            ViewData["RequestId"] = new SelectList(_context.PolicyRequests, "RequestId", "RequestId", availableRequests);

            // Pass the isApproved value as part of ViewData
            ViewData["IsApproved"] = isApproved;

            return View();
        }



        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PolicyId,Date,Amount,Approved,Reason,RequestId")] PolicyApprovalDetailsDto policyApprovalDetails)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var approvalDetail = new PolicyApprovalDetails
                    {
                        RequestId = policyApprovalDetails.RequestId,
                        PolicyId = policyApprovalDetails.PolicyId,
                        Date = policyApprovalDetails.Date,
                        Amount = policyApprovalDetails.Amount,
                        Approved = policyApprovalDetails.Approved,
                        Reason = policyApprovalDetails.Reason
                    };

                    _context.PolicyApprovals.Add(approvalDetail);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception (if you have a logging mechanism)
                    ModelState.AddModelError("", "An error occurred while creating the policy approval. Please try again.");
                }
            }
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyName", policyApprovalDetails.PolicyId);

            // Fetch RequestIds that are not in PolicyApprovals
            var existingRequestIds = _context.PolicyApprovals.Select(pa => pa.RequestId);
            var availableRequests = await _context.PolicyRequests
                .Where(pr => !existingRequestIds.Contains(pr.RequestId))
                .ToListAsync();

            ViewData["RequestId"] = new SelectList(_context.PolicyRequests, "RequestId", "RequestId", availableRequests);
            return View(policyApprovalDetails);
        }



        // GET: admin/PolicyApproval/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyApprovalDetails = await _context.PolicyApprovals.FindAsync(id);
            if (policyApprovalDetails == null)
            {
                return NotFound();
            }
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyName", policyApprovalDetails.PolicyId);
            ViewData["RequestId"] = new SelectList(_context.PolicyRequests, "RequestId", "RequestId", policyApprovalDetails.RequestId);
            return View(policyApprovalDetails);
        }

        // POST: admin/PolicyApproval/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PolicyId,Date,Amount,Approved,Reason")] PolicyApprovalDetailsDto policyApprovalDetailsDto)
        {
            if (id != policyApprovalDetailsDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing record from the database
                    var existingApprovalDetail = await _context.PolicyApprovals.FindAsync(id);

                    if (existingApprovalDetail == null)
                    {
                        return NotFound();
                    }

                    // Update the existing record with the new values
                    existingApprovalDetail.RequestId = policyApprovalDetailsDto.RequestId;
                    existingApprovalDetail.PolicyId = policyApprovalDetailsDto.PolicyId;
                    existingApprovalDetail.Date = policyApprovalDetailsDto.Date;
                    existingApprovalDetail.Amount = policyApprovalDetailsDto.Amount;
                    existingApprovalDetail.Approved = policyApprovalDetailsDto.Approved;
                    existingApprovalDetail.Reason = policyApprovalDetailsDto.Reason;

                    _context.PolicyApprovals.Update(existingApprovalDetail);
                    // Save changes
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PolicyApprovalDetailsExists(policyApprovalDetailsDto.Id))
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

            // Reload the policy list for the view
            ViewData["PolicyId"] = new SelectList(_context.Policies, "PolicyId", "PolicyName", policyApprovalDetailsDto.PolicyId);
            ViewData["RequestId"] = policyApprovalDetailsDto.RequestId;
            return View(policyApprovalDetailsDto);
        }


        // GET: admin/PolicyApproval/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var policyApprovalDetails = await _context.PolicyApprovals
                .Include(p => p.Policy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (policyApprovalDetails == null)
            {
                return NotFound();
            }

            return View(policyApprovalDetails);
        }

        // POST: admin/PolicyApproval/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var policyApprovalDetails = await _context.PolicyApprovals.FindAsync(id);
            if (policyApprovalDetails != null)
            {
                _context.PolicyApprovals.Remove(policyApprovalDetails);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PolicyApprovalDetailsExists(int id)
        {
            return _context.PolicyApprovals.Any(e => e.Id == id);
        }
    }

    public class PolicyApprovalIndexViewModel
    {
        public IEnumerable<PolicyApprovalDetails> PolicyApprovals { get; set; }
        public string SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
