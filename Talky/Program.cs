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

        private ChannelRepository _channelRepository = ChannelRepository.Instance;
        private ClientRepository _clientRepository = ClientRepository.Instance;
        private CommandManager _commandManager = CommandManager.Instance;

        public int Port { get; } = 4096;

        static void Main(string[] args)
        {
            new Program();
        }

        private Program()
        {
            _channelRepository.Store(new LobbyChannel("+lobby"));
            _channelRepository.Store(new SystemChannel("+test-1", false));
            _channelRepository.Store(new SystemChannel("+test-2", true));

            try
            {
                _commandManager.RegisterCommand(new CommandHelp());
                _commandManager.RegisterCommand(new CommandName());
                _commandManager.RegisterCommand(new CommandJoin());
                _commandManager.RegisterCommand(new CommandClist());
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
        }

        private void ShowConsole()
        {
            while (true)
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
            while (true)
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
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                ServerClient serverClient = new ServerClient(tcpClient);

                Thread clientThread = new Thread(new ThreadStart(new ServerConnection(serverClient).HandleMessages));
                clientThread.Start();
            }
        }

    }
}
