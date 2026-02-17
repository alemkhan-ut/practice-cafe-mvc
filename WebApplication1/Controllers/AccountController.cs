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
