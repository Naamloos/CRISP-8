using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp8.Interfaces
{
    public interface IScreen
    {
        void Draw(byte x, byte y, byte[] data);
    }
}
