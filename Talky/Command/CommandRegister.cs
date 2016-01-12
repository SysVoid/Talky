using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using Talky.Authentication;

namespace Talky.Command
{
    class CommandRegister : TalkyCommand
    {

        public CommandRegister() : base("register", "Create a new account.", "/register <username> <password>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (args.Length != 2)
            {
                SendUsage(client);
                return;
            }

            string username = args[0];
            string password = args[1];

            if (client.Account != null)
            {
                client.SendMessage("You are already authenticated.");
                return;
            }

            if (username.Length > 16 || username.Contains("%") || username.Contains("/") || username.Contains("@") || username.Contains("\\") || username.Contains(";"))
            {
                client.SendMessage("Invalid username. Usernames may not contain %, /, @, ; or \\. Usernames also have a maximum length of 16 characters.");
                return;
            }

            if (UserAccount.Find(username) != null)
            {
                client.SendMessage("An account with that username already exists.");
                return;
            }

            UserAccount account = UserAccount.Create(username, password, false);
            if (account == null)
            {
                client.SendMessage("There was a problem creating your account. Please try again later.");
                return;
            }

            client.Account = account;
            client.SendMessage("You are now authenticated as " + account.Username + "!");
        }

    }
}
