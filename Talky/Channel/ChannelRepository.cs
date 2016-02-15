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
        private List<TalkyChannel> _channels = new List<TalkyChannel>();
        private object _lock = new object();

        private ChannelRepository() { }
        
        public void Store(TalkyChannel channel)
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

        public TalkyChannel Get(string name)
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

        public IReadOnlyList<T> Get<T>() where T : TalkyChannel
        {
            lock (_lock)
            {
                List<TalkyChannel> channels = _channels.FindAll(channel => channel is T);
                List<T> goodChannels = new List<T>();

                foreach (TalkyChannel channel in channels)
                {
                    goodChannels.Add((T) channel);
                }

                return goodChannels;
            }
        }

        public bool Exists<T>() where T : TalkyChannel
        {
            return (Get<T>().Count > 0);
        }

        public IReadOnlyCollection<TalkyChannel> All()
        {
            lock (_lock)
            {
                return new List<TalkyChannel>(_channels).AsReadOnly();
            }
        }

        public int Count()
        {
            lock (_lock)
            {
                return _channels.Count;
            }
        }

        public void Remove(TalkyChannel channel)
        {
            lock (_lock)
            {
                _channels.Remove(channel);
            }
        }

    }
}
