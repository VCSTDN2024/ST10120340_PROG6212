using ContractMonthlyClaimSystem.Controllers;
using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using System;                   
using System.Collections.Generic; 
using System.Linq;                
using System.Threading.Tasks;   

namespace ContractMonthlyClaimSystem.Tests
{
    [TestFixture]
    public class ReviewControllerTests
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private ApplicationDbContext _context;
        private ReviewController _controller;

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

            // Create controller
            _controller = new ReviewController(_context, _mockUserManager.Object);

            // Setup user context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(ClaimTypes.NameIdentifier, "coordinator-id"),
                new System.Security.Claims.Claim(ClaimTypes.Name, "coordinator@example.com"),
                new System.Security.Claims.Claim(ClaimTypes.Role, "Programme Coordinator")
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
            _context.LecturerClaims.Add(new LecturerClaim
            {
                LecturerId = "lecturer-id",
                LecturerName = "Lecturer",
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
        public async Task ReviewClaim_WithValidId_ReturnsViewWithValidation()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                LecturerId = "lecturer-id",
                LecturerName = "Lecturer",
                HoursWorked = 100,
                HourlyRate = 500,
                TotalAmount = 50000,
                DateSubmitted = DateTime.Now,
                Status = "Pending",
                DocumentPath = "/test.pdf"
            };
            _context.LecturerClaims.Add(claim);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ReviewClaim(claim.Id);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = result as ViewResult;
            Assert.That(viewResult.ViewData["ValidationResult"], Is.Not.Null);
        }

        [Test]
        public async Task QuickValidate_ReturnsValidationResult()
        {
            // Arrange
            var claim = new LecturerClaim
            {
                LecturerId = "lecturer-id",
                LecturerName = "Lecturer",
                HoursWorked = 100,
                HourlyRate = 500,
                TotalAmount = 50000,
                DateSubmitted = DateTime.Now,
                Status = "Pending",
                DocumentPath = "/test.pdf"
            };
            _context.LecturerClaims.Add(claim);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.QuickValidate(claim.Id);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = result as JsonResult;
            dynamic data = jsonResult.Value;
            Assert.That(data.success, Is.True);
            Assert.That(data.isValid, Is.True);
        }

        [Test]
        public async Task Index_WithStatusFilter_ReturnsFilteredClaims()
        {
            // Arrange
            _context.LecturerClaims.AddRange(
                new LecturerClaim
                {
                    LecturerId = "lecturer-id",
                    LecturerName = "Lecturer 1",
                    HoursWorked = 100,
                    HourlyRate = 500,
                    TotalAmount = 50000,
                    DateSubmitted = DateTime.Now,
                    Status = "Pending"
                },
                new LecturerClaim
                {
                    LecturerId = "lecturer-id-2",
                    LecturerName = "Lecturer 2",
                    HoursWorked = 50,
                    HourlyRate = 300,
                    TotalAmount = 15000,
                    DateSubmitted = DateTime.Now,
                    Status = "Approved"
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index("Pending");

            // Assert
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<LecturerClaim>;
            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].Status, Is.EqualTo("Pending"));
        }
    }
}