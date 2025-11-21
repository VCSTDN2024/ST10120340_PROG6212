# Contract Monthly Claim System

A comprehensive web-based system for managing monthly claims submitted by independent contractor lecturers at an educational institution.

## 📋 Project Information

**Student:** [Your Name]  
**Student Number:** [Your Student Number]  
**Module:** PROG6212 - Programming 2B  
**Institution:** [Your Institution]  
**Submission Date:** November 21, 2025  
**POE Part:** Part 3 - Automation & Testing

---

## 🤖 AI Usage Declaration

**IMPORTANT DECLARATION:**

This project was developed with assistance from Artificial Intelligence (Claude AI by Anthropic) for the following purposes:

### AI-Assisted Areas:
1. **Code Optimization:** AI was used to help optimize code structure, improve performance, and follow best practices
2. **Problem-Solving Strategies:** AI provided guidance on architectural decisions and implementation approaches
3. **Debugging Assistance:** AI helped identify and resolve technical issues during development
4. **Code Review:** AI provided suggestions for code improvements and security enhancements

### Student Contribution:
- **Core Logic & Implementation:** All business logic was designed and implemented by the student
- **Database Design:** Database schema and relationships were designed by the student
- **UI/UX Design:** User interface and user experience decisions were made by the student
- **Integration:** All components were integrated and tested by the student
- **Customization:** All code was reviewed, understood, and customized to meet project requirements

### Learning Outcomes:
The use of AI as a learning tool enhanced the development process by:
- Accelerating problem resolution and debugging
- Providing exposure to industry best practices
- Enabling focus on understanding core concepts rather than syntax errors
- Facilitating learning of advanced programming patterns

**The student takes full responsibility for all code submitted and confirms understanding of all implemented functionality.**

---

## 📖 Table of Contents

