using Bank_Application.Models;

namespace Bank_Application.Services.Interfaces
{
    public interface ILoanService
    {
        // Apply loan
        void ApplyLoan(int customerId, decimal loanAmount);

        // Get customer loans
        List<Loan> GetCustomerLoans(int customerId);

        // Get pending loans
        List<Loan> GetPendingLoans();

        // Approve loan
        void ApproveLoan(int loanId, decimal interestRate);

        // Reject loan
        void RejectLoan(int loanId);

        // Get loan by ID
        Loan GetLoanById(int loanId);
    }
}
