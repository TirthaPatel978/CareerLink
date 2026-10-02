using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerLink.Migrations
{
    /// <inheritdoc />
    public partial class AddJobSeekerResume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumePath",
                table: "JobSeekers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "JobSeekers");

            migrationBuilder.DropColumn(
                name: "ResumePath",
                table: "JobSeekers");
        }
    }
}
