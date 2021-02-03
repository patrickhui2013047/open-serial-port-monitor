using System;
using System.IO;
using System.Collections.Generic;
using Caliburn.Micro;
using Custom_UI.Framework;
using Custom_UI.Messages;
using Custom_UI.External;
using Newtonsoft.Json;
using System.Windows.Input;
using System.Linq;

namespace Custom_UI.ViewModels
{
    public class SerialCommandViewModel : PropertyChangedBase, IHandle<SerialPortConnect>, IHandle<SerialPortDisconnect>, IHandle<ConnectionError>, IHandle<SerialCommandUpdate>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private SerialCommandDetailViewModel _serialCommandDetail;

        private string _dataToSend = string.Empty;
        public string DataToSend
        {
            get { return _dataToSend; }
            set
            {
                _dataToSend = value;
                NotifyOfPropertyChange(() => DataToSend);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }

        private bool _isText = true;
        public bool IsText
        {
            get { return _isText; }
            set
            {
                _isText = value;
                NotifyOfPropertyChange(() => IsText);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }

        private bool _isHex = false;
        public bool IsHex
        {
            get { return _isHex; }
            set
            {
                _isHex = value;
                NotifyOfPropertyChange(() => IsHex);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }
        public bool IsValidData
        {
            get
            {
                if (!IsConnected)
                {
                    return false;
                }

                if (DataToSend.Length <= 0)
                {
                    return false;
                }

                if (IsText) // Left this here for readability even though it is not needed. Otherwise someone might think it was missing or forgotten about.
                {
                }

                if (IsHex)
                {
                    try
                    {
                        string[] characters = DataToSend.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string hex in characters)
                        {
                            byte value = Convert.ToByte(hex, 16);
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }

        private string _filter = string.Empty;
        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                NotifyOfPropertyChange(() => Filter);
                NotifyOfPropertyChange(() => CommandListSource);
            }
        }

        private List<SerialCommand> _commandListSource = new List<SerialCommand>();
        public List<SerialCommand> CommandListSource
        {
            get 
            {
                if (string.IsNullOrEmpty(Filter) || string.IsNullOrWhiteSpace(Filter))
                {
                    return _commandListSource;
                }
                return _commandListSource.Where(obj => obj.Name.ToLower().StartsWith(Filter)|| obj.Command.ToLower().StartsWith(Filter)).ToList();
            }
            set
            {
                _commandListSource = value;
                _commandListSource.ForEach(obj => obj.ApplySendEvent(SendButtonHandle));
                _commandListSource.ForEach(obj => obj.ApplyEditEvent(EditButtonHandle));
                NotifyOfPropertyChange(() => CommandListSource);
                NotifyOfPropertyChange(() => IsConnected);
                NotifyOfPropertyChange(() => IsValidData);
            }
        }



        public SerialCommandViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _windowManager = windowManager;

            using (StreamReader file = File.OpenText("Commands.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                CommandListSource = JsonConvert.DeserializeObject<List<SerialCommand>>(file.ReadToEnd());
                _ = 1;
            }
        }

        public void DoSend()
        {
            List<byte> data = new List<byte>();

            if (IsHex)
            {
                string[] characters = DataToSend.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string hex in characters)
                {
                    byte value = Convert.ToByte(hex, 16);
                    data.Add(value);
                }
            }

            if (IsText)
            {
                string parsed = DataToSend.Replace("\\\\r", "\r").Replace("\\\\n", "\n");
                data.AddRange(System.Text.Encoding.ASCII.GetBytes(parsed));
            }

            _eventAggregator.PublishOnUIThread(new SerialPortSend() { Data = data.ToArray() });
        }


        public void Add()
        {
            var command = new SerialCommand();
            _serialCommandDetail = new SerialCommandDetailViewModel(_eventAggregator, new SerialCommandUpdate() { Command = command, Operation = SerialCommandOperation.Create });
            _windowManager.ShowDialog(_serialCommandDetail);
        }

        private bool SendButtonHandle(SerialCommand command)
        {
            DataToSend = command.Command;
            IsText = !command.IsHex;
            IsHex = command.IsHex;
            DoSend();
            return true;
        }

        private bool EditButtonHandle(SerialCommand command)
        {
            //TODO
            _serialCommandDetail = new SerialCommandDetailViewModel(_eventAggregator, new SerialCommandUpdate() { Command = command, Operation = SerialCommandOperation.Update });
            _windowManager.ShowDialog(_serialCommandDetail);

            return true;
        }
        public void Handle(SerialPortConnect message)
        {
            IsConnected = true;
        }

        public void Handle(SerialPortDisconnect message)
        {
            IsConnected = false;
        }

        public void Handle(ConnectionError message)
        {
            IsConnected = false;
        }

        public void Handle(SerialCommandUpdate message)
        {
            switch (message.Operation)
            {
                case SerialCommandOperation.Create:
                    break;
                case SerialCommandOperation.Remove:
                    break;
                case SerialCommandOperation.Update:
                    break;
                case SerialCommandOperation.Send:
                    break;
                default:

                    break;
            }
        }
    }
}