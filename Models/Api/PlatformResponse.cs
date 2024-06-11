using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Models.Api.PlatformInfoResponse
{
    public class Platform
    {
        public int Count { get; set; }
        public string Name { get; set; }
    }

    public class PlatformInfoResponse
    {
        public List<Platform> PlatformInfos { get; set; }
    }
}
