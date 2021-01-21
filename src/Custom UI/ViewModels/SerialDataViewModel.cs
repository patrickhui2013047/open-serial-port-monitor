using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Custom_UI.Messages;
using Whitestone.OpenSerialPortMonitor.SerialCommunication;

namespace Custom_UI.ViewModels
{
    //TODO:parsed data view

    public class SerialDataViewModel : PropertyChangedBase, IHandle<SerialPortConnect>, IHandle<SerialPortDisconnect>, IHandle<Autoscroll>, IHandle<SerialPortSend>
    {
        private readonly IEventAggregator _eventAggregator;
        private SerialReader _serialReader;
        private Timer _cacheTimer;
        private int _incomingRawDataCounter = 0;
        private int _outcomingRawDataCounter = 0;

        public SerialDataViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _serialReader = new SerialReader();
        }

        private bool _isIncomingViewRaw = true;
        public bool IsIncomingViewRaw
        {
            get { return _isIncomingViewRaw; }
            set
            {
                _isIncomingViewRaw = value;
                NotifyOfPropertyChange(() => IsIncomingViewRaw);
                NotifyOfPropertyChange(() => IsIncomingViewPretty);
            }
        }
        public bool IsIncomingViewPretty
        {
            get { return !_isIncomingViewRaw; }
            set
            {
                _isIncomingViewRaw = !value;
                NotifyOfPropertyChange(() => IsIncomingViewRaw);
                NotifyOfPropertyChange(() => IsIncomingViewPretty);
            }
        }

        private bool _isOutcomingViewRaw = true;
        public bool IsOutcomingViewRaw
        {
            get { return _isOutcomingViewRaw; }
            set
            {
                _isOutcomingViewRaw = value;
                NotifyOfPropertyChange(() => IsOutcomingViewRaw);
                NotifyOfPropertyChange(() => IsOutcomingViewPretty);
            }
        }
        public bool IsOutcomingViewPretty
        {
            get { return !_isOutcomingViewRaw; }
            set
            {
                _isOutcomingViewRaw = !value;
                NotifyOfPropertyChange(() => IsOutcomingViewRaw);
                NotifyOfPropertyChange(() => IsOutcomingViewPretty);
            }
        }

        private bool _isAutoscroll = true;
        public bool IsAutoscroll
        {
            get { return _isAutoscroll; }
            set
            {
                _isAutoscroll = value;
                NotifyOfPropertyChange(() => IsAutoscroll);
            }
        }
        
        private StringBuilder _incomingDataViewParsedBuilder = new StringBuilder();
        private string _incomingDataViewParsed = string.Empty;
        public string IncomingDataViewParsed
        {
            get { return _incomingDataViewParsed; }
            set
            {
                _incomingDataViewParsed = value;
                NotifyOfPropertyChange(() => IncomingDataViewParsed);
            }
        }

        private StringBuilder _incomingDataViewRawBuilder = new StringBuilder();
        private string _incomingDataViewRaw = string.Empty;
        public string IncomingDataViewRaw
        {
            get { return _incomingDataViewRaw; }
            set
            {
                _incomingDataViewRaw = value;
                NotifyOfPropertyChange(() => IncomingDataViewRaw);
            }
        }

        private StringBuilder _incomingDataViewHexBuilder = new StringBuilder();
        private string _incomingDataViewHex = string.Empty;
        public string IncomingDataViewHex
        {
            get { return _incomingDataViewHex; }
            set
            {
                _incomingDataViewHex = value;
                NotifyOfPropertyChange(() => IncomingDataViewHex);
            }
        }
        
        private StringBuilder _outcomingDataViewParsedBuilder = new StringBuilder();
        private string _outcomingDataViewParsed = string.Empty;
        public string OutcomingDataViewParsed
        {
            get { return _outcomingDataViewParsed; }
            set
            {
                _outcomingDataViewParsed = value;
                NotifyOfPropertyChange(() => OutcomingDataViewParsed);
            }
        }

        private StringBuilder _outcomingDataViewRawBuilder = new StringBuilder();
        private string _outcomingDataViewRaw = string.Empty;
        public string OutcomingDataViewRaw
        {
            get { return _outcomingDataViewRaw; }
            set
            {
                _outcomingDataViewRaw = value;
                NotifyOfPropertyChange(() => OutcomingDataViewRaw);
            }
        }

