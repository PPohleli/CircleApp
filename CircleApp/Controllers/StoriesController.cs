using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.ViewModels.Stories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Controllers
{
    public class StoriesController : Controller
    {
        private readonly AppDbContext _context;

        public StoriesController(AppDbContext contect)
        {
            _context = contect;
        }

        public async Task<IActionResult> Index()
        
        {
            var allStories = await _context.Stories.Include(s => s.User).ToListAsync();
            return View(allStories);
        }

        public async Task<IActionResult> CreateStory(StoryVM storyVM)
        {
            int loggedInUser = 1;

            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                UserId = loggedInUser
            };

            // Check and save the image
            if (storyVM.Image != null && storyVM.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (storyVM.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImage = Path.Combine(rootFolderPath, "images/stories/");
                    Directory.CreateDirectory(rootFolderPathImage);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(storyVM.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImage, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await storyVM.Image.CopyToAsync(stream);

                    // Set the URL to the newPost object
                    newStory.ImageUrl = "/images/stories/" + fileName;
                }
            }
            await _context.Stories.AddAsync(newStory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
