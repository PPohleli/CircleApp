using CircleApp.Controllers.Base;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Helpers.Enums;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Stories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.Controllers
{
    [Authorize(Roles = AppRoles.User)]
    public class StoriesController : BaseController
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
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogin();

            var fileUploadPath = await _filesService.UploadImageAsync(storyVM.Image, ImageFileType.StoryImage);
            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                ImageUrl = fileUploadPath,
                UserId = loggedInUserId.Value
            };
            await _storiesService.CreateStoryAsync(newStory);
            return RedirectToAction("Index", "Home");
        }
    }
}