        private StringBuilder _outcomingDataViewHexBuilder = new StringBuilder();
        private string _outcomingDataViewHex = string.Empty;
        public string OutcomingDataViewHex
        {
            get { return _outcomingDataViewHex; }
            set
            {
                _outcomingDataViewHex = value;
                NotifyOfPropertyChange(() => OutcomingDataViewHex);
            }
        }

        public void IncomingRaw()
        {
            IsIncomingViewRaw = true;
        }

        public void IncomingPretty()
        {
            IsIncomingViewRaw = false;
        }

        public void Handle(SerialPortConnect message)
        {
            try
            {
                _cacheTimer = new Timer();
                _cacheTimer.Interval = 500;
                _cacheTimer.Elapsed += _cacheTimer_Elapsed;
                _cacheTimer.Start();

                _serialReader.Start(message.PortName, message.BaudRate, message.Parity, message.DataBits, message.StopBits);
                _serialReader.SerialDataReceived += SerialDataReceived;
            }
            catch (Exception ex)
            {
                _eventAggregator.PublishOnUIThread(new ConnectionError() { Exception = ex });
            }
        }

        public void Handle(SerialPortDisconnect message)
        {
            _cacheTimer.Stop();

            _serialReader.Stop();
        }

        void _cacheTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string incomingDataParsed = _incomingDataViewParsedBuilder.ToString();
            _incomingDataViewParsedBuilder = new StringBuilder();

            string incomingDataHex = _incomingDataViewHexBuilder.ToString();
            _incomingDataViewHexBuilder = new StringBuilder();

            string incomingDataRaw = _incomingDataViewRawBuilder.ToString();
            _incomingDataViewRawBuilder = new StringBuilder();

            IncomingDataViewParsed += incomingDataParsed;
            IncomingDataViewHex += incomingDataHex;
            IncomingDataViewRaw += incomingDataRaw;

            string outcomingDataParsed = _outcomingDataViewParsedBuilder.ToString();
            _outcomingDataViewParsedBuilder = new StringBuilder();

            string outcomingDataHex = _outcomingDataViewHexBuilder.ToString();
            _outcomingDataViewHexBuilder = new StringBuilder();

            string outcomingDataRaw = _outcomingDataViewRawBuilder.ToString();
            _outcomingDataViewRawBuilder = new StringBuilder();

            OutcomingDataViewParsed += outcomingDataParsed;
            OutcomingDataViewHex += outcomingDataHex;
            OutcomingDataViewRaw += outcomingDataRaw;
        }

        public void Handle(Autoscroll message)
        {
            IsAutoscroll = message.IsTurnedOn;
        }

        void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _incomingDataViewParsedBuilder.Append(System.Text.Encoding.ASCII.GetString(e.Data));

            foreach (byte data in e.Data)
            {
                _incomingRawDataCounter = _incomingRawDataCounter + 1;

                char character = (char)data;
                if (data <= 31 ||
                    data == 127)
                {
                    character = '.';
                }

                _incomingDataViewHexBuilder.Append(string.Format("{0:x2} ", data));
                _incomingDataViewRawBuilder.Append(character);

                if (_incomingRawDataCounter > 0 && _incomingRawDataCounter % 16 == 15)
                {
                    _incomingDataViewHexBuilder.Append("\r\n");
                    _incomingDataViewRawBuilder.Append("\r\n");
                    _incomingRawDataCounter = 0;
                }
            }
        }

        public void Handle(SerialPortSend message)
        {
            _serialReader.Send(message.Data);

            _outcomingDataViewParsedBuilder.Append(System.Text.Encoding.ASCII.GetString(message.Data));

            foreach (byte data in message.Data)
            {
                _outcomingRawDataCounter = _outcomingRawDataCounter + 1;

                char character = (char)data;
                if (data <= 31 ||
                    data == 127)
                {
                    character = '.';
                }

                _outcomingDataViewHexBuilder.Append(string.Format("{0:x2} ", data));
                _outcomingDataViewRawBuilder.Append(character);

                if (_outcomingRawDataCounter > 0 && _outcomingRawDataCounter % 16 == 15)
                {
                    _outcomingDataViewHexBuilder.Append("\r\n");
                    _outcomingDataViewRawBuilder.Append("\r\n");
                    _outcomingRawDataCounter = 0;
                }
            }

        }
    }
}
