using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank_Application.Migrations
{
    /// <inheritdoc />
    public partial class FixAuditLogTransactionIdNullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Transactions_TransactionId",
                table: "AuditLogs");

            // Alter column to be nullable
            migrationBuilder.AlterColumn<int>(
                name: "TransactionId",
                table: "AuditLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: false);

            // Re-add foreign key with optional relationship
            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Transactions_TransactionId",
                table: "AuditLogs",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Transactions_TransactionId",
                table: "AuditLogs");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionId",
                table: "AuditLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Transactions_TransactionId",
                table: "AuditLogs",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "TransactionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
