using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models
{
    public class ChannelModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Category { get; set; }
        public List<string> Bans { get; set; }
        public List<string> Mods { get; set; }
        public List<string> Guests { get; set; }
        public int MemberCount { get; set; }
        public int IsLive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int IsAiChatEnabled { get; set; }
        public int? DiscordThreadId { get; set; }
        public string Avatar { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int PlanTier { get; set; }
        public int ViewCount { get; set; }
    }
}
