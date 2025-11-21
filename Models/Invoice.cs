using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMonthlyClaimSystem.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }

        [Required]
        public int ClaimId { get; set; }

        [ForeignKey("ClaimId")]
        public LecturerClaim? LecturerClaim { get; set; }

        [Required]
        [StringLength(100)]
        public string LecturerName { get; set; }

        [Required]
        [EmailAddress]
        public string LecturerEmail { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal NetAmount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Unpaid";

        [DataType(DataType.DateTime)]
        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        public string? PaymentReference { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(100)]
        public string GeneratedBy { get; set; }

        // AUTOMATION METHOD: Calculate tax and net amount
        public void CalculateAmounts(decimal taxRate = 0.15m)
        {
            TaxAmount = TotalAmount * taxRate;
            NetAmount = TotalAmount - TaxAmount;
        }

        // AUTOMATION METHOD: Set due date (30 days from invoice date)
        public void SetDueDate(int daysUntilDue = 30)
        {
            DueDate = InvoiceDate.AddDays(daysUntilDue);
        }
    }
}
