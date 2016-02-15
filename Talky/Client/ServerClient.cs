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

        public int LastMessage { get; set; } = 0;
        public int LastCommand { get; set; } = 0;
        public int LastActivity { get; set; } = 0;

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
        public TalkyChannel Channel { get; private set; }

        public UserAccount Account { get; set; } = null;

        public ServerClient(TcpClient client)
        {
            Username = "%";
            TcpClient = client;
            LastActivity = (int) (DateTime.UtcNow.Subtract(Program.EPOCH_START)).TotalSeconds;
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
                Channel.BroadcastMessage(Username + " disconnected: " + reason);
                using (StreamWriter writer = new StreamWriter(TcpClient.GetStream()))
                {
                    writer.WriteLine("M:§2You were disconnected from the server. Reason: " + reason);
                    writer.Flush();
                }
            } else
            {
                if (!string.IsNullOrEmpty(Username) && !Username.Equals("%"))
                {
                    Channel.BroadcastMessage(Username + " disconnected.");
                }
            }

            TcpClient.Client.Close();
            ClientRepository.Instance.Remove(this);
        }

        public void JoinChannel(TalkyChannel channel, bool announce = true)
        {
            if (channel.Locked)
            {
                if (Account == null || !Account.Role.Equals("admin"))
                {
                    SendMessage("§2That channel is locked!");
                    return;
                }
            }

            if (Channel != null)
            {
                Channel.BroadcastMessage(Username + " left " + "§4" + Channel.Name + "§0.");
            }

            Channel = channel;

            if (announce)
            {
                channel.BroadcastMessage(Username + " joined " + "§4" + channel.Name + "§0!");
            }
        }

    }
}
