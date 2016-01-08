using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Channel;

namespace Talky.Command
{
    class CommandName : TalkyCommand
    {

        public CommandName() : base("name", "Change your username.", "/name <username>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (args.Length != 1)
            {
                SendUsage(client);
                return;
            }

            string desiredUsername = args[0];

            if (desiredUsername.Contains("%") || desiredUsername.Contains("/") || desiredUsername.Contains("@") || desiredUsername.Contains("\\"))
            {
                client.SendMessage("Invalid username. Usernames may not contain %, /, @ or \\.");
                return;
            }

            if (ClientRepository.Instance.Exists(desiredUsername))
            {
                client.SendMessage("That username is already in use.");
                return;
            }

            string oldUsername = client.Username;
            client.Username = desiredUsername;
            client.SendMessage("You are now known as " + desiredUsername + ".");

            if (client.Channel == null)
            {
                client.JoinChannel(ChannelRepository.Instance.GetLobby());
            }

            if (!oldUsername.Equals("%"))
            {
                client.Channel.BroadcastMessage(oldUsername + " is now known as " + desiredUsername);
            }
        }

    }
}
