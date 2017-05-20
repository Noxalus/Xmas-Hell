using System;
using System.Collections.Generic;
using System.Text;

namespace XmasHell.Entities.Bosses
{
    public interface IBossDeadlyEntity
    {
        bool Used();
        void Used(bool value);
    }
}
