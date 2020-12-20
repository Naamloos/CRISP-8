using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp8
{
    internal static class FontLoader
    {
        public static byte[] GetDefaultFont()
        {
            // devernay.free.fr/hacks/chip8/C8TECH10.HTM#2.4
            var font = new byte[16 * 5];

            // 0
            font[0] = 0xF0;
            font[1] = 0x90;
            font[2] = 0x90;
            font[3] = 0x90;
            font[4] = 0xF0;

            // 1
            font[5] = 0x20;
            font[6] = 0x60;
            font[7] = 0x20;
            font[8] = 0x20;
            font[9] = 0x70;

            // 2
            font[10] = 0xF0;
            font[11] = 0x10;
            font[12] = 0xF0;
            font[13] = 0x80;
            font[14] = 0xF0;

            // 3
            font[15] = 0xF0;
            font[16] = 0x60;
            font[17] = 0x20;
            font[18] = 0x20;
            font[19] = 0x70;

            // 4
            font[20] = 0x90;
            font[21] = 0x90;
            font[22] = 0xF0;
            font[23] = 0x10;
            font[24] = 0x10;

            // 5
            font[25] = 0xF0;
            font[26] = 0x80;
            font[27] = 0xF0;
            font[28] = 0x10;
            font[29] = 0xF0;

            // 6
            font[30] = 0xF0;
            font[31] = 0x80;
            font[32] = 0xF0;
            font[33] = 0x90;
            font[34] = 0xF0;

            // 7
            font[35] = 0xF0;
            font[36] = 0x10;
            font[37] = 0x20;
            font[38] = 0x40;
            font[39] = 0x40;

            // 8
            font[40] = 0xF0;
            font[41] = 0x90;
            font[42] = 0xF0;
            font[43] = 0x90;
            font[44] = 0xF0;

            // 9
            font[45] = 0xF0;
            font[46] = 0x90;
            font[47] = 0xF0;
            font[48] = 0x10;
            font[49] = 0xF0;

            // A
            font[50] = 0xF0;
            font[51] = 0x90;
            font[52] = 0xF0;
            font[53] = 0x90;
            font[54] = 0x90;

            // B
            font[55] = 0xE0;
            font[56] = 0x90;
            font[57] = 0xE0;
            font[58] = 0x90;
            font[59] = 0xE0;

            // C
            font[60] = 0xF0;
            font[61] = 0x80;
            font[62] = 0x80;
            font[63] = 0x80;
            font[64] = 0xF0;

            // D
            font[65] = 0xE0;
            font[66] = 0x90;
            font[67] = 0x90;
            font[68] = 0x90;
            font[69] = 0xE0;

            // E
            font[70] = 0xF0;
            font[71] = 0x80;
            font[72] = 0xF0;
            font[73] = 0x80;
            font[74] = 0xF0;

            // F
            font[75] = 0xF0;
            font[76] = 0x80;
            font[77] = 0xF0;
            font[78] = 0x80;
            font[79] = 0x80;

            return font;
        }
    }
}
