using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Client;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Talky.Message;
using Talky.Channel;

namespace Talky.Connection
{
    class ServerConnection
    {

        public ServerClient Client { get; private set; }

        public ServerConnection(ServerClient client)
        {
            Client = client;
            ClientRepository.Instance.Store(client);
        }

        public void HandleMessages()
        {
            StreamReader reader = new StreamReader(Client.TcpClient.GetStream());
            while (true)
            {
                string line = null;
                
                try
                {
                    line = reader.ReadLine();
                } catch (System.Exception e)
                {
                    line = null;
                }

                if (string.IsNullOrEmpty(line))
                {
                    Client.Disconnect("Unexpected EOL");
                    return;
                }

                ChatMessage chatMessage = new ChatMessage(Client, line);
                CommandMessage commandMessage = new CommandMessage(Client, line);

                if (chatMessage.Valid())
                {
                    chatMessage.Handle();
                } else if (commandMessage.Valid())
                {
                    commandMessage.Handle();
                } else
                {
                    // ???
                    Client.Disconnect("What was that?");
                    return;
                }
            }
        }

    }
}
