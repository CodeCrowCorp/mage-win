using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Utils
{
    public static class Util
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static string GenerateRandomARGBHex()
        {
            byte[] buffer = new byte[4];
            new Random().NextBytes(buffer);
            string hex = $"#{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2}";
            return hex;
        }


        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

    }
}
