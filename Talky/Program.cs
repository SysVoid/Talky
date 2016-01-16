using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talky.Channel;
using Talky.Message;
using Talky.Client;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Talky.Connection;
using Talky.Command;
using Talky.Exception;
using Talky.Configuration;

namespace Talky
{
    class Program
    {

        public const double SPAM_DELAY = 0.5;
        public static readonly DateTime EPOCH_START = new DateTime(1970, 1, 1);

        public bool PanicMode { get; private set; } = false;

        private ChannelRepository _channelRepository = ChannelRepository.Instance;
        private ClientRepository _clientRepository = ClientRepository.Instance;
        private CommandManager _commandManager = CommandManager.Instance;

        public int Port { get; } = 4096;

        public static Program Instance { get; private set; }

        static void Main(string[] args)
        {
            new Program();
        }

        private Program()
        {
            Instance = this;

            Dictionary<string, string> defaults = new Dictionary<string, string>();
            defaults.Add("+lobby", "true,false");
            defaults.Add("+admins", "false,true");

            ConfigurationFile config = new ConfigurationFile("channels");
            if (!config.Exists())
            {
                config.Write(defaults);
            }

            IReadOnlyDictionary<string, string> channels = config.Values();

            foreach (string key in channels.Keys)
            {
                string channelName = key;
                if (!channelName.StartsWith("+"))
                {
                    channelName = "+" + channelName;
                }

                string settings;
                string[] splitSettings;
                channels.TryGetValue(key, out settings);
                splitSettings = settings.Split(new char[] { ',' }, 2);

                if (splitSettings.Length != 2)
                {
                    continue;
                }

                bool lobby = (splitSettings[0].Equals("true") ? true : false);
                bool locked = (splitSettings[1].Equals("true") ? true : false);

                if (lobby && _channelRepository.GetLobby() != null)
                {
                    continue;
                }

                if (_channelRepository.Get(channelName) != null)
                {
                    continue;
                }

                if (lobby)
                {
                    _channelRepository.Store(new LobbyChannel(channelName));
                } else
                {
                    _channelRepository.Store(new SystemChannel(channelName, locked));
                }
            }

            try
            {
                _commandManager.RegisterCommand(new CommandHelp());
                _commandManager.RegisterCommand(new CommandName());
                _commandManager.RegisterCommand(new CommandJoin());
                _commandManager.RegisterCommand(new CommandClist());
                _commandManager.RegisterCommand(new CommandCC());
                _commandManager.RegisterCommand(new CommandAuth());
                _commandManager.RegisterCommand(new CommandRegister());
                _commandManager.RegisterCommand(new CommandRole());
                _commandManager.RegisterCommand(new CommandKick());
                _commandManager.RegisterCommand(new CommandMute());
                _commandManager.RegisterCommand(new CommandChangePassword());
                _commandManager.RegisterCommand(new CommandMsg());
            } catch (CommandExistsException cEE)
            {
                Console.WriteLine(cEE.StackTrace);
                return;
            }

            Thread listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Start();

            Thread consoleThread = new Thread(new ThreadStart(ShowConsole));
            consoleThread.Start();

            Thread channelManagerThread = new Thread(new ThreadStart(MonitorChannels));
            channelManagerThread.Start();
        }

        public void OHGODNO(string WHAT, System.Exception theRealProblem = null)
        {
            PanicMode = true;

            IReadOnlyCollection<ServerClient> clients = _clientRepository.All();
            foreach (ServerClient client in clients)
            {
                client.Disconnect("Server went into panic mode.");
            }

            Thread.Sleep(501);

            Console.Clear();
            Console.WriteLine("======================================");
            Console.WriteLine("=        SERVER IN PANIC MODE        =");
            Console.WriteLine("=         SEE PROBLEMS BELOW         =");
            Console.WriteLine("======================================");
            Console.WriteLine("");
            Console.WriteLine("DEAR GOD! NO!! A BUG! IT'S ALL BROKEN!");
            Console.WriteLine(WHAT);

            if (theRealProblem != null)
            {
                Console.WriteLine("");
                Console.WriteLine(theRealProblem.Message);
            }
            Console.WriteLine("");

            Console.WriteLine("Press a key or something to exit... I won't mind...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private void ShowConsole()
        {
            while (!PanicMode)
            {
                Console.Clear();
                Console.WriteLine("Talky | Created by SysVoid");
                Console.WriteLine("==========================");
                Console.WriteLine("Clients: " + _clientRepository.Count());
                Console.WriteLine("Channels: " + _channelRepository.Count());
                Console.WriteLine("Commands: " + _commandManager.Count());
                Console.WriteLine("==========================");
                Thread.Sleep(500);
            }
        }

        private void ListenForClients()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            while (!PanicMode)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                ServerClient serverClient = new ServerClient(tcpClient);

                if (_channelRepository.GetLobby() == null)
                {
                    serverClient.Disconnect("Server Error: No Lobby!");
                    continue;
                }

                Thread clientThread = new Thread(new ThreadStart(new ServerConnection(serverClient).HandleMessages));
                clientThread.Start();
            }
        }

        private void MonitorChannels()
        {
            while (!PanicMode)
            {
                IReadOnlyCollection<ClientChannel> clientChannels = _channelRepository.Get<ClientChannel>();

                if (clientChannels.Count > 0)
                {
                    foreach (ClientChannel clientChannel in clientChannels)
                    {
                        if (_clientRepository.Find(clientChannel).Count == 0)
                        {
                            _channelRepository.Remove(clientChannel);
                        }
                    }
                }

                Thread.Sleep(5000);
            }
        }

    }
}
