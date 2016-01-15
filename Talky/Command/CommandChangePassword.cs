using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Authentication;
using Talky.Client;

namespace Talky.Command
{
    class CommandChangePassword : TalkyCommand
    {

        public CommandChangePassword() : base("changepassword", "Change your password.", "/changepassword <current> <new> <repeat>") { }

        public override void Execute(ServerClient client, string[] args)
        {
            if (client.Account == null)
            {
                client.SendMessage("You must be authenticated to use this command. See /auth.");
                return;
            }

            if (args.Length != 3)
            {
                SendUsage(client);
                return;
            }

            string currentPassword = args[0];
            string newPassword = args[1];
            string newPasswordRepeat = args[2];

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                client.SendMessage("Invalid password. Passwords must contain at least 6 characters. Passwords may not be whitespace.");
                return;
            }

            if (!client.Account.ComparePassword(currentPassword))
            {
                client.SendMessage("Failed to authenticate. Invalid password.");
                return;
            }

            if (client.Account.ComparePassword(newPassword))
            {
                client.SendMessage("New password is the same as your old password.");
                return;
            }
            
            if (!newPassword.Equals(newPasswordRepeat, StringComparison.Ordinal))
            {
                client.SendMessage("New password and password confirmation do not match. Please try again.");
                return;
            }

            client.Account.SetPassword(newPassword);
            client.SendMessage("Your password has been updated.");
        }

    }
}
