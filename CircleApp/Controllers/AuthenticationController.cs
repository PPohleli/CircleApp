﻿using CircleApp.Data.Helpers.Constants;
using CircleApp.Data.Models;
using CircleApp.ViewModels.Authentication;
using CircleApp.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CircleApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View();
            var existingUser = await _userManager.FindByEmailAsync(loginVM.Email);
            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid email or password. Please, try again!");
                return View(loginVM);

            }

            var existingUserClaims = await _userManager.GetClaimsAsync(existingUser);

            if (!existingUserClaims.Any(c => c.Type == CustomClaim.FullName))
                await _userManager.AddClaimAsync(existingUser, new Claim(CustomClaim.FullName, existingUser.FullName));

            var result = await _signInManager.PasswordSignInAsync(existingUser.UserName, loginVM.Password, false, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");


            ModelState.AddModelError("", "Invalid login attempt");
            return View(loginVM);
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordVM updatePasswordVM)
        {
            if (updatePasswordVM.NewPassword != updatePasswordVM.ConfirmPassword)
            {
                TempData["PasswordError"] = "Passwords do not match!";
                TempData["ActiveTab"] = "Password";

                return RedirectToAction("Index", "Settings");
            }

            var loggedInUser = await _userManager.GetUserAsync(User);
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(loggedInUser, updatePasswordVM.CurrentPassword);

            if (!isCurrentPasswordValid)
            {
                TempData["PasswordError"] = "Current password is invalid!";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }

            var result = await _userManager.ChangePasswordAsync(loggedInUser, updatePasswordVM.CurrentPassword, updatePasswordVM.NewPassword);

            if (result.Succeeded)
            {
                TempData["PasswordSuccess"] = "Password updated successfully!";
                TempData["ActiveTab"] = "Password";
                await _signInManager.RefreshSignInAsync(loggedInUser);
                
            }

            return RedirectToAction("Index", "Settings");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileVM profileVM)
        {
            var loggedInUser = await _userManager.GetUserAsync(User); 
            if (loggedInUser == null)
                return RedirectToAction("Login");

            loggedInUser.FullName = profileVM.Fullname;
            loggedInUser.UserName = profileVM.UserName;
            loggedInUser.Bio = profileVM.Bio;

            var result = await _userManager.UpdateAsync(loggedInUser);
            if (!result.Succeeded)
            {
                TempData["ProfileError"] = "User profile could not be updated!";
                TempData["ActiveTab"] = "Profile";

                return RedirectToAction("Index", "Settings");
            }
            await _signInManager.RefreshSignInAsync(loggedInUser);
            return RedirectToAction("Index", "Settings");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            var newUser = new User()
            {
                FullName = $"{registerVM.FirstName} {registerVM.LastName}",
                Email = registerVM.Email ,
                UserName = registerVM.Email
            };
            var existingUser = await _userManager.FindByEmailAsync(registerVM.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(registerVM);
            }

            var result = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);
                await _userManager.AddClaimAsync(newUser, new Claim(CustomClaim.FullName, newUser.FullName));
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }
    }
}
