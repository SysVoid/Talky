using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;

namespace Talky.Command
{
    class CommandMsg : TalkyCommand
    {

        public CommandMsg() : base("msg", "Message another client.", "/msg <username> <message...>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (client.Muted)
            {
                client.SendMessage("You are muted.");
                return;
            }

            if (args.Length < 2)
            {
                SendUsage(client);
                return;
            }

            string recipientUsername = args[0];
            string message = "";
            
            for (int i = 1; i < args.Length; i++)
            {
                if (i == args.Length - 1)
                {
                    message += args[i];
                } else
                {
                    message += args[i] + " ";
                }
            }

            ServerClient recipient = ClientRepository.Instance.Find(recipientUsername);
            if (recipient == null)
            {
                client.SendMessage("No client found by that username.");
                return;
            }

            if (recipient.Equals(client))
            {
                client.SendMessage("You cannot send messages to yourself.");
                return;
            }

            recipient.SendMessage("<" + client.Username + " -> you> " + message);
            client.SendMessage("<you -> " + recipient.Username + "> " + message);
        }

    }
}
