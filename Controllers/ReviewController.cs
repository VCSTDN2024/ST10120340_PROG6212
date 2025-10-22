using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.ViewModels;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize(Roles = "Programme Coordinator,Academic Manager")]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Review Dashboard - List all claims
        public async Task<IActionResult> Index(string filter = "All")
        {
            var query = _context.LecturerClaims.AsQueryable();

            // Filter by status
            if (filter != "All")
            {
                query = query.Where(c => c.Status == filter);
            }

            var claims = await query
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            ViewBag.Filter = filter;
            return View(claims);
        }

        // GET: Review specific claim
        public async Task<IActionResult> ReviewClaim(int id)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            var viewModel = new ReviewClaimViewModel
            {
                ClaimId = claim.Id,
                LecturerName = claim.LecturerName,
                HoursWorked = claim.HoursWorked,
                HourlyRate = claim.HourlyRate,
                TotalAmount = claim.TotalAmount,
                AdditionalNotes = claim.AdditionalNotes,
                DateSubmitted = claim.DateSubmitted,
                Status = claim.Status,
                DocumentPath = claim.DocumentPath
            };

            return View(viewModel);
        }

        // POST: Approve Claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveClaim(int id, string reviewComments)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            claim.Status = "Approved";
            claim.ReviewedBy = user.Email;
            claim.ReviewDate = DateTime.Now;
            claim.ReviewComments = reviewComments;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim approved successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Reject Claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectClaim(int id, string reviewComments)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(reviewComments))
            {
                TempData["ErrorMessage"] = "Please provide a reason for rejection.";
                return RedirectToAction(nameof(ReviewClaim), new { id });
            }

            var user = await _userManager.GetUserAsync(User);

            claim.Status = "Rejected";
            claim.ReviewedBy = user.Email;
            claim.ReviewDate = DateTime.Now;
            claim.ReviewComments = reviewComments;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Claim rejected.";
            return RedirectToAction(nameof(Index));
        }
    }
}