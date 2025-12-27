using static Shared.DataTypes;

namespace playOn.Contracts
{
    public class PageInfo
    {
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
    }

    public class VideosList
    {
        public string Kind { get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public VideoDataFromApi[] Items { get; set; } = [];
        public string? NextPageToken { get; set; } = string.Empty;
        public string? PrevPageToken { get; set; } = string.Empty;
        public PageInfo PageInfo { get; set; } = new();
    }
}
