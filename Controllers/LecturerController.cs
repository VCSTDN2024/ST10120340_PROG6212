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

        // POST: Submit Claim (ENHANCED WITH PART 3 AUTOMATION)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    string? documentPath = null;
                    long? fileSize = null;
                    string? fileType = null;

                    // ===== PART 3: ENHANCED FILE UPLOAD WITH VALIDATION =====
                    if (model.SupportingDocument != null && model.SupportingDocument.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".jpeg", ".png" };
                        var fileExtension = Path.GetExtension(model.SupportingDocument.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("SupportingDocument", "Invalid file type. Allowed types: PDF, DOCX, XLSX, JPG, PNG");
                            return View(model);
                        }

                        // Validate file size (max 5MB)
                        if (model.SupportingDocument.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("SupportingDocument", "File size must not exceed 5MB");
                            return View(model);
                        }

                        // Save file
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        Directory.CreateDirectory(uploadsFolder);

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.SupportingDocument.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.SupportingDocument.CopyToAsync(fileStream);
                        }

                        documentPath = "/uploads/" + uniqueFileName;
                        fileSize = model.SupportingDocument.Length;
                        fileType = fileExtension;
                    }

                    // Create claim object
                    var claim = new LecturerClaim
                    {
                        LecturerId = user.Id,
                        LecturerName = user.Email,
                        HoursWorked = model.HoursWorked,
                        HourlyRate = model.HourlyRate,
                        AdditionalNotes = model.AdditionalNotes,
                        DateSubmitted = DateTime.Now,
                        Status = "Pending",
                        DocumentPath = documentPath,
                        DocumentFileSize = fileSize,
                        DocumentFileType = fileType
                    };

                    // ===== PART 3: AUTO-CALCULATION =====
                    claim.CalculateTotalAmount();

                    // ===== PART 3: BUSINESS RULE VALIDATION =====
                    if (!claim.ValidateClaimRules(out string validationMessage))
                    {
                        ModelState.AddModelError("", validationMessage);
                        return View(model);
                    }

                    _context.LecturerClaims.Add(claim);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Claim submitted successfully! Total Amount: R{claim.TotalAmount:N2}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error submitting claim: {ex.Message}");
                }
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

        // ===== PART 3: AJAX ENDPOINT FOR AUTO-CALCULATION =====
        [HttpPost]
        public JsonResult CalculateTotalAmount(decimal hoursWorked, decimal hourlyRate)
        {
            try
            {
                var totalAmount = hoursWorked * hourlyRate;

                // Validate against business rules
                var claim = new LecturerClaim
                {
                    HoursWorked = hoursWorked,
                    HourlyRate = hourlyRate,
                    TotalAmount = totalAmount
                };

                bool isValid = claim.ValidateClaimRules(out string message);

                return Json(new
                {
                    success = true,
                    totalAmount = totalAmount,
                    isValid = isValid,
                    message = message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // ===== PART 3: TRACK CLAIM STATUS =====
        public async Task<IActionResult> TrackStatus()
        {
            var userId = _userManager.GetUserId(User);
            var claims = await _context.LecturerClaims
                .Where(c => c.LecturerId == userId)
                .OrderByDescending(c => c.DateSubmitted)
                .ToListAsync();

            return View(claims);
        }
    }
}