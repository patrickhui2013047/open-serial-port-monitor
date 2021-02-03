using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_UI.External;

namespace Custom_UI.Messages
{
    public class SerialCommandUpdate
    {
        public SerialCommand Command { get; set; }
        public SerialCommandOperation Operation { get; set; }
        public int CommandCount { get; set; }

        public static SerialCommandUpdate New()
        {
            return new SerialCommandUpdate() { Command = new SerialCommand(), Operation = SerialCommandOperation.Create };
        }
    }

    public enum SerialCommandOperation
    {
        Create,
        Remove,
        Update,
        Send
    }
}
