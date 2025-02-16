﻿using CircleApp.Controllers.Base;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService _usersService;
        private readonly UserManager<User> _userManager;
        public UsersController(IUsersService usersService, UserManager<User> userManager)
        {
            _usersService = usersService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userPosts = await _usersService.GetUserPosts(userId);
            var userProfile = new GetUserProfileVM()
            {
                User = user,
                Posts = userPosts
            };

            return View(userProfile);
        }
    }
}
