using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.ViewModels
{
    public class SubmitClaimViewModel
    {
        [Required]
        [Display(Name = "Hours Worked")]
        [Range(0.01, 1000, ErrorMessage = "Hours must be greater than 0")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Display(Name = "Hourly Rate (R)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hourly rate must be greater than 0")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Additional Notes")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? AdditionalNotes { get; set; }

        [Display(Name = "Supporting Document")]
        public IFormFile? SupportingDocument { get; set; }
    }
}
