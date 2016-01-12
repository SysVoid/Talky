using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Command;

namespace Talky.Message
{
    class CommandMessage : ChatMessage
    {

        public CommandMessage(ServerClient client, string message) : base(client, message) { }

        public override bool Valid()
        {
            return _message.StartsWith("M:/");
        }

        protected override void Process()
        {
            string[] split = _message.Substring(3).Split(new char[] { ' ' });
            string command = split[0].ToLower();
            string[] args = new string[split.Length - 1];

            if (args.Length > 0)
            {
                args = _message.Substring(4 + command.Length).Split(new char[] { ' ' });
            }

            if (_client.Channel == null && !(command.Equals("name") || command.Equals("auth")))
            {
                _client.SendMessage("Please use /name <name> to set a username before using commands.");
                _client.SendMessage("If you are a registered client, please use /auth to claim your username.");
                return;
            }

            TalkyCommand theCommand = CommandManager.Instance.Get(command);
            if (theCommand == null)
            {
                _client.SendMessage("That command does not exist.");
                return;
            }
            theCommand.Execute(_client, args);
        }

    }
}
