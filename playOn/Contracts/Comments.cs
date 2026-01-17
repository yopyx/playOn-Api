using DataLayer.EfClasses;
using static Shared.DataTypes;

namespace playOn.Contracts
{
    public class Comments
    {
        public string Kind { get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public string? NextPageToken { get; set; }
        public PageInfo PageInfo { get; set; } = new();
        public CommentFromApi[] Items { get; set; } = [];
    }
}