using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaneTicketShop.Core.Models;
using PlaneTicketShop.Infrastructure.Data;
using System.Security.Claims;

namespace PlaneTicketShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Please enter both username and password.");
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null || user.PasswordHash != password)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // provjera maila
                    if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Email already exists.");
                        return View(model);
                    }

                    // provjera usernamea
                    if (await _context.Users.AnyAsync(u => u.UserName == model.UserName))
                    {
                        ModelState.AddModelError("UserName", "Username already exists.");
                        return View(model);
                    }

                    // default values
                    model.Role = "User";
                    model.Bookings = new List<Booking>();

                    // dodati usera u bazu
                    _context.Users.Add(model);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User registered successfully: {Username}", model.UserName);
                    return RedirectToAction(nameof(Login));
                }

                // failed
                _logger.LogWarning("Registration failed for user: {Username}. ModelState errors: {Errors}", 
                    model.UserName, 
                    string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Username}", model.UserName);
                ModelState.AddModelError("", "An error occurred during registration. Please try again.");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Login));
            }

            var userId = int.Parse(User.Identity.Name);
            var user = _context.Users
                .Include(u => u.Bookings)
                .ThenInclude(b => b.Flight)
                .FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
} 