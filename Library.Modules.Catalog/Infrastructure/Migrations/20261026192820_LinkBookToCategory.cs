using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Library.Modules.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkBookToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "catalog",
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                schema: "catalog",
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                schema: "catalog",
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "catalog",
                table: "Books",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                schema: "catalog",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryId",
                schema: "catalog",
                table: "Books",
                column: "CategoryId",
                principalSchema: "catalog",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryId",
                schema: "catalog",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CategoryId",
                schema: "catalog",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "catalog",
                table: "Books");

            migrationBuilder.InsertData(
                schema: "catalog",
                table: "Books",
                columns: new[] { "Id", "Author", "Isbn", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Robert C. Martin", "978-0134494166", "Clean Architecture" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Eric Evans", "978-0321125217", "Domain-Driven Design" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Martin Fowler", "978-0201485677", "Refactoring" }
                });
        }
    }
}
