using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talky.Channel
{
    class ChannelRepository
    {

        public static ChannelRepository Instance { get; } = new ChannelRepository();
        private List<ServerChannel> _channels = new List<ServerChannel>();
        private object _lock = new object();

        private ChannelRepository() { }
        
        public void Store(ServerChannel channel)
        {
            lock (_lock)
            {
                _channels.Add(channel);
            }
        }

        public LobbyChannel GetLobby()
        {
            lock (_lock)
            {
                return (LobbyChannel) _channels.FirstOrDefault(channel => channel is LobbyChannel);
            }
        }

        public ServerChannel Get(string name)
        {
            lock (_lock)
            {
                return _channels.FirstOrDefault(channel => string.Equals(channel.Name, name, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public bool Exists(string name)
        {
            return (Get(name) != null);
        }

        public IReadOnlyCollection<T> Get<T>() where T : ServerChannel
        {
            lock (_lock)
            {
                return (IReadOnlyCollection<T>) _channels.FindAll(channel => channel is T);
            }
        }

        public bool Exists<T>() where T : ServerChannel
        {
            return (Get<T>().Count > 0);
        }

        public IReadOnlyCollection<ServerChannel> All()
        {
            lock (_lock)
            {
                return new List<ServerChannel>(_channels).AsReadOnly();
            }
        }

        public int Count()
        {
            lock (_lock)
            {
                return _channels.Count;
            }
        }

        public void Remove(ServerChannel channel)
        {
            lock (_lock)
            {
                _channels.Remove(channel);
            }
        }

    }
}
