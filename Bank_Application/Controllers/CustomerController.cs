using Bank_Application.Data;
using Bank_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bank_Application.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
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

        // GET: Customer/Dashboard
        public IActionResult Dashboard()
        {
            // Check if customer is logged in
            if (!IsCustomerLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var customerId = GetCurrentCustomerId();

            // Get customer details
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            // Get customer accounts with total balance
            var accounts = _context.Accounts
                .Where(a => a.CustomerId == customerId)
                .ToList();

            decimal totalBalance = accounts.Sum(a => a.Balance);

            // Pass data to view
            ViewBag.CustomerName = customer.Name;
            ViewBag.Email = customer.Email;
            ViewBag.ContactInfo = customer.ContactInfo;
            ViewBag.TotalBalance = totalBalance;
            ViewBag.AccountCount = accounts.Count;

            return View(accounts);
        }

        // GET: Customer/CreateAccount
        public IActionResult CreateAccount()
        {
            // Check if customer is logged in
            if (!IsCustomerLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // POST: Customer/CreateAccount
        [HttpPost]
        public IActionResult CreateAccount(string accountType)
        {
            // Check if customer is logged in
            if (!IsCustomerLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrEmpty(accountType))
            {
                ViewBag.Error = "Please select an account type.";
                return View();
            }

            // Validate account type
            if (accountType != "SAVINGS" && accountType != "CURRENT")
            {
                ViewBag.Error = "Invalid account type.";
                return View();
            }

            var customerId = GetCurrentCustomerId();

            // Create new account
            var newAccount = new Account
            {
                CustomerId = (int)customerId,
                AccountType = accountType,
                Balance = 0
            };

            _context.Accounts.Add(newAccount);
            _context.SaveChanges();

            ViewBag.Success = $"{accountType} account created successfully!";
            return RedirectToAction("Dashboard");
        }

        // GET: Customer/GetAccountDetails/{accountId}
        public IActionResult GetAccountDetails(int accountId)
        {
            // Check if customer is logged in
            if (!IsCustomerLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var customerId = GetCurrentCustomerId();

            // Get account
            var account = _context.Accounts
                .FirstOrDefault(a => a.AccountId == accountId && a.CustomerId == customerId);

            if (account == null)
            {
                ViewBag.Error = "Account not found.";
                return RedirectToAction("Dashboard");
            }

            // Get transactions for this account
            var transactions = _context.Transactions
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            ViewBag.AccountId = account.AccountId;
            ViewBag.AccountType = account.AccountType;
            ViewBag.Balance = account.Balance;
            ViewBag.TransactionCount = transactions.Count;

            return View(transactions);
        }

        // GET: Customer/UpdateProfile
        public IActionResult UpdateProfile()
        {
            // Check if customer is logged in
            if (!IsCustomerLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var customerId = GetCurrentCustomerId();

            // Get customer details
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            return View(customer);
        }

        // POST: Customer/UpdateProfile
        [HttpPost]
        public IActionResult UpdateProfile(int customerId, string name, string email, string contactInfo)
        {
            // Check if customer is logged in
            if (!IsCustomerLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var currentCustomerId = GetCurrentCustomerId();

            // Verify the customer is updating their own profile
            if (customerId != currentCustomerId)
            {
                ViewBag.Error = "You can only update your own profile.";
                return RedirectToAction("Dashboard");
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contactInfo))
            {
                ViewBag.Error = "All fields are required.";
                return RedirectToAction("UpdateProfile");
            }

            // Get customer from database
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return RedirectToAction("Logout", "Auth");
            }

            // Check if email is already taken by another customer
            var emailExists = _context.Customers
                .FirstOrDefault(c => c.Email == email && c.CustomerId != customerId);

            if (emailExists != null)
            {
                ViewBag.Error = "Email is already in use.";
                return RedirectToAction("UpdateProfile");
            }

            // Update customer details
            customer.Name = name;
            customer.Email = email;
            customer.ContactInfo = contactInfo;

            _context.SaveChanges();

            // Update session with new name
            HttpContext.Session.SetString("username", name);

            ViewBag.Success = "Profile updated successfully!";
            return RedirectToAction("Dashboard");
        }
    }
}
