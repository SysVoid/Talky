using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Channel;

namespace Talky.Command
{
    class CommandJoin : TalkyCommand
    {

        public CommandJoin() : base("join", "Join another channel.", "/join +<channel>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (args.Length != 1)
            {
                SendUsage(client);
                return;
            }

            string desiredChannel = args[0];

            if (!desiredChannel.StartsWith("+"))
            {
                desiredChannel = "+" + desiredChannel;
            }

            ServerChannel channel = ChannelRepository.Instance.Get(desiredChannel);
            if (channel == null)
            {
                client.SendMessage("That channel does not exist.");
                client.SendMessage("Use /clist to see a list of channels.");
                client.SendMessage("Use /cc to create a temporary channel.");
                return;
            }

            client.JoinChannel(channel);
        }

    }
}
