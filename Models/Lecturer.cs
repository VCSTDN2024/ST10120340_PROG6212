using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }

        [Required]
        [Range(50, 10000)]
        [DataType(DataType.Currency)]
        public decimal DefaultHourlyRate { get; set; }

        [StringLength(50)]
        public string? BankName { get; set; }

        [StringLength(50)]
        public string? AccountNumber { get; set; }

        [StringLength(20)]
        public string? BranchCode { get; set; }

        [StringLength(50)]
        public string? TaxNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? TerminationDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}