1. [System Overview](#system-overview)
2. [Features](#features)
3. [Technology Stack](#technology-stack)
4. [System Requirements](#system-requirements)
5. [Installation Guide](#installation-guide)
6. [User Roles](#user-roles)
7. [Part 3 Automation Features](#part-3-automation-features)
8. [Testing](#testing)
9. [Project Structure](#project-structure)
10. [Database Schema](#database-schema)
11. [Usage Guide](#usage-guide)
12. [Known Issues](#known-issues)
13. [Future Enhancements](#future-enhancements)
14. [License](#license)

---

## 🎯 System Overview

The Contract Monthly Claim System is an ASP.NET Core MVC application designed to streamline the process of submitting, reviewing, and approving monthly claims for independent contractor lecturers.

### Workflow:
1. **Lecturer** submits monthly claim with hours worked and hourly rate
2. **Programme Coordinator** reviews and validates claim
3. **Academic Manager** provides final approval
4. **HR Department** generates invoices and processes payments

---

## ✨ Features

### Core Functionality (Parts 1 & 2)
- ✅ User authentication and authorization (ASP.NET Identity)
- ✅ Role-based access control (Lecturer, Coordinator, Manager, HR)
- ✅ Claim submission with file upload support
- ✅ Multi-stage approval workflow
- ✅ Claim status tracking
- ✅ Document management

### Part 3 Automation Features
- ✅ **Auto-Calculation:** Real-time calculation of claim amounts (Hours × Rate)
- ✅ **Live Validation:** Client-side validation with visual feedback badges
- ✅ **Automated Verification:** 6 automated business rule checks
- ✅ **Workflow Enforcement:** Sequential approval (Coordinator → Manager)
- ✅ **Invoice Generation:** Automatic invoice number generation
- ✅ **Comprehensive Testing:** Unit tests with NUnit framework

---

## 🛠 Technology Stack

### Backend
- **Framework:** ASP.NET Core MVC 8.0
- **Language:** C# 12
- **ORM:** Entity Framework Core 8.0.19
- **Database:** SQL Server (Development) / InMemory (Testing)
- **Authentication:** ASP.NET Core Identity

### Frontend
- **UI Framework:** Bootstrap 5.3
- **Icons:** Font Awesome 6.4
- **JavaScript:** jQuery 3.6
- **Styling:** Custom CSS

### Testing
- **Framework:** NUnit 3.14
- **Mocking:** Moq 4.18.4
- **Test Database:** EF Core InMemory Provider
- **Test Runner:** .NET CLI / Visual Studio Test Explorer

### Tools & IDEs
- **IDE:** Visual Studio 2022
- **Version Control:** Git
- **Package Manager:** NuGet

---

## 💻 System Requirements

### Development Environment
- **OS:** Windows 10/11, macOS, or Linux
- **IDE:** Visual Studio 2022 or Visual Studio Code
- **.NET SDK:** 8.0 or higher
- **SQL Server:** 2019 or higher (or SQL Server Express)
- **Browser:** Modern browser (Chrome, Edge, Firefox, Safari)

### Minimum Hardware
- **RAM:** 8GB (16GB recommended)
- **Storage:** 2GB free space
- **Processor:** Dual-core 2.0 GHz or higher

---

## 📥 Installation Guide

### Step 1: Clone Repository
```bash
git clone https://github.com/yourusername/ContractMonthlyClaimSystem.git
cd ContractMonthlyClaimSystem
```

### Step 2: Install Dependencies
```bash
dotnet restore
```

### Step 3: Update Database Connection

Edit `appsettings.json` and update the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ClaimSystemDb;Trusted_Connection=True;"
  }
}
```

### Step 4: Apply Database Migrations
```bash
dotnet ef database update
```

### Step 5: Run Application
```bash
dotnet run
```

Navigate to: `https://localhost:5001`

### Step 6: Register Users

1. Click **Register**
2. Create accounts with different roles:
   - Lecturer
   - Programme Coordinator
   - Academic Manager
   - HR

---

## 👥 User Roles

### 1. Lecturer
**Permissions:**
- Submit monthly claims
- Upload supporting documents
- View own claim history
- Track claim status

**Dashboard:**
- List of submitted claims
- Claim submission form
- Status indicators

### 2. Programme Coordinator
**Permissions:**
- Review pending claims
- View automated validation results
- Approve/reject claims (first stage)
- Add review comments

**Dashboard:**
- Pending claims list
- Validation dashboard
- Approval/rejection interface

### 3. Academic Manager
**Permissions:**
- Final approval of coordinator-approved claims
- View all validation results
- Override coordinator decisions (with justification)
- Access comprehensive reports

**Dashboard:**
- All claims overview
- Workflow status tracking
- Final approval interface

### 4. HR Personnel
**Permissions:**
- Generate invoices for approved claims
- Process payments
- View financial reports
- Access claim history

**Dashboard:**
- Approved claims awaiting processing
- Invoice generation tools
- Financial reports

---

## 🤖 Part 3 Automation Features

### 1. Lecturer View Automation (20 marks)

#### Auto-Calculation Feature
```javascript
// Real-time calculation as user types
Hours Worked: 100
Hourly Rate: R500
Total Amount: R50,000.00 ← Calculated automatically
```

**Implementation:**
- JavaScript event listeners on input fields
- Instant calculation on input change
- Formatted currency display
- Server-side validation backup

#### Visual Validation Badges
- ✅ **Green Badge:** Valid input (within acceptable range)
- ❌ **Red Badge:** Invalid input (outside acceptable range)
- ⚠️ **Yellow Badge:** Warning (recommended but not required)
- ⚪ **Gray Badge:** Not yet checked

**Validation Rules:**
1. Hours: 0.01 - 744 hours (monthly maximum)
2. Rate: R50 - R10,000 per hour
3. Total: Maximum R500,000
4. Document: File type and size validation

#### Smart Submit Button
- Automatically **disabled** when validation fails
- **Enabled** only when all checks pass
- Prevents invalid form submission

### 2. Coordinator/Manager Automation (20 marks)

#### Automated Validation Checks
The system performs 6 automated checks:

1. **✓ Hours Worked Validation**
   - Range: 0.01 - 744 hours
   - Status: Pass/Fail with message

2. **✓ Hourly Rate Validation**
   - Range: R50 - R10,000
   - Status: Pass/Fail with message

3. **✓ Total Amount Calculation**
   - Verifies: Hours × Rate = Total
   - Detects calculation errors

4. **✓ Supporting Document Check**
   - Verifies: Document uploaded
   - File type validation

5. **✓ Amount Threshold Check**
   - Maximum: R500,000
   - Flags excessive amounts

6. **✓ Submission Date Validation**
   - Checks: Valid date
   - Prevents future dates

#### Overall Recommendation
Based on automated checks, system provides:
- ✅ **"All checks passed - Recommended for approval"**
- ❌ **"Some checks failed - Review required"**

#### Workflow Enforcement
**Sequential Approval Process:**
```
Pending → Coordinator Review → Manager Review → Approved
```

**Business Rules:**
- ❌ Manager **cannot** approve without Coordinator approval
- ✅ System **enforces** sequential workflow
- 📋 Complete **audit trail** maintained

### 3. HR Automation (Bonus)

#### Invoice Generation
**Automated Invoice Features:**
- Unique invoice number: `INV-{ClaimId}-{Date}-{Time}`
- Automatic tax calculation (15%)
- Net amount computation
- Due date setting (30 days)

**Example:**
```
Invoice Number: INV-123-20251121-143052
Gross Amount: R50,000.00
Tax (15%): R7,500.00
Net Amount: R42,500.00
Due Date: December 21, 2025
```

#### Bulk Processing
- Generate multiple invoices simultaneously
- Batch approval functionality
- Export reports

---

## 🧪 Testing

### Unit Tests Overview
**Framework:** NUnit 3.14  
**Total Tests:** 12+  
**Coverage Areas:**
- Model validation logic
- Business rule enforcement
- Calculation accuracy
- Workflow compliance

### Test Categories

#### 1. Model Tests (LecturerClaimTests)
```csharp
✓ CalculateTotalAmount_ShouldCalculateCorrectly
✓ ValidateClaimRules_WithValidData_ShouldReturnTrue
✓ ValidateClaimRules_WithExcessiveHours_ShouldReturnFalse
✓ GenerateInvoiceNumber_ShouldCreateUniqueNumber
✓ GetAutomatedValidationResult_WithValidClaim_ShouldReturnValid
```

#### 2. Controller Tests (LecturerControllerTests)
```csharp
✓ Index_ReturnsViewWithClaims
✓ SubmitClaim_ReturnsView
✓ CalculateTotalAmount_ReturnsCorrectJson
✓ CalculateTotalAmount_WithInvalidData_ReturnsInvalid
```

#### 3. Review Controller Tests
```csharp
✓ ReviewClaim_WithValidId_ReturnsViewWithValidation
✓ QuickValidate_ReturnsValidationResult
✓ Index_WithStatusFilter_ReturnsFilteredClaims
```

### Running Tests

**Command Line:**
```bash
cd ContractMonthlyClaimSystem.Tests
dotnet test
```

**With Detailed Output:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

**Expected Output:**
```
Starting test execution...
Passed!  - Failed: 0, Passed: 12, Skipped: 0, Total: 12
Test Run Successful.
```

---

## 📁 Project Structure
```
ContractMonthlyClaimSystem/
├── Controllers/
│   ├── HomeController.cs
│   ├── LecturerController.cs          # Claim submission, auto-calc
│   ├── ReviewController.cs            # Automated validation
│   └── HRController.cs                # Invoice generation
├── Models/
│   ├── LecturerClaim.cs              # With automation methods
│   ├── Invoice.cs
│   ├── Lecturer.cs
│   ├── ValidationResult.cs
│   └── ErrorViewModel.cs
├── ViewModels/
│   ├── SubmitClaimViewModel.cs
│   └── ReviewClaimViewModel.cs
├── Views/
│   ├── Lecturer/
│   │   ├── Index.cshtml
│   │   ├── SubmitClaim.cshtml        # With auto-calc JavaScript
│   │   └── Details.cshtml
│   ├── Review/
│   │   ├── Index.cshtml
│   │   └── ReviewClaim.cshtml        # With validation display
│   └── HR/
│       ├── Index.cshtml              # Invoice dashboard
│       ├── ViewInvoice.cshtml
│       ├── ProcessedClaims.cshtml
│       └── Reports.cshtml
├── Data/
│   └── ApplicationDbContext.cs
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── uploads/                      # Uploaded documents
└── Areas/
    └── Identity/                     # Authentication pages

ContractMonthlyClaimSystem.Tests/
├── LecturerClaimTests.cs             # Model tests
├── LecturerControllerTests.cs        # Controller tests
└── ReviewControllerTests.cs          # Review tests
```

---

## 🗄 Database Schema

### Main Tables

#### LecturerClaims
```sql
- Id (PK)
- LecturerId (FK to AspNetUsers)
- LecturerName
- HoursWorked (decimal)
- HourlyRate (decimal)
- TotalAmount (decimal)
- AdditionalNotes
- DateSubmitted
- Status (Pending/Approved/Rejected)
- ReviewedBy
- ReviewDate
- ReviewComments
- DocumentPath
- DocumentFileSize
- DocumentFileType
- IsCoordinatorApproved (bool)
- IsManagerApproved (bool)
- IsHRProcessed (bool)
- InvoiceNumber
- InvoiceGeneratedDate
```

#### Invoices (Part 3)
```sql
- InvoiceId (PK)
- ClaimId (FK)
- InvoiceNumber
- TotalAmount
- TaxAmount (15%)
- NetAmount
- InvoiceDate
- DueDate
- PaymentStatus
- GeneratedBy
```

---

## 📖 Usage Guide

### For Lecturers

#### Submitting a Claim
1. **Login** with Lecturer credentials
2. Navigate to **"Submit Claim"**
3. Enter **Hours Worked** (watch total calculate automatically)
4. Enter **Hourly Rate**
5. View **Total Amount** (auto-calculated)
6. Check **validation badges** (all should be green)
7. Add **optional notes**
8. Upload **supporting document** (PDF, DOCX, etc.)
9. Click **"Submit Claim"** (enabled only when valid)
10. Receive **confirmation message**

### For Coordinators

#### Reviewing Claims
1. **Login** with Coordinator credentials
2. Navigate to **"Review Claims"**
3. Click **"Review Claim"** on pending claim
4. View **Automated Validation Results:**
   - 6 automated checks with ✓ or ⚠️
   - Overall recommendation
5. Review **claim details**
6. Add **review comments** (optional)
7. Click **"Approve"** or **"Reject"**
8. Claim moves to next stage (if approved)

### For Managers

#### Final Approval
1. **Login** with Manager credentials
2. Navigate to **"Review Claims"**
3. View **coordinator-approved claims**
4. System **enforces** sequential approval
   - Cannot approve claims not yet approved by coordinator
5. Review **validation results**
6. Add **final comments**
7. **Approve** for HR processing

### For HR

#### Generating Invoices
1. **Login** with HR credentials
2. Navigate to **"HR Dashboard"**
3. View **approved claims** awaiting processing
4. Click **"Generate Invoice"** for single claim
   - Or use **"Bulk Generate"** for multiple
5. System automatically:
   - Creates unique invoice number
   - Calculates tax (15%)
   - Computes net amount
   - Sets due date (30 days)
6. View/Print **generated invoice**
7. Export **reports** as needed

---

## ⚠️ Known Issues

### Issue 1: Visual Studio Test Explorer Discovery
**Problem:** Tests may not appear in Test Explorer  
**Workaround:** Use command line: `dotnet test`  
**Status:** Known Visual Studio issue with .NET 8

### Issue 2: File Upload Size Limit
**Problem:** Large files (>5MB) rejected  
**Solution:** Implemented in code for security  
**Future:** Consider cloud storage for larger files

### Issue 3: Concurrent Claim Submission
**Problem:** Potential race condition with simultaneous submissions  
**Status:** Low priority - unlikely scenario  
**Future:** Implement optimistic concurrency

---

## 🚀 Future Enhancements

### Phase 1 (Short-term)
- ✉️ Email notifications for claim status changes
- 📧 PDF invoice generation and email delivery
- 📊 Advanced reporting dashboard
- 📱 Mobile-responsive improvements

### Phase 2 (Medium-term)
- 🔐 Two-factor authentication
- 📁 Cloud storage integration (Azure Blob/AWS S3)
- 📈 Analytics and insights dashboard
- 🔔 Real-time notifications (SignalR)

### Phase 3 (Long-term)
- 🤖 Machine learning for fraud detection
- 📱 Mobile app (iOS/Android)
- 🌍 Multi-language support
- 💳 Payment gateway integration

---

## 🎓 Learning Outcomes

### Technical Skills Developed
- ✅ ASP.NET Core MVC architecture
- ✅ Entity Framework Core ORM
- ✅ Identity and authentication
- ✅ Role-based authorization
- ✅ JavaScript DOM manipulation
- ✅ AJAX and JSON handling
- ✅ Unit testing with NUnit
- ✅ Database design and migrations
- ✅ Git version control

### Soft Skills Enhanced
- ✅ Problem-solving strategies
- ✅ Code documentation
- ✅ Project planning
- ✅ Time management
- ✅ Technical communication

---

## 📜 License

This project is submitted as part of academic coursework for PROG6212 - Programming 2B.

**Academic Use Only**  
Not licensed for commercial use or redistribution.

© 2025 [Your Name]. All rights reserved.

---

## 🙏 Acknowledgments

- **Lecturer:** [Lecturer Name] - For guidance and requirements specification
- **Claude AI (Anthropic):** For code optimization assistance and problem-solving strategies
- **Microsoft Documentation:** For ASP.NET Core best practices
- **NUnit Community:** For testing framework and documentation
- **Stack Overflow Community:** For troubleshooting assistance

---

## 📞 Contact

**Student:** [Your Name]  
**Email:** [Your Email]  
**Student Number:** [Your Student Number]  
**Institution:** [Your Institution]

---

## 📝 Version History

### Version 1.0 - Part 1 (Completed)
- Basic MVC structure
- User authentication
- Claim submission functionality

### Version 2.0 - Part 2 (Completed)
- Multi-role authorization
- Claim review workflow
- Document upload feature

### Version 3.0 - Part 3 (Current)
- ✅ Auto-calculation feature
- ✅ Real-time validation
- ✅ Automated verification (6 checks)
- ✅ Workflow enforcement
- ✅ Invoice generation
- ✅ Comprehensive unit tests

---

**Last Updated:** November 21, 2025  
**Project Status:** ✅ Complete - Ready for Submission