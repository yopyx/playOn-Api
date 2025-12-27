
using static Shared.DataTypes;

namespace playOn.Contracts
{
    public class Channels
    {
        public string Kind { get; set; } = string.Empty;
        public string Etag { get; set; } = string.Empty;
        public PageInfo PageInfo { get; set; } = new();
        public ChannelFromApi[] Items { get; set; } = [];

    }
}