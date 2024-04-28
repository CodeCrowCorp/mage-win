using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.Services.YouTubeLiveChatMessageListResponse
{

    public class YouTubeLiveChatMessageListResponse
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public int PollingIntervalMillis { get; set; }
        public PageInfo PageInfo { get; set; }
        public string NextPageToken { get; set; }
        public List<Item> Items { get; set; }
    }
    public class PageInfo
    {
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
    }

    public class Item
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string Id { get; set; }
        public Snippet Snippet { get; set; }
        public AuthorDetails AuthorDetails { get; set; }
    }

    public class Snippet
    {
        public string Type { get; set; }
        public string LiveChatId { get; set; }
        public string AuthorChannelId { get; set; }
        public DateTime PublishedAt { get; set; }
        public bool HasDisplayContent { get; set; }
        public string DisplayMessage { get; set; }
        public TextMessageDetails TextMessageDetails { get; set; }
    }

    public class TextMessageDetails
    {
        public string MessageText { get; set; }
    }

    public class AuthorDetails
    {
        public string ChannelId { get; set; }
        public string ChannelUrl { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool IsVerified { get; set; }
        public bool IsChatOwner { get; set; }
        public bool IsChatSponsor { get; set; }
        public bool IsChatModerator { get; set; }
    }
}
