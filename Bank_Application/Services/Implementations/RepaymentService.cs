using Bank_Application.Data;
using Bank_Application.Models;
using Bank_Application.Services.Interfaces;

namespace Bank_Application.Services.Implementations
{
    public class RepaymentService : IRepaymentService
    {
        private readonly ApplicationDbContext _context;

        public RepaymentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Loan> GetApprovedLoansForCustomer(int customerId)
        {
            return _context.Loans
                .Where(l => l.CustomerId == customerId && l.LoanStatus == "APPROVED")
                .ToList();
        }

        public void MakeRepayment(int customerId, int loanId, decimal amountPaid)
        {
            try
            {
                // ========== STEP 1: Validate Input ==========
                if (amountPaid <= 0)
                    throw new ArgumentException("Repayment amount must be greater than 0");

                // ========== STEP 2: Get Loan ==========
                var loan = _context.Loans.FirstOrDefault(l => l.LoanId == loanId && l.CustomerId == customerId);

                if (loan == null)
                    throw new ArgumentException("Loan not found");

                if (loan.LoanStatus == "COMPLETED")
                    throw new ArgumentException("This loan has already been fully repaid");

                if (loan.LoanStatus != "APPROVED")
                    throw new ArgumentException("Only approved loans can be repaid");

                // ========== STEP 3: Get Customer Account ==========
                var account = _context.Accounts.FirstOrDefault(a => a.CustomerId == customerId);

                if (account == null)
                    throw new ArgumentException("Account not found. Please create an account first");

                // ========== STEP 4: Calculate Total Amount with Interest ==========
                // totalAmount = LoanAmount + (LoanAmount * InterestRate / 100)
                decimal totalAmount = loan.LoanAmount + (loan.LoanAmount * loan.InterestRate / 100);

                // ========== STEP 5: Get Total Already Paid ==========
                decimal totalPaid = GetTotalPaid(loanId);

                // ========== STEP 6: Calculate Remaining Balance ==========
                decimal remainingBalance = totalAmount - totalPaid;

                if (remainingBalance <= 0)
                    throw new ArgumentException("This loan has already been fully repaid");

                // ========== STEP 7: Validate Repayment Amount ==========
                if (amountPaid > remainingBalance)
                    throw new ArgumentException($"Cannot pay more than remaining balance of {remainingBalance:C}");

                // ========== STEP 8: Validate Account Balance ==========
                if (account.Balance < amountPaid)
                    throw new ArgumentException($"Insufficient account balance. Your balance: {account.Balance:C}, Required: {amountPaid:C}");

                // ========== STEP 9: Calculate New Remaining Balance ==========
                decimal newRemainingBalance = remainingBalance - amountPaid;

                // ========== STEP 10: Deduct from Account ==========
                account.Balance -= amountPaid;

                // ========== STEP 11: Create Repayment Record ==========
                var repayment = new Repayment
                {
                    LoanId = loanId,
                    AmountPaid = amountPaid,
                    RepaymentDate = DateTime.Now,
                    BalanceRemaining = newRemainingBalance  // IMPORTANT: Set the new remaining balance
                };

                _context.Repayments.Add(repayment);

                // ========== STEP 12: Update Loan Status if Completed ==========
                if (newRemainingBalance <= 0)
                {
                    loan.LoanStatus = "COMPLETED";
                }

                // ========== STEP 13: Save All Changes (Account + Repayment + Loan) ==========
                _context.SaveChanges();

                // ========== STEP 14: Create Audit Log (AFTER saving repayment) ==========
                var auditLog = new AuditLog
                {
                    TransactionId = null,  // ✅ NO TRANSACTION - Set to null
                    ActionPerformed = "REPAYMENT",
                    PerformedBy = "Customer",
                    LogDate = DateTime.Now
                };

                _context.AuditLogs.Add(auditLog);

                // ========== STEP 15: Save Audit Log ==========
                _context.SaveChanges();
            }
            catch (ArgumentException)
            {
                // Re-throw validation errors
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error processing repayment: {ex.Message}", ex);
            }
        }

        public List<Repayment> GetRepaymentsByLoanId(int loanId)
        {
            return _context.Repayments
                .Where(r => r.LoanId == loanId)
                .OrderBy(r => r.RepaymentDate)
                .ToList();
        }

        public List<Repayment> GetCustomerRepaymentHistory(int customerId, int loanId)
        {
            var loan = _context.Loans.FirstOrDefault(l => l.LoanId == loanId && l.CustomerId == customerId);
            if (loan == null)
                return new List<Repayment>();

            return _context.Repayments
                .Where(r => r.LoanId == loanId)
                .OrderBy(r => r.RepaymentDate)
                .ToList();
        }

        public decimal CalculateTotalAmount(Loan loan)
        {
            // Total Amount = Loan Amount + (Loan Amount * Interest Rate / 100)
            return loan.LoanAmount + (loan.LoanAmount * loan.InterestRate / 100);
        }

        public decimal GetTotalPaid(int loanId)
        {
            return _context.Repayments
                .Where(r => r.LoanId == loanId)
                .Sum(r => r.AmountPaid);
        }
    }
}
