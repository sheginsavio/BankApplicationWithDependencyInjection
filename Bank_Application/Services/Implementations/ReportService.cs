using Bank_Application.Data;
using Bank_Application.Models;
using Bank_Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank_Application.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Transaction> GetAllTransactions()
        {
            return _context.Transactions
                .Include(t => t.Account)
                .ThenInclude(a => a.Customer)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        public List<AuditLog> GetAllAuditLogs()
        {
            return _context.AuditLogs
                .Include(al => al.Transaction)
                .OrderByDescending(al => al.LogDate)
                .ToList();
        }

        public Dictionary<string, int> GetTransactionStatistics()
        {
            var stats = new Dictionary<string, int>
            {
                { "DEPOSIT", _context.Transactions.Count(t => t.TransactionType == "DEPOSIT") },
                { "WITHDRAWAL", _context.Transactions.Count(t => t.TransactionType == "WITHDRAWAL") },
                { "TRANSFER_IN", _context.Transactions.Count(t => t.TransactionType == "TRANSFER_IN") },
                { "TRANSFER_OUT", _context.Transactions.Count(t => t.TransactionType == "TRANSFER_OUT") }
            };

            return stats;
        }

        public int GetTotalAuditLogs()
        {
            return _context.AuditLogs.Count();
        }
    }
}
