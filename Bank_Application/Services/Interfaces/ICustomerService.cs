using Bank_Application.Models;

namespace Bank_Application.Services.Interfaces
{
    public interface ICustomerService
    {
        // Get operations
        Customer GetCustomerById(int customerId);
        Customer GetCustomerByEmail(string email);
        Customer GetCustomerByName(string name);

        // Create operations
        void CreateAccount(int customerId, string accountType);

        // Update operations
        void UpdateCustomer(Customer customer);

        // Save operations
        void SaveChanges();
    }
}
