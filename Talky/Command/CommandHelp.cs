using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;

namespace Talky.Command
{
    class CommandHelp : TalkyCommand
    {

        public CommandHelp() : base("help", "See a list of commands.", "/help") { }

        public override void Execute(ServerClient client, string[] args)
        {
            IReadOnlyList<TalkyCommand> commands = CommandManager.Instance.All();
            client.SendMessage("=COMMANDS=============");

            foreach (TalkyCommand command in commands)
            {
                client.SendMessage("[" + command.Usage + "] " + command.Description);
            }

            client.SendMessage("=COMMANDS=============");
        }

    }
}
