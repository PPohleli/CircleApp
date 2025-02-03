using CircleApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public StoriesViewComponent(AppDbContext contect)
        {
            _context = contect;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _context.Stories.Include(s => s.User).ToListAsync();
            return View(allStories);
        }
    }
}
