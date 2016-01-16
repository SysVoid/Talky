using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Authentication;
using Talky.Channel;

namespace Talky.Command
{
    class CommandAuth : TalkyCommand
    {

        public CommandAuth() : base("auth", "Authenticate with Talky.", "/auth <username> <password>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (args.Length != 2)
            {
                SendUsage(client);
                return;
            }

            string username = args[0];
            string password = args[1];

            UserAccount account = UserAccount.Attempt(username, password);
            if (account == null)
            {
                client.SendMessage("§2Invalid username/password.");
                return;
            }

            ServerClient foundClient = ClientRepository.Instance.Find(account.Username);
            if (foundClient != null)
            {
                if (!foundClient.Equals(client))
                {
                    foundClient.Disconnect("§2This username has been reclaimed by the account owner.");
                }
            }

            string oldUsername = client.Username;
            client.Account = account;
            client.Username = account.Username;
            client.SendMessage("§4You are now authenticated as " + account.Username + "!");

            if (client.Channel == null)
            {
                client.JoinChannel(ChannelRepository.Instance.GetLobby());
            }

            if (!oldUsername.Equals("%"))
            {
                client.Channel.BroadcastMessage("§1" + oldUsername + " is now known as " + account.Username);
            }
        }

    }
}
