using Bank_Application.Models;

namespace Bank_Application.Services.Interfaces
{
    public interface IReportService
    {
        // Get all transactions
        List<Transaction> GetAllTransactions();

        // Get all audit logs
        List<AuditLog> GetAllAuditLogs();

        // Get transaction statistics
        Dictionary<string, int> GetTransactionStatistics();

        // Get audit log statistics
        int GetTotalAuditLogs();
    }
}
