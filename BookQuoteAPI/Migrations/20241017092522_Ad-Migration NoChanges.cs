using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookQuoteAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdMigrationNoChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
        name: "Books",
        columns: table => new
        {
            Id = table.Column<int>(nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Title = table.Column<string>(nullable: true),
            Author = table.Column<string>(nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Books", x => x.Id);
        });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
        name: "Books");

        }
    }
}


/*
 * 
  migrationBuilder.CreateTable(
        name: "Books",
        columns: table => new
        {
            Id = table.Column<int>(nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Title = table.Column<string>(nullable: true),
            Author = table.Column<string>(nullable: true)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Books", x => x.Id);
        });
}

protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.DropTable(
        name: "Books");

 */