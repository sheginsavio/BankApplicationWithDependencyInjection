using Microsoft.AspNetCore.Mvc;

namespace Bank_Application.Controllers
{
    public class AdminController : Controller
    {
        // Helper method to check if user is logged in as Admin
        private bool IsAdminLoggedIn()
        {
            return HttpContext.Session.GetString("role") == "Admin";
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Check if admin is logged in
            if (!IsAdminLoggedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }
}
