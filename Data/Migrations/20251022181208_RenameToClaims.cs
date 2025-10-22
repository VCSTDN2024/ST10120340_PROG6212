using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractMonthlyClaimSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameToClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Claims",
                table: "Claims");

            migrationBuilder.RenameTable(
                name: "Claims",
                newName: "LecturerClaims");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LecturerClaims",
                table: "LecturerClaims",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LecturerClaims",
                table: "LecturerClaims");

            migrationBuilder.RenameTable(
                name: "LecturerClaims",
                newName: "Claims");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Claims",
                table: "Claims",
                column: "Id");
        }
    }
}
