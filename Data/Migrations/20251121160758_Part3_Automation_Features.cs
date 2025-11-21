using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractMonthlyClaimSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class Part3_Automation_Features : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CoordinatorApprovedDate",
                table: "LecturerClaims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoordinatorName",
                table: "LecturerClaims",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DocumentFileSize",
                table: "LecturerClaims",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentFileType",
                table: "LecturerClaims",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HRPersonnelName",
                table: "LecturerClaims",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HRProcessedDate",
                table: "LecturerClaims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvoiceGeneratedDate",
                table: "LecturerClaims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "LecturerClaims",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCoordinatorApproved",
                table: "LecturerClaims",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHRProcessed",
                table: "LecturerClaims",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManagerApproved",
                table: "LecturerClaims",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ManagerApprovedDate",
                table: "LecturerClaims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerName",
                table: "LecturerClaims",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClaimId = table.Column<int>(type: "int", nullable: false),
                    LecturerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LecturerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GeneratedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_LecturerClaims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "LecturerClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lecturers",
                columns: table => new
                {
                    LecturerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DefaultHourlyRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturers", x => x.LecturerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ClaimId",
                table: "Invoices",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lecturers_Email",
                table: "Lecturers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Lecturers");

            migrationBuilder.DropColumn(
                name: "CoordinatorApprovedDate",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "CoordinatorName",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "DocumentFileSize",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "DocumentFileType",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "HRPersonnelName",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "HRProcessedDate",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "InvoiceGeneratedDate",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "IsCoordinatorApproved",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "IsHRProcessed",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "IsManagerApproved",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "ManagerApprovedDate",
                table: "LecturerClaims");

            migrationBuilder.DropColumn(
                name: "ManagerName",
                table: "LecturerClaims");
        }
    }
}
