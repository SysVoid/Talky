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

            _channelRepository.Store(new LobbyChannel("+lobby"));
            _channelRepository.Store(new SystemChannel("+admins", true));

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

            Thread serverMessageThread = new Thread(new ThreadStart(ServerMessageReader));
            serverMessageThread.Start();

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
                int before = Console.CursorLeft;

                ClearConsoleLine(0);
                Console.WriteLine("Talky | Created by SysVoid");
                ClearConsoleLine(1);
                Console.WriteLine("==========================");
                ClearConsoleLine(2);
                Console.WriteLine("Clients: " + _clientRepository.Count());
                ClearConsoleLine(3);
                Console.WriteLine("Channels: " + _channelRepository.Count());
                ClearConsoleLine(4);
                Console.WriteLine("Commands: " + _commandManager.Count());
                ClearConsoleLine(5);
                Console.WriteLine("==========================");

                Console.SetCursorPosition(before, 6);
                Thread.Sleep(500);
            }
        }

        private static void ClearConsoleLine(int line)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth - 1));
            Console.SetCursorPosition(0, line);
        }

        public void ServerMessageReader()
        {
            while (!PanicMode)
            {
                string input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                GlobalBroadcastMessage(input);
                ClearConsoleLine(6);
                Console.SetCursorPosition(0, 7);
                Console.WriteLine("Global message sent!");
                Console.SetCursorPosition(0, 6);
                Thread.Sleep(1500);
                ClearConsoleLine(7);
            }
        }

        private void GlobalBroadcastMessage(string message)
        {
            foreach (ServerClient client in ClientRepository.Instance.All())
            {
                client.SendMessage("[GLOBAL] " + message);
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
