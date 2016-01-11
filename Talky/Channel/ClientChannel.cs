using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talky.Channel
{
    class ClientChannel : ServerChannel
    {

        public ClientChannel(string name) : base(name, false) { }

    }
}
