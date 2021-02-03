using Custom_UI.Framework;
using System;
using System.Windows.Input;

namespace Custom_UI.External
{
    public class SerialCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string CommandPatten = string.Empty;
        public string Command
        {
            get
            {
                if (Key.Length > 0 || Key == null)
                {
                    return string.Format(CommandPatten, _key);
                }
                else
                {
                    return CommandPatten;
                }

            }
            set
            {
                CommandPatten = value;
            }
        }

        private object[] _key;
        public object[] Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public bool IsHex { get; set; }

        private Func<SerialCommand, bool> _sendHandler;
        public void ApplySendEvent(Func<SerialCommand,bool> handler)
        {
            _sendHandler = handler;
        }

        public void CommandSend()
        {
            if (_sendHandler != null) { _sendHandler.Invoke(this); }
        }
        private ICommand _send;
        public ICommand Send
        {
            get
            {
                if (_send == null)
                {
                    _send = new RelayCommand(CommandSend);
                }
                return _send;
            }
        }

        private Func<SerialCommand, bool> _editHandler;
        public void ApplyEditEvent(Func<SerialCommand, bool> handler)
        {
            _editHandler = handler;
        }

        public void CommandEdit()
        {
            if (_editHandler != null) { _editHandler.Invoke(this); }
        }


        private ICommand _edit;
        public ICommand Edit
        {
            get
            {
                if (_edit == null)
                {
                    _edit = new RelayCommand(CommandEdit);
                }
                return _edit;
            }
        }

    }
}