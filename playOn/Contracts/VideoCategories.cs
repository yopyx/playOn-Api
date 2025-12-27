using static Shared.DataTypes;

namespace playOn.Contracts
{
    public class VideoCategories
    {
        public string Kind { get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public List<VideoCategoryFromApi> Items { get; set; } = [];
    }
}
