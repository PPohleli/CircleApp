using CircleApp.Data.Services;
using CircleApp.ViewModels.Settings;
using CircleApp.Data.Helpers.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CircleApp.Data.Models;

namespace CircleApp.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly IFilesService _filesService;
        private readonly UserManager<User> _userManager;
        public SettingsController(IUsersService usersService, IFilesService filesService, UserManager<User> userManager)
        {
            _usersService = usersService;
            _filesService = filesService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userDb = await _usersService.GetUser(int.Parse(loggedInUserId));

            var loggedInUser = await _userManager.GetUserAsync(User);

            return View(loggedInUser);
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
