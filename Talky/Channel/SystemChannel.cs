using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talky.Channel
{
    class SystemChannel : ServerChannel
    {

        public SystemChannel(string name, bool locked) : base(name, locked) { }

    }
}
