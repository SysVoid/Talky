using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;

namespace Talky.Message
{
    class ChatMessage : ServerMessage
    {

        public ChatMessage(ServerClient client, string message) : base(client, message) { }

        public override bool Valid()
        {
            return (_message.StartsWith("M:") && !_message.StartsWith("M:/"));
        }

        protected override void Process()
        {
            string actualMessage = _message.Substring(2);
            if (actualMessage.StartsWith(" "))
            {
                actualMessage = actualMessage.Substring(1);
            }

            while (actualMessage.EndsWith(" "))
            {
                actualMessage = actualMessage.Substring(0, actualMessage.Length - 1);
            }

            if (actualMessage.StartsWith("register") || actualMessage.StartsWith("auth"))
            {
                _client.SendMessage("To protect our users, we do not send messages starting with 'register' or 'auth'. This is in case they miss out the / on the register or auth commands.");
                return;
            }

            _client.Channel.BroadcastMessage($"<{(_client.Account != null && _client.Account.Role.Equals("admin") ? "%" : "")}{_client.Username}> {actualMessage}");
        }

    }
}
