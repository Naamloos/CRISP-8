using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp8.Interfaces
{
    public interface IScreen
    {
        bool Pixel(int x, int y);

        void Clear();

        void Render();
    }
}
