using Bank_Application.Models;

namespace Bank_Application.Services.Interfaces
{
    public interface ITransactionService
    {
        // Deposit
        void Deposit(int accountId, decimal amount);

        // Withdraw
        void Withdraw(int accountId, decimal amount);

        // Transfer
        void Transfer(int fromAccountId, int toAccountId, decimal amount);

        // Get transactions
        List<Transaction> GetTransactionsByAccountId(int accountId);

        // Get customer accounts
        List<Account> GetCustomerAccounts(int customerId);

        // Create audit log
        void CreateAuditLog(int? transactionId, string actionPerformed, string performedBy);
    }
}

