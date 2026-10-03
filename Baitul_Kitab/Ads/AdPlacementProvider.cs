using Microsoft.Extensions.Options;

namespace Baitul_Kitab.Ads
{
    public interface IAdPlacementProvider
    {
        /// <summary>Returns render data, or null when the placement must not render.</summary>
        AdPlacementModel? Get(string placement);

        bool IsEnabled(string placement);

        /// <summary>Configured interval for in-list placements (minimum 1).</summary>
        int GetInterval(string placement, int fallback = 6);
    }

    public class AdPlacementProvider : IAdPlacementProvider
    {
        private readonly IOptionsMonitor<AdSettings> _options;

        public AdPlacementProvider(IOptionsMonitor<AdSettings> options)
        {
            _options = options;
        }

        public AdPlacementModel? Get(string placement)
        {
            var settings = _options.CurrentValue;
            if (!settings.Enabled || string.IsNullOrWhiteSpace(settings.PublisherId))
                return null;
            if (!settings.Placements.TryGetValue(placement, out var p) || !p.Enabled || string.IsNullOrWhiteSpace(p.SlotId))
                return null;

            return new AdPlacementModel(placement, settings.PublisherId.Trim(), p.SlotId.Trim(), p.Format);
        }

        public bool IsEnabled(string placement) => Get(placement) != null;

        public int GetInterval(string placement, int fallback = 6)
        {
            var interval = _options.CurrentValue.Placements.TryGetValue(placement, out var p) ? p.Interval : fallback;
            return interval < 1 ? fallback : interval;
        }
    }
}
