using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Utils
{
    public  class Util
    {
        public static string GenerateRandomARGBHex()
        {
            byte[] buffer = new byte[4];
            new Random().NextBytes(buffer);
            string hex = $"#{buffer[0]:X2}{buffer[1]:X2}{buffer[2]:X2}{buffer[3]:X2}";
            return hex;
        }

    }
}
