// http://devernay.free.fr/hacks/chip8/C8TECH10.HTM
using Crisp8.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

// TODO: Maybe implement super chip-48 support?

namespace Crisp8
{
    public class Chip8
    {
        private byte[] ram = new byte[4096];
        private Registers registers = new Registers();

        private IScreen screen;
        private ISoundHandler sound;
        private IKeyboard keyboard;

        private Stopwatch timer;
        private Random rng;

        public Chip8(byte[] rom, IScreen screen, ISoundHandler sound, IKeyboard keyboard)
        {
            this.ram = rom;

            this.screen = screen;
            this.sound = sound;
            this.keyboard = keyboard;
            this.registers.Stack = new Stack<ushort>();
            this.rng = new Random();
        }

        public void Start()
        {
            timer = new Stopwatch();
            timer.Start();
            // load default font into interpreter ram
            FontLoader.GetDefaultFont().CopyTo(ram, 0);

            registers.PC = 0x200; // Most chip-8 programs start at address 0x200 (512)
            while (true)
            {
                if(timer.ElapsedMilliseconds == 1000 / 60)
                {
                    tick();
                    timer.Reset();
                }
            }
        }

        private void tick()
        {
            var pointer = this.registers.PC;
            if(registers.ST > 0)
            {
                // buzz
                registers.ST--;
            }
            if(registers.DT > 0)
            {
                registers.DT--;
            }

            // read instruction
            // All instructions are 2 bytes long
            ushort instruction = BitConverter.ToUInt16( new byte[2] { ram[pointer], ram[pointer + 1] }, 0);
            executeInstruction(instruction);
        }

        private void executeInstruction(ushort instruction)
        {
            // an instruction is 1 short, so 2 bytes, so 4 nibbles. C# has no nibble type so we'll have to figure that out ourselves.
            // bitmasking time! :D ( D: )

            // The first nibble is never variable, we can just throw that into a switch
            switch ((ushort)(instruction & 0x0FFF))
            {
                case 0x0000:
                    // nible 0 is 0
                    instruction_0(instruction);
                    break;

                case 0x1000:
                    // nible 0 is 1
                    break;

                case 0x2000:
                    // nible 0 is 2
                    break;

                case 0x3000:
                    // nible 0 is 3
                    break;

                case 0x4000:
                    // nible 0 is 4
                    break;

                case 0x5000:
                    // nible 0 is 5
                    break;

                case 0x6000:
                    // nible 0 is 6
                    break;

                case 0x7000:
                    // nible 0 is 7
                    break;

                case 0x8000:
                    // nible 0 is 8
                    break;

                case 0x9000:
                    // nible 0 is 9
                    break;

                case 0xA000:
                    // nible 0 is A
                    break;

                case 0xB000:
                    // nible 0 is B
                    break;

                case 0xC000:
                    // nible 0 is C
                    break;

                case 0xD000:
                    // nible 0 is D
                    break;

                case 0xE000:
                    // nible 0 is E
                    break;

                case 0xF000:
                    // nible 0 is F
                    break;
            }

            registers.PC++;
        }

        private void instruction_0(ushort instruction)
        {
            // 0nnn is ingored by "modern interpreters" so we'll just ignore it
            // to become a modern interpreter.

            switch (instruction)
            {
                case 0x00E0:
                    // TODO Clear display
                    break;

                case 0x00EE:
                    // TODO Return from subroutine
                    break;

                default:
                    unknownInstruction(instruction);
                    break;
            }
        }

        private void instruction_1(ushort instruction)
        {
            // 1 only has one instruction, jump to location.
            // program counter gets set to the last 3 nibbles.
            registers.PC = (ushort)(instruction & 0xF000);
        }

        private void instruction_2(ushort instruction)
        {
            // Calls subroutine at nnn (last 3 nibbles)
            // The interpreter increments the stack pointer, then puts the current PC on the top of the stack. The PC is then set to nnn.
            registers.SP++;
            registers.Stack.Push(registers.PC);
            registers.PC = (ushort)(instruction & 0xF000);
        }

        private void instruction_3(ushort instruction)
        {
            // 3xkk
            // Skip next instruction if Vx = kk.
            // The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
            var register = ((instruction & 0xF0FF) >> 2);

            if(registers.V[register] == (byte)(instruction & 0xFF00))
            {
                // PC always gets incremented so we just increment it once extra to increment it twice.
                this.registers.PC++;
            }
        }

        private void instruction_4(ushort instruction)
        {
            // 4xkk - SNE Vx, byte
            // Skip next instruction if Vx != kk.
            // The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
            var register = ((instruction & 0xF0FF) >> 2);

            if (registers.V[register] != (byte)(instruction & 0xFF00))
            {
                // PC always gets incremented so we just increment it once extra to increment it twice.
                this.registers.PC++;
            }
        }

