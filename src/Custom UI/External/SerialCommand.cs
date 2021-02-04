using Custom_UI.Framework;
using System;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Custom_UI.External
{
    [JsonObject]
    public class SerialCommand
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonIgnore]
        private string _description = string.Empty;
        [JsonProperty]
        public string Description 
        {
            get { return _description; }
            set { _description = value; }
        }
        [JsonIgnore]
        private string _commandPatten = string.Empty;
        [JsonProperty]
        public string CommandPatten 
        {
            get { return _commandPatten; }
            set { _commandPatten = value; }
        }
        [JsonIgnore]
        public string Command
        {
            get
            {
                if (Key != null && Key.Length > 0)
                {
                    return string.Format(CommandPatten, _key);
                }
                else if( CommandPatten!=null)
                {
                    return CommandPatten;
                }
                else
                {
                    return string.Empty;
                }

            }
        }
        [JsonIgnore]
        private object[] _key;
        [JsonProperty]
        public object[] Key
        {
            get
            {
                return _key; 
            }
            set { _key = value; }
        }
        [JsonProperty]
        public bool IsHex { get; set; }
        [JsonIgnore]
        public int ID { get; set; }

        //The following code is not for data storage, Don't change anything
        [JsonIgnore]
        private Func<SerialCommand, bool> _sendHandler;

        public void ApplySendEvent(Func<SerialCommand,bool> handler)
        {
            _sendHandler = handler;
        }

        public void CommandSend()
        {
            if (_sendHandler != null) { _sendHandler.Invoke(this); }
        }
        [JsonIgnore]
        private ICommand _send;
        [JsonIgnore]
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
        [JsonIgnore]
        private Func<SerialCommand, bool> _editHandler;

        public void ApplyEditEvent(Func<SerialCommand, bool> handler)
        {
            _editHandler = handler;
        }

        public void CommandEdit()
        {
            if (_editHandler != null) { _editHandler.Invoke(this); }
        }

        [JsonIgnore]
        private ICommand _edit;
        [JsonIgnore]
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