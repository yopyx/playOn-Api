namespace Shared
{
    public class DataTypes
    {
        public class ThumbnailDetails
        {
            public string Url { get; set; } = string.Empty;
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class Statistics
        {
            public string ViewCount { get; set; } = string.Empty;
            public string SubscriberCount { get; set; } = string.Empty;
            public bool HiddenSubscriberCount { get; set; }
            public string VideoCount { get; set; } = string.Empty;
        }

        // Video Category
        public class Snippet
        {
            public string Title { get; set; } = string.Empty;
            public bool? Assignable { get; set; }
            public string? ChannelId { get; set; }
        }
        public class VideoCategoryFromApi
        {
            public string? Kind { get; set; }
            public string? Etag { get; set; }
            public string Id { get; set; } = string.Empty;
            public Snippet Snippet { get; set; } = new();
        }


        // Comment
        public class Snippet5
        {
            public string ChannelId { get; set; } = string.Empty;
            public string VideoId { get; set; } = string.Empty;
            public string TextDisplay { get; set; } = string.Empty;
            public string TextOriginal { get; set; } = string.Empty;
            public string AuthorDisplayName { get; set; } = string.Empty;
            public string AuthorProfileImageUrl { get; set; } = string.Empty;
            public string AuthorChannelUrl { get; set; } = string.Empty;
            public Dictionary<string, string> AuthorChannelId { get; set; } = [];
            public bool CanRate { get; set; }
            public string ViewerRating { get; set; } = string.Empty;
            public int LikeCount { get; set; }
            public string PublishedAt { get; set; } = string.Empty;
            public string UpdatedAt { get; set; } = string.Empty;
            public string? ParentId { get; set; }
            public string? TotalReplyCount { get; set; }
        }
        public class TopLevelComment
        {
            public string Kind { get; set; } = string.Empty;
            public string Etag { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public Snippet5 Snippet { get; set; } = new();

        }
        public class Snippet4
        {
            public string ChannelId { get; set; } = string.Empty;
            public string VideoId { get; set; } = string.Empty;
            public TopLevelComment TopLevelComment { get; set; } = new();
            public bool CanReply { get; set; }
            public int TotalReplyCount { get; set; }
            public bool IsPublic { get; set; }
        }
        public class NestedComment
        {
            public string Kind { get; set; } = string.Empty;
            public string Etag { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public Snippet5 Snippet { get; set; } = new();
            public Replies? Replies { get; set; }
        }
        public class Replies {
            public NestedComment[] Comments { get; set; } = [];
        }
        public class CommentFromApi
        {
            public string Kind { get; set; } = string.Empty;
            public string Etag { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public Snippet4 Snippet { get; set; } = new();
            public Replies? Replies { get; set; }
        }

        //channels
        public class Snippet6
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string CustomUrl { get; set; } = string.Empty;
            public string PublishedAt { get; set; } = string.Empty;
            public Dictionary<string, ThumbnailDetails> Thumbnails { get; set; } = [];
            public Dictionary<string, string> Localized { get; set; } = [];
            public string Country { get; set; } = string.Empty;
        }
        public class ChannelFromApi
        {
            public string Kind { get; set; } = string.Empty;
            public string Etag { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public Snippet6 Snippet { get; set; } = new();
            public Statistics Statistics { get; set; } = new();
        }


        // videos
        public class Snippet2
        {
            public string PublishedAt { get; set; } = string.Empty;
            public string ChannelId { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public Dictionary<string, ThumbnailDetails> Thumbnails { get; set; } = [];
            public string ChannelTitle { get; set; } = string.Empty;
            public string[]? Tags { get; set; } = [];
            public string CategoryId { get; set; } = string.Empty;
            public string LiveBroadcastContent { get; set; } = string.Empty;
            public string DefaultLanguage { get; set; } = string.Empty;
            public Dictionary<string, string> Localized { get; set; } = [];
            public string DefaultAudioLanguage { get; set; } = string.Empty;
        }

        public class ContentDetails
        {
            public string Duration { get; set; } = string.Empty;
            public string Dimension { get; set; } = string.Empty;
            public string Definition { get; set; } = string.Empty;
            public string Caption { get; set; } = string.Empty;
            public bool LicensedContent { get; set; }
            public Object ContentRating { get; set; } = new();
            public string Projection { get; set; } = string.Empty;
        }

        public class VideoDataFromApi
        {
            public string Kind { get; set; } = string.Empty;
            public string Etag { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public Snippet2 Snippet { get; set; } = new();
            public ContentDetails ContentDetails { get; set; } = new();
            public Dictionary<string, string> Statistics { get; set; } = [];
            public Dictionary<string, string>? LiveStreamingDetails { get; set; }
        }



        //live chat

        public class Snippet7
        {
            public string Type { get; set; } = string.Empty;
            public string LiveChatId { get; set; } = string.Empty;
            public string AuthorChannelId { get; set; } = string.Empty;
            public string PublishedAt { get; set; } = string.Empty;
            public bool HasDisplayContent { get; set; }
            public string DisplayMessage { get; set; } = string.Empty;
        }
        public class AuthorDetails
        {
            public string ChannelId { get; set; } = string.Empty;
            public string ChannelUrl { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public string ProfileImageUrl { get; set; } = string.Empty;
            public bool IsVerified { get; set; }
            public bool IsChatOwner { get; set; }
            public bool IsChatSponsor { get; set; }
            public bool IsChatModerator { get; set; }


        }
        public class LiveCommentFromApi
        {
            public string Kind { get; set; } = string.Empty;
            public string Etag { get; set; } = string.Empty;
            public string Id { get; set; } = string.Empty;
            public Snippet7 Snippet { get; set; } = new();
            public AuthorDetails AuthorDetails { get; set; } = new();
        }
    }
}