        private void instruction_5(ushort instruction)
        {
            // 5xy0 - SE Vx, Vy
            // Skip next instruction if Vx = Vy.
            // The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.

            var x = ((instruction & 0xF0FF) >> 2);
            var y = ((instruction & 0xFF0F) >> 1);

            if(registers.V[x] == registers.V[y])
            {
                this.registers.PC++;
            }
        }

        private void instruction_6(ushort instruction)
        {
            // 6xkk - LD Vx, byte
            // Set Vx = kk.
            // The interpreter puts the value kk into register Vx.

            var x = ((instruction & 0xF0FF) >> 2);
            var kk = ((instruction & 0xFF00));

            registers.V[x] = (byte)kk;
        }

        private void instruction_7(ushort instruction)
        {
            // 7xkk - ADD Vx, byte
            // Set Vx = Vx + kk.
            // Adds the value kk to the value of register Vx, then stores the result in Vx.

            var x = ((instruction & 0xF0FF) >> 2);
            var kk = ((instruction & 0xFF00));

            registers.V[x] += (byte)kk;
        }

        private void instruction_8(ushort instruction)
        {
            // 8xyi
            // a lot of bitwise stuff ig
            var x = ((instruction & 0xF0FF) >> 2);
            var y = ((instruction & 0xFF0F) >> 1);
            var i = (instruction & 0xFFF0);

            // do operations based on z
            switch (i)
            {
                case 0x0000:
                    // store Vy in Vx
                    registers.V[x] = registers.V[y];
                    break;

                case 0x0001:
                    // Set Vx = Vx OR Vy.
                    // Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx.
                    registers.V[x] = (byte)((int)registers.V[x] | (int)registers.V[y]);
                    break;

                case 0x0002:
                    // Set Vx = Vx AND Vy.
                    // Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx.
                    registers.V[x] = (byte)((int)registers.V[x] & (int)registers.V[y]);
                    break;

                case 0x0003:
                    // XOR
                    registers.V[x] = (byte)((int)registers.V[x] ^ (int)registers.V[y]);
                    break;

                case 0x0004:
                    // Set Vx = Vx + Vy, set VF = carry.
                    // The values of Vx and Vy are added together. 
                    // If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0.
                    // Only the lowest 8 bits of the result are kept, and stored in Vx.
                    var res = registers.V[x] + registers.V[y];

                    if (res > 255)
                    {
                        registers.V[0x0F] = 1;
                    }

                    registers.V[x] = (byte)(res & 0xFFFF0000);
                    break;

                case 0x0005:
                    // Set Vx = Vx - Vy, set VF = NOT borrow.
                    // If Vx > Vy, then VF is set to 1, otherwise 0.Then Vy is subtracted from Vx, and the results stored in Vx.
                    if(registers.V[x] > registers.V[y])
                    {
                        registers.V[0x0F] = 1;
                    }
                    else
                    {
                        registers.V[0x0F] = 0;
                    }

                    registers.V[x] -= registers.V[y];
                    break;

                case 0x0006:
                    // Set Vx = Vx SHR 1.
                    // If the least - significant bit of Vx is 1, then VF is set to 1, otherwise 0.Then Vx is divided by 2.
                    if((registers.V[x] & 1) == 1)
                    {
                        registers.V[0x0F] = 1;
                    }
                    else
                    {
                        registers.V[0x0F] = 0;
                    }

                    registers.V[x] = (byte)(registers.V[x] / 2);
                    break;

                case 0x0007:
                    // Set Vx = Vy - Vx, set VF = NOT borrow.
                    // If Vy > Vx, then VF is set to 1, otherwise 0.Then Vx is subtracted from Vy, and the results stored in Vx.

                    if(registers.V[y] > registers.V[x])
                    {
                        registers.V[0x0F] = 1;
                    }
                    else
                    {
                        registers.V[0x0F] = 0;
                    }

                    registers.V[y] -= registers.V[x];
                    break;

                case 0x000E:
                    // Set Vx = Vx SHL 1.
                    // If the most - significant bit of Vx is 1, then VF is set to 1, otherwise to 0.Then Vx is multiplied by 2.

                    // same as 0006, but instead we bitshift by 15 to set the MSB at the position of the LSB.
                    if (((registers.V[x] >> 15) & 1) == 1)
                    {
                        registers.V[0x0F] = 1;
                    }
                    else
                    {
                        registers.V[0x0F] = 0;
                    }

                    registers.V[x] = (byte)(registers.V[x] * 2);
                    break;

                default:
                    Console.WriteLine("Invalid instruction on 8xyi");
                    break;
            }
        }

