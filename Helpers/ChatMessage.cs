using Google.Apis.YouTube.v3.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

namespace MageWin.Helpers
{
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _prefix;
        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (_prefix != value)
                {
                    _prefix = value;
                    OnPropertyChanged();
                }
            }
        }

        private SolidColorBrush _prefixColor;
        public SolidColorBrush PrefixColor
        {
            get { return _prefixColor; }
            set
            {
                if (_prefixColor != value)
                {
                    _prefixColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        private SolidColorBrush _messageColor;
        public SolidColorBrush MessageColor
        {
            get { return _messageColor; }
            set
            {
                if (_messageColor != value)
                {
                    _messageColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _iconPath;
        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                if (_iconPath != value)
                {
                    _iconPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private Visibility _imageVisibility;
        public Visibility ImageVisibility
        {
            get { return _imageVisibility; }
            set
            {
                if (_imageVisibility != value)
                {
                    _imageVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _tagText;
        public string TagText
        {
            get { return _tagText; }
            set
            {
                if (_tagText != value)
                {
                    _tagText = value;
                    OnPropertyChanged();
                }
            }
        }

        private SolidColorBrush _tagColor;
        public SolidColorBrush TagColor
        {
            get { return _tagColor; }
            set
            {
                if (_tagColor != value)
                {
                    _tagColor = value;
                    OnPropertyChanged();
                }
            }
        }
        private BitmapImage? _svgImage;
        public BitmapImage? SvgImage
        {
            get { return _svgImage; }
            set
            {
                if (_svgImage != value)
                {
                    _svgImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChatMessage(string prefix,
            string message,
            SolidColorBrush prefixColor,
            SolidColorBrush messageColor,
            SolidColorBrush tagColor,
            string tagText,
            string iconPath = null,
            Visibility imageVisibility = Visibility.Visible)
        {
            iconPath = "https://imagedelivery.net/815v_avUAZL5R0XoDZG3pw/5355871f-6a19-4fee-5d92-8940e1bc6600/public";
            Prefix = prefix;
            Message = message;
            PrefixColor = prefixColor;
            MessageColor = messageColor;          
            ImageVisibility = !string.IsNullOrWhiteSpace(iconPath) ? imageVisibility : Visibility.Collapsed;
            TagColor = tagColor;
            TagText = tagText;
            SvgImage = GetBitmapImageFromSvgUrl(iconPath);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ReloadMessage(string newMessage)
        {
            this.Message = string.Empty;
            this.Message = newMessage;
        }

        public BitmapImage? GetBitmapImageFromSvgUrl(string path) {
            
            try {
                if (string.IsNullOrWhiteSpace(path))
                    return default;

                var quality = 100;
                var svg = new SKSvg();
                var pict = svg.Load(LoadSvgStreamFromUrl(path));
                var dimen = new SkiaSharp.SKSizeI(
                    (int)Math.Ceiling(pict.CullRect.Width),
                    (int)Math.Ceiling(pict.CullRect.Height)
                );

                var matrix = SKMatrix.CreateScale(1, 1);
                var img = SKImage.FromPicture(pict, dimen, matrix);
                var skdata = img.Encode(SkiaSharp.SKEncodedImageFormat.Png, quality);
                var image = new BitmapImage();
                image.SetSource(skdata.AsStream().AsRandomAccessStream());
                return image ?? default;
            }
            catch (Exception) {
                return default;
            }
        }


        public Stream LoadSvgStreamFromUrl(string url)
        {
            using (var client = new HttpClient())
            {

                byte[] svgBytes = client.GetByteArrayAsync(url).Result;
                return new MemoryStream(svgBytes);
            }
        }

    }
}
