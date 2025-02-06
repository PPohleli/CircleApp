using CircleApp.Data;
using CircleApp.Data.Helpers.Enums;
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
        private readonly IFilesService _filesService;

        public StoriesController(IStoriesServices storiesService, IFilesService filesService)
        {
            _storiesService = storiesService;
            _filesService = filesService;
        }

        public async Task<IActionResult> CreateStory(StoryVM storyVM)
        {
            int loggedInUser = 1;
            var fileUploadPath = await _filesService.UploadImageAsync(storyVM.Image, ImageFileType.StoryImage);
            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                ImageUrl = fileUploadPath,
                UserId = loggedInUser
            };
            await _storiesService.CreateStoryAsync(newStory);
            return RedirectToAction("Index", "Home");
        }
    }
}
