using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_UI.ViewModels;
using Custom_UI.Framework;
using Custom_UI.Messages;
using System.Windows;

namespace Custom_UI.ViewModels
{
    [Export(typeof(IShell))]
    class MainViewModel : Screen, IShell
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        private bool _isAutoscroll = true;

        private SerialConnectorViewModel _serialConnectorView;
        public SerialConnectorViewModel SerialConnectorView
        {
            get
            {
                return _serialConnectorView;
            }
            set
            {
                _serialConnectorView = value;
                NotifyOfPropertyChange(() => SerialConnectorView);
            }
        }

        private SerialDataViewModel _serialDataView;
        public SerialDataViewModel SerialDataView
        {
            get
            {
                return _serialDataView;
            }
            set
            {
                _serialDataView = value;
                NotifyOfPropertyChange(() => SerialDataView);
            }
        }

        private SerialDataSendViewModel _serialDataSendView;
        public SerialDataSendViewModel SerialDataSendView
        {
            get
            {
                return _serialDataSendView;
            }
            set
            {
                _serialDataSendView = value;
                NotifyOfPropertyChange(() => SerialDataSendView);
            }
        }

        [ImportingConstructor]
        public MainViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;

            base.DisplayName = "Serial Port Monitor";// +Assembly.GetExecutingAssembly().GetName().Version;
        }

        protected override void OnInitialize()
        {
            SerialConnectorView = new SerialConnectorViewModel(_eventAggregator);
            SerialDataView = new SerialDataViewModel(_eventAggregator);
            SerialDataSendView = new SerialDataSendViewModel(_eventAggregator);
            base.OnInitialize();
        }

        public void FileExit()
        {
            Application.Current.Shutdown();
        }

        public void Autoscroll()
        {
            _isAutoscroll = !_isAutoscroll;
            _eventAggregator.PublishOnUIThread(new Autoscroll { IsTurnedOn = _isAutoscroll });
        }

        public void OpenAbout()
        {
            _windowManager.ShowDialog(new AboutViewModel());
        }
    }
}
