using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
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

            // ===== PART 3: STATISTICS FOR DASHBOARD =====
            ViewBag.PendingCount = await _context.LecturerClaims.CountAsync(c => c.Status == "Pending");
            ViewBag.ApprovedCount = await _context.LecturerClaims.CountAsync(c => c.Status == "Approved");
            ViewBag.RejectedCount = await _context.LecturerClaims.CountAsync(c => c.Status == "Rejected");

            ViewBag.Filter = filter;
            return View(claims);
        }

        // GET: Review specific claim (ENHANCED WITH PART 3 AUTOMATED VALIDATION)
        public async Task<IActionResult> ReviewClaim(int id)
        {
            var claim = await _context.LecturerClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            // ===== PART 3: PERFORM AUTOMATED VALIDATION =====
            var validationResult = claim.GetAutomatedValidationResult();
            ViewBag.ValidationResult = validationResult;

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

            // ===== PART 3: WORKFLOW STATUS =====
            ViewBag.IsCoordinatorApproved = claim.IsCoordinatorApproved;
            ViewBag.IsManagerApproved = claim.IsManagerApproved;
            ViewBag.CoordinatorName = claim.CoordinatorName;
            ViewBag.ManagerName = claim.ManagerName;
            ViewBag.CoordinatorApprovedDate = claim.CoordinatorApprovedDate;
            ViewBag.ManagerApprovedDate = claim.ManagerApprovedDate;

            return View(viewModel);
        }

        // POST: Approve Claim (ENHANCED WITH PART 3 WORKFLOW)
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
            var userRoles = await _userManager.GetRolesAsync(user);

            // ===== PART 3: SEQUENTIAL APPROVAL WORKFLOW =====

            // Determine if user is Coordinator or Manager
            bool isCoordinator = userRoles.Contains("Programme Coordinator");
            bool isManager = userRoles.Contains("Academic Manager");

            if (isCoordinator)
            {
                // Coordinator approval (first stage)
                claim.IsCoordinatorApproved = true;
                claim.CoordinatorApprovedDate = DateTime.Now;
                claim.CoordinatorName = user.Email;
                claim.Status = "Coordinator Approved";
                claim.ReviewedBy = user.Email;
                claim.ReviewDate = DateTime.Now;
                claim.ReviewComments = reviewComments;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim approved by Coordinator. Awaiting Manager approval.";
            }
            else if (isManager)
            {
                // ===== PART 3: WORKFLOW ENFORCEMENT - Manager cannot approve without Coordinator =====
                if (!claim.IsCoordinatorApproved)
                {
                    TempData["ErrorMessage"] = "This claim must be approved by Programme Coordinator first.";
                    return RedirectToAction(nameof(ReviewClaim), new { id });
                }

                // Manager approval (final stage)
                claim.IsManagerApproved = true;
                claim.ManagerApprovedDate = DateTime.Now;
                claim.ManagerName = user.Email;
                claim.Status = "Approved";
                claim.ReviewedBy = user.Email;
                claim.ReviewDate = DateTime.Now;

                // Append manager comments to existing comments
                if (!string.IsNullOrEmpty(claim.ReviewComments))
                {
                    claim.ReviewComments += $"\n\nManager Comments: {reviewComments}";
                }
                else
                {
                    claim.ReviewComments = $"Manager Comments: {reviewComments}";
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim fully approved. Ready for HR processing.";
            }

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

        // ===== PART 3: AJAX ENDPOINT FOR QUICK VALIDATION =====
        [HttpPost]
        public async Task<JsonResult> QuickValidate(int id)
        {
            try
            {
                var claim = await _context.LecturerClaims.FindAsync(id);

                if (claim == null)
                {
                    return Json(new { success = false, message = "Claim not found" });
                }

                var validationResult = claim.GetAutomatedValidationResult();

                return Json(new
                {
                    success = true,
                    isValid = validationResult.IsValid,
                    messages = validationResult.ValidationMessages,
                    recommendation = validationResult.OverallRecommendation
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ===== PART 3: VIEW APPROVED CLAIMS =====
        public async Task<IActionResult> ApprovedClaims()
        {
            var approvedClaims = await _context.LecturerClaims
                .Where(c => c.Status == "Approved" || c.Status == "Coordinator Approved")
                .OrderByDescending(c => c.ReviewDate)
                .ToListAsync();

            return View(approvedClaims);
        }

        // ===== PART 3: VIEW REJECTED CLAIMS =====
        public async Task<IActionResult> RejectedClaims()
        {
            var rejectedClaims = await _context.LecturerClaims
                .Where(c => c.Status == "Rejected")
                .OrderByDescending(c => c.ReviewDate)
                .ToListAsync();

            return View(rejectedClaims);
        }
    }
}