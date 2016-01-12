using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Channel;

namespace Talky.Message
{
    class StatMessage : ServerMessage
    {

        public StatMessage(ServerClient client, string message) : base(client, message) { }

        public override bool Valid()
        {
            return _message.StartsWith("S:");
        }

        protected override void Process()
        {
            string actualMessage = _message.Substring(2);

            if (actualMessage.Equals("ChannelList"))
            {
                string channels = "";

                IReadOnlyList<ServerChannel> serverChannels = (IReadOnlyList<ServerChannel>) ChannelRepository.Instance.All();
                int max = serverChannels.Count;
                
                for (int i = 0; i < max; i++)
                {
                    if (i != max - 1)
                    {
                        channels += serverChannels[i].Name + ";";
                    } else
                    {
                        channels += serverChannels[i].Name;
                    }
                }

                _client.SendRawMessage("S:ChannelList:" + channels);
            } else if (actualMessage.Equals("Client"))
            {
                _client.SendRawMessage($"S:Client:{(_client.Username.Equals("%") ? "N/A" : _client.Username)};{_client.Muted.ToString()};{(_client.Channel == null ? "N/A" : _client.Channel.Name)}");
            } else if (actualMessage.Equals("Account"))
            {
                if (_client.Account != null)
                {
                    _client.SendRawMessage($"S:Account:{_client.Account.AccountId};{_client.Account.Username};{_client.Account.Role}");
                }
            }
        }

    }
}
