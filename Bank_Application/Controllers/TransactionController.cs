using Bank_Application.Models;
using Bank_Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Application.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // Helper method to check if user is logged in as Customer
        private bool IsCustomerLoggedIn()
        {
            return HttpContext.Session.GetString("role") == "Customer" &&
                   HttpContext.Session.GetInt32("userId") != null;
        }

        // Helper method to get current customer ID
        private int? GetCurrentCustomerId()
        {
            return HttpContext.Session.GetInt32("userId");
        }

        // GET: Transaction/Deposit
        public IActionResult Deposit()
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "Auth");

            var customerId = (int)GetCurrentCustomerId();
            var accounts = _transactionService.GetCustomerAccounts(customerId);

            if (accounts.Count == 0)
            {
                ViewBag.Error = "You have no accounts. Please create an account first.";
                return View();
            }

            return View(accounts);
        }

        // POST: Transaction/Deposit
        [HttpPost]
        public IActionResult Deposit(int accountId, decimal amount)
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "Auth");

            var customerId = (int)GetCurrentCustomerId();

            try
            {
                _transactionService.Deposit(accountId, amount);
                ViewBag.Success = $"Deposit of {amount:C} successful!";
                return RedirectToAction("Dashboard", "Customer");
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                var accounts = _transactionService.GetCustomerAccounts(customerId);
                return View(accounts);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error processing deposit: {ex.Message}";
                var accounts = _transactionService.GetCustomerAccounts(customerId);
                return View(accounts);
            }
        }

        // GET: Transaction/Withdraw
        public IActionResult Withdraw()
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "Auth");

            var customerId = (int)GetCurrentCustomerId();
            var accounts = _transactionService.GetCustomerAccounts(customerId);

            if (accounts.Count == 0)
            {
                ViewBag.Error = "You have no accounts. Please create an account first.";
                return View();
            }

            return View(accounts);
        }

        // POST: Transaction/Withdraw
        [HttpPost]
        public IActionResult Withdraw(int accountId, decimal amount)
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "Auth");

            var customerId = (int)GetCurrentCustomerId();

            try
            {
                _transactionService.Withdraw(accountId, amount);
                ViewBag.Success = $"Withdrawal of {amount:C} successful!";
                return RedirectToAction("Dashboard", "Customer");
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                var accounts = _transactionService.GetCustomerAccounts(customerId);
                return View(accounts);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error processing withdrawal: {ex.Message}";
                var accounts = _transactionService.GetCustomerAccounts(customerId);
                return View(accounts);
            }
        }

        // GET: Transaction/Transfer
        public IActionResult Transfer()
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "Auth");

            var customerId = (int)GetCurrentCustomerId();
            var accounts = _transactionService.GetCustomerAccounts(customerId);

            if (accounts.Count == 0)
            {
                ViewBag.Error = "You have no accounts. Please create an account first.";
                return View();
            }

            ViewBag.Accounts = accounts;
            return View();
        }

        // POST: Transaction/Transfer
        [HttpPost]
        public IActionResult Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "Auth");

            var customerId = (int)GetCurrentCustomerId();

            try
            {
                _transactionService.Transfer(fromAccountId, toAccountId, amount);
                ViewBag.Success = $"Transfer of {amount:C} successful!";
                return RedirectToAction("Dashboard", "Customer");
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                var accounts = _transactionService.GetCustomerAccounts(customerId);
                ViewBag.Accounts = accounts;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error processing transfer: {ex.Message}";
                var accounts = _transactionService.GetCustomerAccounts(customerId);
                ViewBag.Accounts = accounts;
                return View();
            }
        }

        // GET: Transaction/ViewTransactions
        public IActionResult ViewTransactions(int accountId = 0)
        {
            if (!IsCustomerLoggedIn())
                return RedirectToAction("Login", "Auth");

            var customerId = (int)GetCurrentCustomerId();
            var accounts = _transactionService.GetCustomerAccounts(customerId);

            if (accounts.Count == 0)
            {
                ViewBag.Error = "You have no accounts.";
                return View(new List<Transaction>());
            }

            if (accountId == 0 && accounts.Count > 0)
                accountId = accounts[0].AccountId;

            var selectedAccount = accounts.FirstOrDefault(a => a.AccountId == accountId);
            if (selectedAccount == null)
            {
                ViewBag.Error = "Account not found.";
                return View(new List<Transaction>());
            }

            var transactions = _transactionService.GetTransactionsByAccountId(accountId);

            ViewBag.Accounts = accounts;
            ViewBag.SelectedAccountId = accountId;
            ViewBag.SelectedAccountType = selectedAccount.AccountType;
            ViewBag.SelectedAccountBalance = selectedAccount.Balance;
            ViewBag.TransactionCount = transactions.Count;

            return View(transactions);
        }
    }
}
