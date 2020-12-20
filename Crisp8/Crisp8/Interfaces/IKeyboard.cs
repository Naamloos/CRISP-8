using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp8.Interfaces
{
    public interface IKeyboard
    {
        bool isKeyDown(byte key);

        bool isKeyUp(byte key);

        byte waitForKeyPress();
    }
}
