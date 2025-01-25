using CircleApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CircleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext contect)
        {
            _logger = logger;
            _context = contect;
        }

        public async Task<IActionResult> Index()
        {
            var allPosts = await _context.Posts.Include(n => n.User).ToListAsync();
            return View(allPosts);
        }
    }
}
