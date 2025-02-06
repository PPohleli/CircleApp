using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Stories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Controllers
{
    public class StoriesController : Controller
    {
        private readonly IStoriesServices _storiesService;

        public StoriesController(IStoriesServices storiesService)
        {
            _storiesService = storiesService;
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
            await _storiesService.CreateStoryAsync(newStory, storyVM.Image);
            return RedirectToAction("Index", "Home");
        }
    }
}
