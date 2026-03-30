using Bank_Application.Models;

namespace Bank_Application.Services.Interfaces
{
    public interface IRepaymentService
    {
        // Get approved loans for customer
        List<Loan> GetApprovedLoansForCustomer(int customerId);

        // Make repayment
        void MakeRepayment(int customerId, int loanId, decimal amountPaid);

        // Get repayments for loan
        List<Repayment> GetRepaymentsByLoanId(int loanId);

        // Get customer repayment history
        List<Repayment> GetCustomerRepaymentHistory(int customerId, int loanId);

        // Calculate total amount payable
        decimal CalculateTotalAmount(Loan loan);

        // Get total paid
        decimal GetTotalPaid(int loanId);
    }
}
