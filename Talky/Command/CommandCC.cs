using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Channel;

namespace Talky.Command
{
    class CommandCC : TalkyCommand
    {

        public CommandCC() : base("cc", "Create a new channel.", "/cc ++<channel>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (args.Length != 1)
            {
                SendUsage(client);
                return;
            }

            string desiredChannel = args[0];

            if (!desiredChannel.StartsWith("++"))
            {
                desiredChannel = "++" + desiredChannel;
            }

            if (desiredChannel.Length > 16 || desiredChannel.Substring(2).Contains("+") || desiredChannel.Contains("%") || desiredChannel.Contains("/") || desiredChannel.Contains("@") || desiredChannel.Contains("\\") || desiredChannel.Contains(";"))
            {
                client.SendMessage("Invalid Channel name. Channel names may not contain +, %, /, @, ; or \\. Channel names also have a maximum length of 16 characters.");
                return;
            }

            ServerChannel channel = ChannelRepository.Instance.Get(desiredChannel);
            if (channel != null)
            {
                client.SendMessage("That channel already exists.");
                return;
            }

            ClientChannel chan = new ClientChannel(desiredChannel);
            ChannelRepository.Instance.Store(chan);
            client.SendMessage("Your channel has been created.");
            client.JoinChannel(chan);
        }

    }
}
