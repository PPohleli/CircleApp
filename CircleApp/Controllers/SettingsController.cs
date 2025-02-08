using CircleApp.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IUsersService _usersService;
        public SettingsController(IUsersService usersService) 
        {
            _usersService = usersService;
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUser = 1;

            var userDb = await _usersService.GetUser(loggedInUser);

            return View(userDb);
        }
    }
}
