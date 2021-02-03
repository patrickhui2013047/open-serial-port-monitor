using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Custom_UI.External;
using Custom_UI.Messages;

namespace Custom_UI.ViewModels
{
    
    public class SerialCommandDetailViewModel: Screen, IHandle<SerialCommandUpdate>
    {
        private readonly IEventAggregator _eventAggregator;

        private SerialCommand _command;
        public SerialCommand Command 
        {
            get { return _command; }
            set { _command = value; }
        }

        private SerialCommandOperation _operation;
        public SerialCommandOperation Operation
        {
            get { return _operation; }
            set { _operation = value;}
        }
        public string Name
        {
            get { return Command.Name; }
            set { Command.Name = value; }
        }
        public string Description
        {
            get { return Command.Description; }
            set { Command.Description = value; }
        }
        public string CommandPatten
        {
            get { return Command.CommandPatten; }
            set { Command.CommandPatten = value; }
        }
        public object[] Key
        {
            get { return Command.Key; }
            set { Command.Key = value; }
        }
        public string KeyFormated
        {
            get 
            {
                string[] formated = new string[Command.Key.Length];
                for(int i = 0; i < Command.Key.Length; i++)
                {
                    formated[i] = Command.Key[i].ToString();
                }
                return string.Join("\n", formated);
            }
            set { Command.Key = value.Split('\n'); }
        }
        public bool IsHex
        {
            get { return Command.IsHex; }
            set { Command.IsHex = value; }
        }
        public bool IsCreate
        {
            get { return Operation == SerialCommandOperation.Create; }
        }

        public SerialCommandDetailViewModel(IEventAggregator eventAggregator,SerialCommandUpdate update= null)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            if (update == null)
            {
                update = SerialCommandUpdate.New();
            }
            Command = update.Command;
            Operation = update.Operation;
            switch (Operation)
            {
                case SerialCommandOperation.Create:
                    Name = "NewCommand";
                    break;
                case SerialCommandOperation.Update:
                    break;
                default:
                    throw new ArgumentException("Operation not support");
            }

        }

        public void Send()
        {
            _eventAggregator.BeginPublishOnUIThread(new SerialCommandUpdate() { Command = Command,Operation=SerialCommandOperation.Send });
            Close();
        }
        public void Save()
        {
            if (IsCreate)
            {
                _eventAggregator.BeginPublishOnUIThread(new SerialCommandUpdate() { Command = Command, Operation = SerialCommandOperation.Create });
            }
            else
            {
                _eventAggregator.BeginPublishOnUIThread(new SerialCommandUpdate() { Command = Command, Operation = SerialCommandOperation.Update });
            }
            Close();
        }
        public void Delete()
        {
            _eventAggregator.BeginPublishOnUIThread(new SerialCommandUpdate() { Command = Command, Operation = SerialCommandOperation.Remove });
            Close();
        }
        public void Cancel()
        {
            if(IsCreate)
            {
                _eventAggregator.BeginPublishOnUIThread(new SerialCommandUpdate() { Command = Command, Operation = SerialCommandOperation.Remove });
            }
            else
            {
                
            }
            Close();
        }
        private void Close()
        {
            this.TryClose(true);
        }


        public void Handle(SerialCommandUpdate message)
        {
            switch (message.Operation )
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
