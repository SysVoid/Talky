using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;

namespace Talky.Message
{
    abstract class ServerMessage
    {

        protected ServerClient _client;
        protected string _message;
        protected DateTime _time;

        protected ServerMessage(ServerClient client, string message)
        {
            _client = client;
            _message = message;
            _time = DateTime.Now;
        }

        public abstract bool Valid();

        public void Handle()
        {
            if (new ChatMessage(_client, _message).Valid())
            {
                if (_client.Channel == null)
                {
                    _client.SendMessage("§2You are not in a channel. Use /clist to see a list of channels.");
                    return;
                } else if (_client.Muted)
                {
                    _client.SendMessage("§2You are muted.");
                    return;
                } else if (_client.Username.Equals("%"))
                {
                    _client.SendMessage("§1Please set your username with: /name <username>");
                    return;
                }
            }
            Process();
        }

        protected abstract void Process();

    }
}
