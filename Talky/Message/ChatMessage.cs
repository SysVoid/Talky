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
            _client.Channel.BroadcastMessage($"[{_client.Username}] {actualMessage}");
        }

    }
}
