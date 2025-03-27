using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace WAREHOUSE_MANAGEMENT_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class CreateStockMovementsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    MovementType = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    MovementDate = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    Country = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId",
                table: "StockMovements",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
