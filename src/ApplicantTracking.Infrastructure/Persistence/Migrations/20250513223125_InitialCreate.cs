using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicantTracking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "candidates",
                columns: table => new
                {
                    IdCandidate = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    Surname = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Birthdate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Email = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_candidates", x => x.IdCandidate);
                });

            migrationBuilder.CreateTable(
                name: "timelines",
                columns: table => new
                {
                    IdTimeline = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTimelineType = table.Column<byte>(type: "tinyint", nullable: false),
                    IdAggregateRoot = table.Column<int>(type: "int", nullable: false),
                    OldData = table.Column<string>(type: "varchar(max)", nullable: true),
                    NewData = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timelines", x => x.IdTimeline);
                });

            migrationBuilder.CreateIndex(
                name: "IX_candidates_Email",
                table: "candidates",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "candidates");

            migrationBuilder.DropTable(
                name: "timelines");
        }
    }
}
