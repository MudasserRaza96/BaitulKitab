namespace Baitul_Kitab.Ads
{
    /// <summary>Bound from the "Ads" section of appsettings.json.</summary>
    public class AdSettings
    {
        public const string SectionName = "Ads";

        /// <summary>Global switch. When false, no ad markup is rendered anywhere.</summary>
        public bool Enabled { get; set; }

        public string Provider { get; set; } = "AdSense";

        /// <summary>Provider client id, e.g. "ca-pub-XXXXXXXXXXXXXXXX".</summary>
        public string? PublisherId { get; set; }

        /// <summary>Per-placement settings keyed by <see cref="AdPlacements"/> names.</summary>
        public Dictionary<string, AdPlacementSettings> Placements { get; set; } = new();
    }

    public class AdPlacementSettings
    {
        public bool Enabled { get; set; } = true;
        public string? SlotId { get; set; }
        public string Format { get; set; } = "auto";

        /// <summary>Only used by BookListing: show the ad after every N books.</summary>
        public int Interval { get; set; } = 6;
    }

    /// <summary>Known placement names.</summary>
    public static class AdPlacements
    {
        public const string TopBanner = "TopBanner";
        public const string BookListing = "BookListing";
        public const string Sidebar = "Sidebar";
        public const string BookDetails = "BookDetails";
        public const string Footer = "Footer";
    }

    /// <summary>Everything a renderer needs for one ad slot.</summary>
    public record AdPlacementModel(string Placement, string PublisherId, string SlotId, string Format);
}
