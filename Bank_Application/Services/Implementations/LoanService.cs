using Bank_Application.Data;
using Bank_Application.Models;
using Bank_Application.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bank_Application.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ApplicationDbContext _context;

        public LoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void ApplyLoan(int customerId, decimal loanAmount)
        {
            if (loanAmount <= 0)
                throw new ArgumentException("Loan amount must be greater than 0");

            var newLoan = new Loan
            {
                CustomerId = customerId,
                LoanAmount = loanAmount,
                InterestRate = 0,
                LoanStatus = "APPLIED"
            };

            _context.Loans.Add(newLoan);
            _context.SaveChanges();
        }

        public List<Loan> GetCustomerLoans(int customerId)
        {
            return _context.Loans
                .Where(l => l.CustomerId == customerId)
                .OrderByDescending(l => l.LoanId)
                .ToList();
        }

        public List<Loan> GetPendingLoans()
        {
            return _context.Loans
                .Where(l => l.LoanStatus == "APPLIED")
                .OrderBy(l => l.LoanId)
                .ToList();
        }

        public Loan GetLoanById(int loanId)
        {
            return _context.Loans.FirstOrDefault(l => l.LoanId == loanId);
        }

        public void ApproveLoan(int loanId, decimal interestRate)
        {
            if (interestRate < 0)
                throw new ArgumentException("Interest rate cannot be negative");

            var loan = GetLoanById(loanId);
            if (loan == null)
                throw new ArgumentException("Loan not found");

            // Set loan status and interest rate
            loan.LoanStatus = "APPROVED";
            loan.InterestRate = interestRate;

            // Try to add loan amount to customer's account balance
            var account = _context.Accounts
                .FirstOrDefault(a => a.CustomerId == loan.CustomerId);

            if (account != null)
            {
                // Add loan amount to account balance
                account.Balance += loan.LoanAmount;

                // Create a transaction record for audit
                var transaction = new Transaction
                {
                    AccountId = account.AccountId,
                    TransactionType = "TRANSFER",
                    Amount = loan.LoanAmount,
                    TransactionDate = DateTime.Now
                };

                _context.Transactions.Add(transaction);
                _context.SaveChanges();

                // Create audit log for loan approval
                var auditLog = new AuditLog
                {
                    TransactionId = transaction.TransactionId,
                    ActionPerformed = "LOAN_APPROVED",
                    PerformedBy = "LoanApprover",
                    LogDate = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);
            }
            else
            {
                _context.SaveChanges();

                // Create audit log without transaction ID (set to null, NOT 0)
                var auditLog = new AuditLog
                {
                    TransactionId = null,  // ✅ Set to null, NOT 0
                    ActionPerformed = "LOAN_APPROVED",
                    PerformedBy = "LoanApprover",
                    LogDate = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);
            }

            _context.SaveChanges();
        }

        public void RejectLoan(int loanId)
        {
            var loan = GetLoanById(loanId);
            if (loan == null)
                throw new ArgumentException("Loan not found");

            // Set loan status to rejected
            loan.LoanStatus = "REJECTED";
            loan.InterestRate = 0;

            // Save the loan rejection
            _context.SaveChanges();

            // Create audit log for loan rejection (no transaction, so set to null)
            var auditLog = new AuditLog
            {
                TransactionId = null,  // ✅ Set to null, NOT 0
                ActionPerformed = "LOAN_REJECTED",
                PerformedBy = "LoanApprover",
                LogDate = DateTime.Now
            };

            _context.AuditLogs.Add(auditLog);
            _context.SaveChanges();
        }
    }
}
