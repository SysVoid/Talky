using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;

namespace Talky.Channel
{
    abstract class TalkyChannel
    {

        public string Name { get; private set; }
        public bool Locked { get; set; }

        protected TalkyChannel(string name, bool locked)
        {
            if (!name.StartsWith("+"))
            {
                name = "+" + name;
            }

            Name = name.Replace(";", "-").Replace(":", "-").ToLower();
            Locked = locked;
        }

        public void Kick(ServerClient client, string reason = null)
        {
            if (!(this is LobbyChannel))
            {
                LobbyChannel lobby = ChannelRepository.Instance.GetLobby();
                if (lobby == null)
                {
                    return;
                }

                if (client.Channel.Equals(lobby))
                {
                    return;
                }

                TalkyChannel oldChannel = client.Channel;
                client.JoinChannel(lobby, false);

                if (reason != null)
                {
                    oldChannel.BroadcastMessage(client.Username + " was kicked from the channel (" + reason + ").");
                }
                oldChannel.BroadcastMessage(client.Username + " was kicked from the channel.");
            }
        }

        public void BroadcastMessage(string message)
        {
            foreach (ServerClient client in ClientRepository.Instance.Find(this))
            {
                client.SendMessage(message);
            }
        }

    }
}
