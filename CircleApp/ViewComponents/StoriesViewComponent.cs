using CircleApp.Data;
using CircleApp.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        private readonly IStoriesServices _storiesService;

        public StoriesViewComponent(IStoriesServices storiesService)
        {
            _storiesService = storiesService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _storiesService.GetAllStoriesAsync();
            return View(allStories);
        }
    }
}
