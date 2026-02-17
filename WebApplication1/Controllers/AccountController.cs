using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _dbContext;

        public AccountController(ApplicationContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult ChoTakoe()
        {
            return BadRequest("Ты что тут делаешь????");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            await LoginProcessAsync(user);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(User user)
        {
            // Ищем пользователя в базе данных по логину
            User currentUser = _dbContext.Users.FirstOrDefault(u => u.Login == user.Login);

            if (currentUser == null)
            {
                return NotFound("Пользователь не найден");
            }
            else
            {
                if (currentUser.Password != user.Password)
                {
                    return BadRequest("Неверный пароль");
                }

                await LoginProcessAsync(currentUser);

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task LoginProcessAsync(User user)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.DateOfBirth, user.BirthDate.ToString("dd.MM.yyyy"))
                };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);
        }
    }
}
