using Bank_Application.Data;
using Bank_Application.Models;

namespace Bank_Application.Services.Helpers
{
    /// <summary>
    /// Helper class for safe AuditLog creation
    /// Ensures TransactionId is either a valid ID or null, NEVER 0 or invalid values
    /// </summary>
    public class AuditLogHelper
    {
        private readonly ApplicationDbContext _context;

        public AuditLogHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates an audit log with a valid transaction ID
        /// Use this when the action is linked to a transaction
        /// </summary>
        public void CreateAuditLogWithTransaction(int transactionId, string actionPerformed, string performedBy)
        {
            // Validate transaction exists
            var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == transactionId);
            if (transaction == null)
                throw new ArgumentException($"Transaction ID {transactionId} not found");

            var auditLog = new AuditLog
            {
                TransactionId = transactionId,
                ActionPerformed = actionPerformed,
                PerformedBy = performedBy,
                LogDate = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }

        /// <summary>
        /// Creates an audit log WITHOUT a transaction
        /// Use this for actions like loan approval/rejection, repayments, etc.
        /// TransactionId is set to NULL
        /// </summary>
        public void CreateAuditLogWithoutTransaction(string actionPerformed, string performedBy)
        {
            var auditLog = new AuditLog
            {
                TransactionId = null,  // ✅ Explicitly null, never 0
                ActionPerformed = actionPerformed,
                PerformedBy = performedBy,
                LogDate = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }

        /// <summary>
        /// Creates an audit log with optional transaction
        /// Use this when you're not sure if there's a transaction
        /// If transactionId is 0 or less, it's treated as null
        /// </summary>
        public void CreateAuditLog(int? transactionId, string actionPerformed, string performedBy)
        {
            // Convert 0 or negative to null
            int? safeTransactionId = null;
            
            if (transactionId.HasValue && transactionId.Value > 0)
            {
                safeTransactionId = transactionId.Value;
            }

            var auditLog = new AuditLog
            {
                TransactionId = safeTransactionId,  // ✅ Null if 0 or negative
                ActionPerformed = actionPerformed,
                PerformedBy = performedBy,
                LogDate = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }
    }
}
