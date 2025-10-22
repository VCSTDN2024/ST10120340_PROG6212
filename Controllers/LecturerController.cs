using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.ViewModels;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LecturerController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Lecturer Dashboard
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var claims = await _context.LecturerClaims
                .Where(c => c.LecturerId == userId)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(claims);
        }

        // GET: Submit Claim
        public IActionResult SubmitClaim()
        {
            return View();
        }

        // POST: Submit Claim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                string? documentPath = null;

                // Handle file upload
                if (model.SupportingDocument != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SupportingDocument.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.SupportingDocument.CopyToAsync(fileStream);
                    }

                    documentPath = "/uploads/" + uniqueFileName;
                }

                var claim = new LecturerClaim
                {
                    LecturerId = user.Id,
                    LecturerName = user.Email,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    TotalAmount = model.HoursWorked * model.HourlyRate,
                    AdditionalNotes = model.AdditionalNotes,
                    DateSubmitted = DateTime.Now,
                    Status = "Pending",
                    DocumentPath = documentPath
                };

                _context.LecturerClaims.Add(claim);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: View Claim Details
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);
            var claim = await _context.LecturerClaims
                .FirstOrDefaultAsync(c => c.Id == id && c.LecturerId == userId);

            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }
    }
}