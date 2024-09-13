using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthInsurance.Migrations
{
    /// <inheritdoc />
    public partial class updateappdbcontextforpolicys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MedicalId",
                table: "HospitalInfo",
                newName: "HospitalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HospitalId",
                table: "HospitalInfo",
                newName: "MedicalId");
        }
    }
}
