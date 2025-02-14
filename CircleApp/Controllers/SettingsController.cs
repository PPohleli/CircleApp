using CircleApp.Data.Services;
using CircleApp.ViewModels.Settings;
using CircleApp.Data.Helpers.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CircleApp.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IFilesService _filesService;
        public SettingsController(IUsersService usersService, IFilesService filesService)
        {
            _usersService = usersService;
            _filesService = filesService;
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUser = 1;

            var userDb = await _usersService.GetUser(loggedInUser);

            return View(userDb);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureVM profilePictureVM)
        {
            var loggedInUser = 1;

            var uploadedProfilePictureUrl = await _filesService.UploadImageAsync(profilePictureVM.ProfilePictureImage, ImageFileType.ProfilePicture);
            await _usersService.UpdateUserProfilePicture(loggedInUser, uploadedProfilePictureUrl);
            return RedirectToAction("Index");
        }
    }
}
