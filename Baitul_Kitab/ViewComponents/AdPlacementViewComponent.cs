using Baitul_Kitab.Ads;
using Microsoft.AspNetCore.Mvc;

namespace Baitul_Kitab.ViewComponents
{
    /// <summary>
    /// Usage: @await Component.InvokeAsync("AdPlacement", new { placement = AdPlacements.TopBanner })
    /// Renders nothing unless the placement is enabled and fully configured.
    /// </summary>
    public class AdPlacementViewComponent : ViewComponent
    {
        private readonly IAdPlacementProvider _ads;

        public AdPlacementViewComponent(IAdPlacementProvider ads)
        {
            _ads = ads;
        }

        public IViewComponentResult Invoke(string placement)
        {
            var model = _ads.Get(placement);
            if (model == null)
                return Content(string.Empty);

            return View(model);
        }
    }
}
