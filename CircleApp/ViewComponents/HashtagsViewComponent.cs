using CircleApp.Controllers;
using CircleApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.ViewComponents
{
    public class HashtagsViewComponent : ViewComponent 
    {
        private readonly AppDbContext _context;

        public HashtagsViewComponent(AppDbContext contect)
        {
            _context = contect;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var top3Hashtags = await _context.Hashtags.Where(h => h.DateCreated >= h.DateCreated.AddDays(-7) && h.Count > 0).OrderByDescending(n => n.Count).Take(3).ToListAsync();
            return View(top3Hashtags);
        }
    }
}
