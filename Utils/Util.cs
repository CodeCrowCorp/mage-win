using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

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
        public static string RGBToHex(int r, int g, int b)
        {
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        public static string RGBStringToHex(string rgb)
        {
            string cleanedRGB = new string(rgb.Where(char.IsDigit).ToArray());

            int r = int.Parse(cleanedRGB.Substring(0, 3));
            int g = int.Parse(cleanedRGB.Substring(3, 3));
            int b = int.Parse(cleanedRGB.Substring(6, 3));

            return $"#{r:X2}{g:X2}{b:X2}";
        }


        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - UnixEpoch).TotalSeconds;
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
           
            return default(T);
        }

        public static string GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }

        public static string ColorToHex(Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
