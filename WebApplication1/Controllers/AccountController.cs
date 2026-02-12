using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _dbContext;

        public AccountController(ApplicationContext context, HttpContext httpContext)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(User user)
        {
            User currentUser = _dbContext.Users.FirstOrDefault(u => u.Login == user.Login);

            if (currentUser == null)
            {
                return (IActionResult)Results.NotFound("Пользователь не найден");
            }
            else
            {
                if (currentUser.Password != user.Password)
                {
                    return (IActionResult)Results.BadRequest("Неверный пароль");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, currentUser.Login),
                };
                var claimsIdentity = new ClaimsIdentity("BlackCat");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
