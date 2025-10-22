using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.ViewModels
{
    public class ReviewClaimViewModel
    {
        public int ClaimId { get; set; }
        public string LecturerName { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? AdditionalNotes { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string Status { get; set; }
        public string? DocumentPath { get; set; }

        [StringLength(500)]
        [Display(Name = "Review Comments")]
        public string? ReviewComments { get; set; }
    }
}