        private void instruction_9(ushort instruction)
        {
            // 9xy0 - SNE Vx, Vy
            // Skip next instruction if Vx != Vy.
            // The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.

            var x = ((instruction & 0xF0FF) >> 2);
            var y = ((instruction & 0xFF0F) >> 1);

            if (registers.V[x] != registers.V[y])
            {
                // increase by 1 because we always increase
                this.registers.PC++;
            }
        }

        private void instruction_A(ushort instruction)
        {
            // Annn - LD I, addr
            // Set I = nnn.
            // The value of register I is set to nnn.
            registers.I = (byte)(instruction & 0xF000);
        }

        private void instruction_B(ushort instruction)
        {
            // Bnnn - JP V0, addr
            // Jump to location nnn + V0.
            // The program counter is set to nnn plus the value of V0.
            var nnn = (byte)(instruction & 0xF000);
            registers.PC = (ushort)(nnn + registers.V[0]);
        }

        private void instruction_C(ushort instruction)
        {
            // Cxkk - RND Vx, byte
            // Set Vx = random byte AND kk.
            // The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk.
            // The results are stored in Vx. See instruction 8xy2 for more information on AND.

            var x = ((instruction & 0xF0FF) >> 2);
            var kk = ((instruction & 0xFF00));

            var rngbuffer = new byte[1];
            rng.NextBytes(rngbuffer);
            registers.V[x] = (byte)(rngbuffer[0] & kk);
        }

        private void instruction_D(ushort instruction)
        {
            // Dxyn - DRW Vx, Vy, nibble
            // Display n-byte sprite starting at memory location I at(Vx, Vy), set VF = collision.
            /*
             * The interpreter reads n bytes from memory, starting at the address stored in I.
             * These bytes are then displayed as sprites on screen at coordinates(Vx, Vy).
             * Sprites are XORed onto the existing screen.
             * If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0.
             * If the sprite is positioned so part of it is outside the coordinates of the display, 
             * it wraps around to the opposite side of the screen.
             * See instruction 8xy3 for more information on XOR, and section 2.4, Display, for more information on the Chip - 8 screen and sprites.
             */
            var x = ((instruction & 0xF0FF) >> 2);
            var y = ((instruction & 0xFF0F) >> 1);

            var amount = (byte)(instruction & 0xFFF0);
            var startaddress = registers.I;
            var data = new byte[amount];

            for(ushort i = 0; i < amount; i++)
            {
                data[i] = ram[startaddress + i];
            }

            screen.Draw(registers.V[x], registers.V[y], data);
        }

        private void instruction_E(ushort instruction)
        {
            var x = ((instruction & 0xF0FF) >> 2);
            var last2nibs = (instruction & 0xFF00);

            if(last2nibs == 0x009E && keyboard.isKeyDown(registers.V[x])
                || last2nibs == 0x00A1 && keyboard.isKeyUp(registers.V[x]))
            {
                // every isntruction already increases PC so this would just increase it twice
                registers.PC++;
            }
        }

        private void instruction_F(ushort instruction)
        {
            var x = ((instruction & 0xF0FF) >> 2);
            var i = (instruction & 0xFF00);

            switch (i)
            {
                case 0x07:
                    registers.V[x] = registers.DT;
                    break;

                case 0x0A:
                    registers.V[x] = keyboard.waitForKeyPress();
                    break;

                case 0x15:
                    registers.DT = registers.V[x];
                    break;

                case 0x18:
                    registers.ST = registers.V[x];
                    break;

                case 0x1E:
                    registers.I += registers.V[x];
                    break;

                case 0x29:
                    // Set I = location of sprite for digit Vx.
                    // The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx.
                    // See section 2.4, Display, for more information on the Chip - 8 hexadecimal font.

                    // The font is loaded from ram location 0-79 with an increment of 5 for every character.
                    registers.I = (byte)(registers.V[x] * 5);
                    break;

                case 0x33:
                    // Store BCD representation of Vx in memory locations I, I+1, and I+2.
                    // The interpreter takes the decimal value of Vx, and places the hundreds digit in memory at location in I,
                    // the tens digit at location I+1, and the ones digit at location I + 2.
                    var loc = registers.I;
                    var val = registers.V[x];
                    ram[loc] = (byte)((val & 0x0FF) >> 2);
                    ram[loc + 1] = (byte)((val & 0xF0F) >> 1);
                    ram[loc + 2] = (byte)(val & 0xFF0);
                    break;

                case 0x55:
                    var start = registers.I;

                    for(int index = 0; index < x; index++)
                    {
                        ram[start + index] = registers.V[index];
                    }
                    break;

                case 0x65:
                    start = registers.I;

                    for (int index = 0; index < x; index++)
                    {
                        registers.V[index] = ram[start + index];
                    }
                    break;
            }
        }

        private void unknownInstruction(ushort instruction)
        {
            Console.WriteLine($"Unknown instruction {instruction.ToString("X4")}");
        }
    }
}
