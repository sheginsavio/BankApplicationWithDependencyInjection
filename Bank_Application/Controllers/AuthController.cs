using Bank_Application.Data;
using Bank_Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bank_Application.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            // If user is already logged in, redirect to dashboard
            if (HttpContext.Session.GetString("role") != null)
            {
                return RedirectToDashboard();
            }
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            // Check if Admin
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("role", "Admin");
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("Dashboard", "Admin");
            }

            // Check if LoanApprover
            if (username == "loanapprover" && password == "loanapprover123")
            {
                HttpContext.Session.SetString("role", "LoanApprover");
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("PendingLoans", "Loan");
            }

            // Check if Customer (search by email or name)
            var customer = _context.Customers
                .FirstOrDefault(c => c.Email == username || c.Name == username);

            if (customer != null && customer.Password == password)
            {
                HttpContext.Session.SetInt32("userId", customer.CustomerId);
                HttpContext.Session.SetString("role", "Customer");
                HttpContext.Session.SetString("username", customer.Name);
                return RedirectToAction("Dashboard", "Customer");
            }

            // Login failed
            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            // If user is already logged in, redirect to dashboard
            if (HttpContext.Session.GetString("role") != null)
            {
                return RedirectToDashboard();
            }
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        public IActionResult Register(string name, string email, string contactInfo, string password)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contactInfo) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            // Validate password length
            if (password.Length < 4)
            {
                ViewBag.Error = "Password must be at least 4 characters long.";
                return View();
            }

            // Check if email already exists
            var existingCustomer = _context.Customers.FirstOrDefault(c => c.Email == email);
            if (existingCustomer != null)
            {
                ViewBag.Error = "Email already registered.";
                return View();
            }

            // Create new customer
            var newCustomer = new Customer
            {
                Name = name,
                Email = email,
                ContactInfo = contactInfo,
                Password = password
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            ViewBag.Success = "Registration successful! You can now login.";
            return RedirectToAction("Login");
        }

        // GET: Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Helper method to redirect to appropriate dashboard
        private IActionResult RedirectToDashboard()
        {
            var role = HttpContext.Session.GetString("role");

            return role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "LoanApprover" => RedirectToAction("PendingLoans", "Loan"),
                "Customer" => RedirectToAction("Dashboard", "Customer"),
                _ => RedirectToAction("Login")
            };
        }
    }
}
