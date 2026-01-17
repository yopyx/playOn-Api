using static Shared.DataTypes;

namespace playOn.Contracts
{
    public class LiveChat
    {
        public string Kind { get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public int PollingIntervalMillis { get; set; }
        public PageInfo PageInfo { get; set; } = new();
        public string? NextPageToken { get; set; }
        public string? PrevPageToken { get; set; }
        public LiveCommentFromApi[] Items { get; set; } = [];
    }
}
