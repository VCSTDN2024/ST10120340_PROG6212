using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize(Roles = "HR,Academic Manager")]
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HRController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: HR Dashboard - View approved claims ready for processing
        public async Task<IActionResult> Index()
        {
            var approvedClaims = await _context.LecturerClaims
                .Where(c => c.Status == "Approved" && !c.IsHRProcessed)
                .OrderBy(c => c.ReviewDate)
                .ToListAsync();

            var processedClaims = await _context.LecturerClaims
                .Where(c => c.IsHRProcessed)
                .OrderByDescending(c => c.HRProcessedDate)
                .Take(20)
                .ToListAsync();

            ViewBag.ApprovedCount = approvedClaims.Count;
            ViewBag.ProcessedCount = processedClaims.Count;
            ViewBag.TotalPendingAmount = approvedClaims.Sum(c => c.TotalAmount);

            return View(approvedClaims);
        }

        // GET: View single claim for invoice generation
        public async Task<IActionResult> ProcessClaim(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.LecturerClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            if (claim.Status != "Approved")
            {
                TempData["ErrorMessage"] = "Only approved claims can be processed by HR.";
                return RedirectToAction(nameof(Index));
            }

            return View(claim);
        }

        // POST: Generate Invoice for a claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInvoice(int id)
        {
            try
            {
                var claim = await _context.LecturerClaims.FindAsync(id);

                if (claim == null)
                {
                    return NotFound();
                }

                if (claim.Status != "Approved")
                {
                    TempData["ErrorMessage"] = "Only approved claims can have invoices generated.";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.GetUserAsync(User);

                // AUTOMATION: Generate invoice number automatically
                claim.GenerateInvoiceNumber();

                // Mark claim as HR processed
                claim.IsHRProcessed = true;
                claim.HRProcessedDate = DateTime.Now;
                claim.HRPersonnelName = user.Email;

                _context.Update(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Invoice {claim.InvoiceNumber} generated successfully!";
                return RedirectToAction(nameof(ViewInvoice), new { id = claim.Id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating invoice: {ex.Message}";
                return RedirectToAction(nameof(ProcessClaim), new { id });
            }
        }

        // GET: View generated invoice
        public async Task<IActionResult> ViewInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.LecturerClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            if (!claim.IsHRProcessed || string.IsNullOrEmpty(claim.InvoiceNumber))
            {
                TempData["ErrorMessage"] = "Invoice has not been generated for this claim yet.";
                return RedirectToAction(nameof(ProcessClaim), new { id });
            }

            // Calculate tax and net amount for display
            var taxRate = 0.15m; // 15% tax
            var taxAmount = claim.TotalAmount * taxRate;
            var netAmount = claim.TotalAmount - taxAmount;

            ViewBag.TaxAmount = taxAmount;
            ViewBag.NetAmount = netAmount;
            ViewBag.TaxRate = taxRate * 100; // Display as percentage

            return View(claim);
        }

        // GET: View all processed claims (invoices generated)
        public async Task<IActionResult> ProcessedClaims()
        {
            var processedClaims = await _context.LecturerClaims
                .Where(c => c.IsHRProcessed)
                .OrderByDescending(c => c.HRProcessedDate)
                .ToListAsync();

            return View(processedClaims);
        }

        // GET: Generate Report Summary
        public async Task<IActionResult> Reports()
        {
            var totalClaims = await _context.LecturerClaims.CountAsync();
            var approvedClaims = await _context.LecturerClaims.CountAsync(c => c.Status == "Approved");
            var processedClaims = await _context.LecturerClaims.CountAsync(c => c.IsHRProcessed);
            var pendingClaims = await _context.LecturerClaims.CountAsync(c => c.Status == "Pending");
            var rejectedClaims = await _context.LecturerClaims.CountAsync(c => c.Status == "Rejected");

            var totalApprovedAmount = await _context.LecturerClaims
                .Where(c => c.Status == "Approved")
                .SumAsync(c => c.TotalAmount);

            var totalProcessedAmount = await _context.LecturerClaims
                .Where(c => c.IsHRProcessed)
                .SumAsync(c => c.TotalAmount);

            ViewBag.TotalClaims = totalClaims;
            ViewBag.ApprovedClaims = approvedClaims;
            ViewBag.ProcessedClaims = processedClaims;
            ViewBag.PendingClaims = pendingClaims;
            ViewBag.RejectedClaims = rejectedClaims;
            ViewBag.TotalApprovedAmount = totalApprovedAmount;
            ViewBag.TotalProcessedAmount = totalProcessedAmount;

            // Calculate tax summary
            var taxRate = 0.15m;
            ViewBag.TotalTaxAmount = totalProcessedAmount * taxRate;
            ViewBag.TotalNetAmount = totalProcessedAmount - (totalProcessedAmount * taxRate);

            return View();
        }

        // POST: Bulk generate invoices for all approved claims
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkGenerateInvoices()
        {
            try
            {
                var approvedClaims = await _context.LecturerClaims
                    .Where(c => c.Status == "Approved" && !c.IsHRProcessed)
                    .ToListAsync();

                var user = await _userManager.GetUserAsync(User);
                int successCount = 0;

                foreach (var claim in approvedClaims)
                {
                    try
                    {
                        // Generate invoice number
                        claim.GenerateInvoiceNumber();

                        // Mark as processed
                        claim.IsHRProcessed = true;
                        claim.HRProcessedDate = DateTime.Now;
                        claim.HRPersonnelName = user.Email;

                        _context.Update(claim);
                        successCount++;
                    }
                    catch
                    {
                        continue;
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Successfully generated {successCount} invoices!";
                return RedirectToAction(nameof(ProcessedClaims));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error generating invoices: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}