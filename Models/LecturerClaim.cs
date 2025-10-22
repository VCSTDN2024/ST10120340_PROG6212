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
        [Range(0.01, 1000, ErrorMessage = "Hours must be greater than 0")]
        public decimal HoursWorked { get; set; }

        [Required]
        [Display(Name = "Hourly Rate (R)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hourly rate must be greater than 0")]
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
    }
}
