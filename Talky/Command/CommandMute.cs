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
    class CommandMute : TalkyCommand
    {

        public CommandMute() : base("mute", "Toggle a client mute.", "/mute <username>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (client.Account == null || !client.Account.Role.Equals("admin"))
            {
                client.SendMessage("§2That command is admin only!");
                return;
            }

            if (args.Length != 1)
            {
                SendUsage(client);
                return;
            }

            string username = args[0];
            ServerClient found = ClientRepository.Instance.Find(username);
            if (found == null)
            {
                client.SendMessage("§2No client found with that username.");
                return;
            }
            found.Muted = !found.Muted;
            client.SendMessage("§4Client mute set to: " + found.Muted + " for " + found.Username + ".");
        }

    }
}
