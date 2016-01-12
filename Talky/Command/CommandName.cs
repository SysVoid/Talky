using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Channel;
using Talky.Authentication;

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

            if (desiredUsername.Length > 16 || desiredUsername.Contains("%") || desiredUsername.Contains("/") || desiredUsername.Contains("@") || desiredUsername.Contains("\\") || desiredUsername.Contains(";"))
            {
                client.SendMessage("Invalid username. Usernames may not contain %, /, @, ; or \\. Usernames also have a maximum length of 16 characters.");
                return;
            }

            if (ClientRepository.Instance.Exists(desiredUsername))
            {
                client.SendMessage("That username is already in use.");
                return;
            }

            if (UserAccount.Find(desiredUsername) != null)
            {
                client.SendMessage("That username is linked to an account.");
                client.SendMessage("If you are the account holder, use /auth to login and claim the username.");
                return;
            }

            if (client.Account != null)
            {
                client.SendMessage("Authenticated clients may not change their username.");
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
