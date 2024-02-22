using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public ChatMessage(string prefix, string message, SolidColorBrush prefixColor, SolidColorBrush messageColor)
        {
            Prefix = prefix;
            Message = message;
            PrefixColor = prefixColor;
            MessageColor = messageColor;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
