﻿using CircleApp.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CircleApp.ViewComponents
{
    public class SuggestedFriendsViewComponent : ViewComponent
    {
        private readonly IFriendsService _friendsService;

        public SuggestedFriendsViewComponent(IFriendsService friendsService)
        {
            _friendsService = friendsService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var loggedInUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
            var suggestedFriends = await _friendsService.GetSuggestedFriendsAsync(int.Parse(loggedInUserId));
            return View(suggestedFriends);
        }
    }
}
