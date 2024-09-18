using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthInsurance.Migrations
{
    /// <inheritdoc />
    public partial class d : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestId",
                table: "PolicyApprovals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyApprovals_RequestId",
                table: "PolicyApprovals",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyApprovals_PolicyRequests_RequestId",
                table: "PolicyApprovals",
                column: "RequestId",
                principalTable: "PolicyRequests",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyApprovals_PolicyRequests_RequestId",
                table: "PolicyApprovals");

            migrationBuilder.DropIndex(
                name: "IX_PolicyApprovals_RequestId",
                table: "PolicyApprovals");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "PolicyApprovals");
        }
    }
}
