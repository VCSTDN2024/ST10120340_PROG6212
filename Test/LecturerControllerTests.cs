using ContractMonthlyClaimSystem.Controllers;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;                           // ← ADD THIS
using System.Collections.Generic;      // ← ADD THIS
using System.Linq;                     // ← ADD THIS
using System.Security.Claims;
using System.Threading.Tasks;          // ← ADD THIS

namespace ContractMonthlyClaimSystem.Tests
{
    [TestFixture]
    public class LecturerControllerTests
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private ApplicationDbContext _context;
        private LecturerController _controller;

        [SetUp]
        public void Setup()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Mock UserManager
            var store = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Mock environment
            var mockEnvironment = new Mock<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("C:\\test");

            // Create controller
            _controller = new LecturerController(_context, _mockUserManager.Object, mockEnvironment.Object);

            // Setup user context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                new System.Security.Claims.Claim(ClaimTypes.Name, "test@example.com")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithClaims()
        {
            // Arrange
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("test-user-id");

            _context.LecturerClaims.Add(new LecturerClaim
            {
                LecturerId = "test-user-id",
                LecturerName = "Test User",
                HoursWorked = 100,
                HourlyRate = 500,
                TotalAmount = 50000,
                DateSubmitted = DateTime.Now,
                Status = "Pending"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.Model, Is.InstanceOf<List<LecturerClaim>>());
            var model = viewResult.Model as List<LecturerClaim>;
            Assert.That(model.Count, Is.EqualTo(1));
        }

        [Test]
        public void SubmitClaim_ReturnsView()
        {
            // Act
            var result = _controller.SubmitClaim();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void CalculateTotalAmount_ReturnsCorrectJson()
        {
            // Arrange
            decimal hours = 100;
            decimal rate = 500;

            // Act
            var result = _controller.CalculateTotalAmount(hours, rate);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            dynamic data = jsonResult.Value;
            Assert.That(data.success, Is.True);
            Assert.That(data.totalAmount, Is.EqualTo(50000));
        }

        [Test]
        public void CalculateTotalAmount_WithInvalidData_ReturnsInvalid()
        {
            // Arrange
            decimal hours = 1000; // Exceeds limit
            decimal rate = 500;

            // Act
            var result = _controller.CalculateTotalAmount(hours, rate);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            dynamic data = jsonResult.Value;
            Assert.That(data.success, Is.True);
            Assert.That(data.isValid, Is.False);
        }

        [Test]
        public async Task Index_WithMultipleClaims_ReturnsAllClaims()
        {
            // Arrange
            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("test-user-id");

            _context.LecturerClaims.AddRange(
                new LecturerClaim
                {
                    LecturerId = "test-user-id",
                    LecturerName = "Test User",
                    HoursWorked = 100,
                    HourlyRate = 500,
                    TotalAmount = 50000,
                    DateSubmitted = DateTime.Now,
                    Status = "Pending"
                },
                new LecturerClaim
                {
                    LecturerId = "test-user-id",
                    LecturerName = "Test User",
                    HoursWorked = 50,
                    HourlyRate = 300,
                    TotalAmount = 15000,
                    DateSubmitted = DateTime.Now,
                    Status = "Approved"
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<LecturerClaim>;
            Assert.That(model.Count, Is.EqualTo(2));
        }
    }
}