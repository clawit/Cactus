using System;
using System.Collections.Generic;
using System.Text;

namespace Cactus.Protocol.Interface
{
    public interface IBus
    {
        bool Publish();

        bool Subscribe();
    }
}
