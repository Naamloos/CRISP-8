// https://github.com/mattmikolay/chip-8/wiki/CHIP%E2%80%908-Technical-Reference#references
// https://github.com/mattmikolay/chip-8/wiki/CHIP%E2%80%908-Instruction-Set
// http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
using Crisp8.Interfaces;
using System;

namespace Crisp8
{
    public class Chip8
    {
        private int current = 0;
        private byte[] ram = new byte[4096];
        private Registers registers = new Registers();

        private IScreen screen;
        private ISoundHandler sound;
        private IKeyboard keyboard;

        public Chip8(byte[] rom, IScreen screen, ISoundHandler sound, IKeyboard keyboard)
        {
            this.ram = rom;

            this.screen = screen;
            this.sound = sound;
            this.keyboard = keyboard;
        }

        private void runInstruction(short instruction)
        {
            // an instruction is 1 short, so 2 bytes, so 4 nibbles. C# has no nibble type so we'll have to figure that out ourselves.
            // bitmasking time! :D ( D: )
        }
    }
}
