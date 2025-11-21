using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class LecturerClaim
    {
        public int Id { get; set; }

        [Required]
        public string LecturerId { get; set; }

        [Required]
        public string LecturerName { get; set; }

        [Required]
        [Display(Name = "Hours Worked")]
        [Range(0.01, 744, ErrorMessage = "Hours must be between 0.01 and 744 (monthly maximum)")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Display(Name = "Hourly Rate (R)")]
        [Range(50, 10000, ErrorMessage = "Hourly rate must be between R50 and R10,000")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }  // HoursWorked * HourlyRate

        [Display(Name = "Additional Notes")]
        [StringLength(500)]
        public string? AdditionalNotes { get; set; }

        [Display(Name = "Date Submitted")]
        public DateTime DateSubmitted { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }  // "Pending", "Approved", "Rejected"

        [Display(Name = "Reviewed By")]
        public string? ReviewedBy { get; set; }

        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }

        [Display(Name = "Review Comments")]
        [StringLength(500)]
        public string? ReviewComments { get; set; }

        [Display(Name = "Supporting Document")]
        public string? DocumentPath { get; set; }

        // ===== PART 3 AUTOMATION ADDITIONS =====

        // Approval workflow tracking
        public bool IsCoordinatorApproved { get; set; } = false;
        public bool IsManagerApproved { get; set; } = false;
        public bool IsHRProcessed { get; set; } = false;

        public DateTime? CoordinatorApprovedDate { get; set; }
        public DateTime? ManagerApprovedDate { get; set; }
        public DateTime? HRProcessedDate { get; set; }

        [StringLength(100)]
        public string? CoordinatorName { get; set; }

        [StringLength(100)]
        public string? ManagerName { get; set; }

        [StringLength(100)]
        public string? HRPersonnelName { get; set; }

        // Invoice generation fields
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }

        public DateTime? InvoiceGeneratedDate { get; set; }

        public long? DocumentFileSize { get; set; }

        [StringLength(50)]
        public string? DocumentFileType { get; set; }

        // AUTOMATION METHOD 1: Calculate Total Amount
        public void CalculateTotalAmount()
        {
            TotalAmount = HoursWorked * HourlyRate;
        }

        // AUTOMATION METHOD 2: Validate Claim Against Business Rules
        public bool ValidateClaimRules(out string validationMessage)
        {
            validationMessage = string.Empty;

            // Rule 1: Hours worked should not exceed maximum monthly hours (744 hours = 31 days × 24 hours)
            if (HoursWorked > 744)
            {
                validationMessage = "Hours worked cannot exceed 744 hours per month.";
                return false;
            }

            if (HoursWorked < 0.01m)
            {
                validationMessage = "Hours worked must be greater than 0.";
                return false;
            }

            // Rule 2: Hourly rate should be within acceptable range
            if (HourlyRate < 50 || HourlyRate > 10000)
            {
                validationMessage = "Hourly rate must be between R50 and R10,000.";
                return false;
            }

            // Rule 3: Final payment should not exceed maximum threshold
            if (TotalAmount > 500000)
            {
                validationMessage = "Total amount exceeds maximum threshold of R500,000.";
                return false;
            }

            // Rule 4: Supporting document should be present (optional for now)
            // This can be enforced based on your requirements
            if (string.IsNullOrEmpty(DocumentPath))
            {
                validationMessage = "Warning: Supporting document is recommended.";
                // Don't return false - just a warning
            }

            return true;
        }

        // AUTOMATION METHOD 3: Generate Invoice Number
        public void GenerateInvoiceNumber()
        {
            InvoiceNumber = $"INV-{Id}-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
            InvoiceGeneratedDate = DateTime.Now;
        }

        // AUTOMATION METHOD 4: Get validation result for display
        public ValidationResult GetAutomatedValidationResult()
        {
            var result = new ValidationResult
            {
                ClaimId = Id,
                IsValid = true,
                ValidationMessages = new List<string>()
            };

            // Check 1: Verify hours worked is within acceptable range
            if (HoursWorked >= 0.01m && HoursWorked <= 744)
            {
                result.ValidationMessages.Add("✓ Hours worked is within acceptable range (0.01-744 hours)");
            }
            else
            {
                result.IsValid = false;
                result.ValidationMessages.Add("⚠️ Hours worked is outside acceptable range");
            }

            // Check 2: Verify hourly rate is within policy limits
            if (HourlyRate >= 50 && HourlyRate <= 10000)
            {
                result.ValidationMessages.Add("✓ Hourly rate is within policy limits (R50-R10,000)");
            }
            else
            {
                result.IsValid = false;
                result.ValidationMessages.Add("⚠️ Hourly rate is outside policy limits");
            }

            // Check 3: Verify total amount calculation is correct
            var expectedAmount = HoursWorked * HourlyRate;
            if (Math.Abs(TotalAmount - expectedAmount) < 0.01m)
            {
                result.ValidationMessages.Add($"✓ Total amount calculation is correct (R{TotalAmount:N2})");
            }
            else
            {
                result.IsValid = false;
                result.ValidationMessages.Add($"⚠️ Total amount calculation error. Expected: R{expectedAmount:N2}, Got: R{TotalAmount:N2}");
            }

            // Check 4: Verify supporting document is present
            if (!string.IsNullOrEmpty(DocumentPath))
            {
                result.ValidationMessages.Add("✓ Supporting document is attached");
            }
            else
            {
                result.ValidationMessages.Add("⚠️ Supporting document is missing");
            }

            // Check 5: Verify total amount doesn't exceed threshold
            if (TotalAmount <= 500000)
            {
                result.ValidationMessages.Add("✓ Total amount is within threshold (≤R500,000)");
            }
            else
            {
                result.IsValid = false;
                result.ValidationMessages.Add("⚠️ Total amount exceeds maximum threshold of R500,000");
            }

            // Check 6: Check submission date validity
            if (DateSubmitted <= DateTime.Now)
            {
                result.ValidationMessages.Add("✓ Submission date is valid");
            }
            else
            {
                result.IsValid = false;
                result.ValidationMessages.Add("⚠️ Invalid submission date (future date)");
            }

            result.OverallRecommendation = result.IsValid
                ? "✅ All automated checks passed. Claim is recommended for approval."
                : "❌ Some validation checks failed. Review required before approval.";

            return result;
        }
    }

    // Helper class for validation results
    public class ValidationResult
    {
        public int ClaimId { get; set; }
        public bool IsValid { get; set; }
        public List<string> ValidationMessages { get; set; } = new List<string>();
        public string OverallRecommendation { get; set; } = string.Empty;
    }
}