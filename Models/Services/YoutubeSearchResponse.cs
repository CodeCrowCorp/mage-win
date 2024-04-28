using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.Services.YouTubeSearchResponse
{
    public class YouTubeSearchResponse
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string RegionCode { get; set; }
        public PageInfo PageInfo { get; set; }
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
        public Id Id { get; set; }
    }

    public class Id
    {
        public string Kind { get; set; }
        public string VideoId { get; set; }
    }
}
