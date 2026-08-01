using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicManagement.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ClinicManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;


        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }



        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }




        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string username,
            string password)
        {

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(
                    u => u.Username == username
                );


            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }



            // kiểm tra password
            bool checkPassword =
                BCrypt.Net.BCrypt.Verify(
                    password,
                    user.PasswordHash
                );



            if (!checkPassword)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }



            // tạo thông tin đăng nhập
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    user.FullName
                ),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.Name
                )
            };



            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );


            var principal = new ClaimsPrincipal(identity);



            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );



            return RedirectToAction(
                "Index",
                "Home"
            );
        }





        // Logout
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );


            return RedirectToAction("Login");
        }

    }
}