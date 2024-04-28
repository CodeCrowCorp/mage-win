using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.Services.YouTubeVideoResponse
{  
    public class YouTubeVideoResponse
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public ContentDetails ContentDetails { get; set; }
        public Statistics Statistics { get; set; }
        public LiveStreamingDetails LiveStreamingDetails { get; set; }
    }

    public class Snippet
    {
        public DateTime PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public string ChannelTitle { get; set; }
        public List<string> Tags { get; set; }
        public string CategoryId { get; set; }
        public string LiveBroadcastContent { get; set; }
        public string DefaultLanguage { get; set; }
        public Localized Localized { get; set; }
    }

    public class Thumbnails
    {
        public Thumbnail Default { get; set; }
        public Thumbnail Medium { get; set; }
        public Thumbnail High { get; set; }
        public Thumbnail Standard { get; set; }
        public Thumbnail Maxres { get; set; }
    }

    public class Thumbnail
    {
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Localized
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ContentDetails
    {
        public string Duration { get; set; }
        public string Dimension { get; set; }
        public string Definition { get; set; }
        public string Caption { get; set; }
        public bool LicensedContent { get; set; }
        public object ContentRating { get; set; }
        public string Projection { get; set; }
    }

    public class Statistics
    {
        public string ViewCount { get; set; }
        public string LikeCount { get; set; }
        public string FavoriteCount { get; set; }
        public string CommentCount { get; set; }
    }

    public class LiveStreamingDetails
    {
        public DateTime ActualStartTime { get; set; }
        public DateTime ScheduledStartTime { get; set; }
        public string ConcurrentViewers { get; set; }
        public string ActiveLiveChatId { get; set; }
    }

}
