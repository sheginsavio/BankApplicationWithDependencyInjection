using Bank_Application.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bank_Application.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to check if user is logged in as Admin
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("role") == "Admin";
        }

        // GET: Report/GenerateTransactionReport
        public IActionResult GenerateTransactionReport()
        {
            // Check if admin is logged in
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            // Fetch all transactions with related data
            var transactions = _context.Transactions
                .Include(t => t.Account)
                .ThenInclude(a => a.Customer)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            // Create report data
            var reportData = transactions.Select(t => new
            {
                TransactionId = t.TransactionId,
                AccountId = t.AccountId,
                CustomerName = t.Account?.Customer?.Name ?? "Unknown",
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate
            }).ToList();

            ViewBag.TotalTransactions = reportData.Count;
            ViewBag.TotalAmount = reportData.Sum(r => r.Amount);
            ViewBag.DepositCount = reportData.Count(r => r.TransactionType == "DEPOSIT");
            ViewBag.WithdrawalCount = reportData.Count(r => r.TransactionType == "WITHDRAWAL");
            ViewBag.TransferCount = reportData.Count(r => r.TransactionType == "TRANSFER");

            return View(reportData);
        }

        // GET: Report/GetAuditLogs
        public IActionResult GetAuditLogs()
        {
            // Check if admin is logged in
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            // Fetch all audit logs with related transaction data
            var auditLogs = _context.AuditLogs
                .Include(al => al.Transaction)
                .OrderByDescending(al => al.LogDate)
                .ToList();

            ViewBag.TotalAuditLogs = auditLogs.Count;

            return View(auditLogs);
        }
    }
}
