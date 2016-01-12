using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Talky.Channel;
using Talky.Authentication;

namespace Talky.Client
{
    class ServerClient
    {

        public TcpClient TcpClient { get; set; }

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                if (value.Length > 16)
                {
                    value = value.Substring(0, 15);
                }
                _username = value.Replace(";", "-");
            }
        }

        public bool Muted { get; set; } = false;
        public ServerChannel Channel { get; private set; }

        public UserAccount Account { get; set; }

        public ServerClient(TcpClient client)
        {
            Username = "%";
            TcpClient = client;
        }

        public void SendMessage(string message)
        {
            SendRawMessage("M:" + message);
        }

        public void SendRawMessage(string message)
        {
            try
            {
                StreamWriter writer = new StreamWriter(TcpClient.GetStream());
                writer.WriteLine(message);
                writer.Flush();
            } catch
            {
                Disconnect();
            }
        }

        public void Disconnect(string reason = null)
        {
            if (reason != null)
            {
                SendRawMessage("You were disconnected from the server. Reason: " + reason);
            }
            TcpClient.Close();
            ClientRepository.Instance.Remove(this);
        }

        public void JoinChannel(ServerChannel channel, bool announce = true)
        {
            if (channel.Locked)
            {
                if (Account == null || !Account.Role.Equals("admin"))
                {
                    SendMessage("That channel is locked!");
                    return;
                }
            }

            if (Channel != null)
            {
                Channel.BroadcastMessage(Username + " left " + Channel.Name + ".");
            }

            Channel = channel;

            if (announce)
            {
                channel.BroadcastMessage(Username + " joined " + channel.Name + "!");
            }
        }

    }
}
