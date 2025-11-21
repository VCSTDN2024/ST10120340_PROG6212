using ContractMonthlyClaimSystem.Models;
using NUnit.Framework;

namespace ContractMonthlyClaimSystem.Tests
{
    [TestFixture]
    public class LecturerClaimTests
    {
        [Test]
        public void CalculateTotalAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 100,
                HourlyRate = 500
            };

            // Act
            claim.CalculateTotalAmount();

            // Assert
            Assert.That(claim.TotalAmount, Is.EqualTo(50000));
        }

        [TestCase(100, 500, 50000)]
        [TestCase(50, 350, 17500)]
        [TestCase(160, 450, 72000)]
        public void CalculateTotalAmount_WithMultipleInputs_ShouldCalculateCorrectly(
            decimal hours, decimal rate, decimal expected)
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = hours,
                HourlyRate = rate
            };

            // Act
            claim.CalculateTotalAmount();

            // Assert
            Assert.That(claim.TotalAmount, Is.EqualTo(expected));
        }

        [Test]
        public void ValidateClaimRules_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 100,
                HourlyRate = 500,
                TotalAmount = 50000
            };

            // Act
            var result = claim.ValidateClaimRules(out string message);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(message, Is.Empty);
        }

        [Test]
        public void ValidateClaimRules_WithExcessiveHours_ShouldReturnFalse()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 1000, // Exceeds 744 limit
                HourlyRate = 500,
                TotalAmount = 500000
            };

            // Act
            var result = claim.ValidateClaimRules(out string message);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(message, Does.Contain("744"));
        }

        [Test]
        public void ValidateClaimRules_WithLowHourlyRate_ShouldReturnFalse()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 100,
                HourlyRate = 10, // Below R50 minimum
                TotalAmount = 1000
            };

            // Act
            var result = claim.ValidateClaimRules(out string message);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(message, Does.Contain("50"));
        }

        [Test]
        public void ValidateClaimRules_WithExcessiveAmount_ShouldReturnFalse()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 100,
                HourlyRate = 6000,
                TotalAmount = 600000 // Exceeds R500,000 threshold
            };

            // Act
            var result = claim.ValidateClaimRules(out string message);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(message, Does.Contain("500000"));
        }

        [Test]
        public void GenerateInvoiceNumber_ShouldCreateUniqueInvoiceNumber()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                Id = 123
            };

            // Act
            claim.GenerateInvoiceNumber();

            // Assert
            Assert.That(claim.InvoiceNumber, Is.Not.Null);
            Assert.That(claim.InvoiceNumber, Does.StartWith("INV-123-"));
            Assert.That(claim.InvoiceGeneratedDate, Is.Not.Null);
        }

        [Test]
        public void GetAutomatedValidationResult_WithValidClaim_ShouldReturnValid()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                Id = 1,
                HoursWorked = 100,
                HourlyRate = 500,
                TotalAmount = 50000,
                DocumentPath = "/uploads/test.pdf",
                DateSubmitted = DateTime.Now
            };

            // Act
            var result = claim.GetAutomatedValidationResult();

            // Assert
            Assert.That(result.IsValid, Is.True);
            Assert.That(result.OverallRecommendation, Does.Contain("All automated checks passed"));
            Assert.That(result.ValidationMessages, Is.Not.Empty);
        }

        [Test]
        public void GetAutomatedValidationResult_WithInvalidClaim_ShouldReturnInvalid()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                Id = 1,
                HoursWorked = 1000, // Invalid
                HourlyRate = 10, // Invalid
                TotalAmount = 10000,
                DateSubmitted = DateTime.Now
            };

            // Act
            var result = claim.GetAutomatedValidationResult();

            // Assert
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.OverallRecommendation, Does.Contain("validation checks failed"));
            Assert.That(result.ValidationMessages, Is.Not.Empty);
        }

        [Test]
        public void CalculateTotalAmount_WithZeroValues_ShouldReturnZero()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 0,
                HourlyRate = 0
            };

            // Act
            claim.CalculateTotalAmount();

            // Assert
            Assert.That(claim.TotalAmount, Is.EqualTo(0));
        }

        [Test]
        public void ValidateClaimRules_WithNegativeHours_ShouldReturnFalse()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = -10,
                HourlyRate = 500,
                TotalAmount = -5000
            };

            // Act
            var result = claim.ValidateClaimRules(out string message);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(message, Is.Not.Empty);
        }

        [Test]
        public void ValidateClaimRules_WithValidMinimumValues_ShouldReturnTrue()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 0.01m,
                HourlyRate = 50,
                TotalAmount = 0.50m,
                DocumentPath = "/uploads/test.pdf"
            };

            // Act
            var result = claim.ValidateClaimRules(out string message);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateClaimRules_WithValidMaximumValues_ShouldReturnTrue()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                HoursWorked = 744,
                HourlyRate = 672.04m, // Max rate that keeps total under 500000
                TotalAmount = 499998.56m,
                DocumentPath = "/uploads/test.pdf"
            };

            // Act
            var result = claim.ValidateClaimRules(out string message);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}