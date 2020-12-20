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
        private bool killed = false;

        public Chip8(byte[] rom, IScreen screen, ISoundHandler sound, IKeyboard keyboard)
        {
            registers.PC = 512; // Most chip-8 programs start at address 0x200 (512)
            rom.CopyTo(ram, 512);

            this.screen = screen;
            this.sound = sound;
            this.keyboard = keyboard;
            this.registers.Stack = new Stack<ushort>();
            this.registers.V = new byte[16];
            this.rng = new Random();
        }

        public void Start()
        {
            if (killed)
            {
                throw new Exception("Create a new Emulator class to restart! This one has been killed.");
            }
            timer = new Stopwatch();
            timer.Start();
            // load default font into interpreter ram
            FontLoader.GetDefaultFont().CopyTo(ram, 0);

            while (!killed)
            {
                if(timer.ElapsedMilliseconds > 1000 / 60)
                {
                    tick();
                    timer.Restart();
                }
            }
        }

        public void Kill()
        {
            this.killed = true;
        }

        private void tick()
        {
            var pointer = this.registers.PC;
            if(registers.ST > 0)
            {
                // buzz
                sound.Beep();
                registers.ST--;
            }
            if(registers.DT > 0)
            {
                registers.DT--;
            }

            // read instruction
            // All instructions are 2 bytes long
            ushort instruction = BitConverter.ToUInt16( new byte[2] { ram[pointer + 1], ram[pointer] }, 0);
            executeInstruction(instruction);
            screen.Render();
            registers.PC += 2;
        }

        private void executeInstruction(ushort instruction)
        {
            // an instruction is 1 short, so 2 bytes, so 4 nibbles. C# has no nibble type so we'll have to figure that out ourselves.
            // bitmasking time! :D ( D: )

            // The first nibble is never variable, we can just throw that into a switch
            var mask = (instruction & 0xF000);
            var opcode = (byte)(mask >> 12);
            switch (opcode)
            {
                case 0x0:
                    // nible 0 is 0
                    instruction_0(instruction);
                    break;

                case 0x1:
                    // nible 0 is 1
                    instruction_1(instruction);
                    break;

                case 0x2:
                    // nible 0 is 2
                    instruction_2(instruction);
                    break;

                case 0x3:
                    // nible 0 is 3
                    instruction_3(instruction);
                    break;

                case 0x4:
                    // nible 0 is 4
                    instruction_4(instruction);
                    break;

                case 0x5:
                    // nible 0 is 5
                    instruction_5(instruction);
                    break;

                case 0x6:
                    // nible 0 is 6
                    instruction_6(instruction);
                    break;

                case 0x7:
                    // nible 0 is 7
                    instruction_7(instruction);
                    break;

                case 0x8:
                    // nible 0 is 8
                    instruction_8(instruction);
                    break;

                case 0x9:
                    // nible 0 is 9
                    instruction_9(instruction);
                    break;

                case 0xA:
                    // nible 0 is A
                    instruction_A(instruction);
                    break;

                case 0xB:
                    // nible 0 is B
                    instruction_B(instruction);
                    break;

                case 0xC:
                    // nible 0 is C
                    instruction_C(instruction);
                    break;

                case 0xD:
                    // nible 0 is D
                    instruction_D(instruction);
                    break;

                case 0xE:
                    // nible 0 is E
                    instruction_E(instruction);
                    break;

                case 0xF:
                    // nible 0 is F
                    instruction_F(instruction);
                    break;
            }
        }

        private void instruction_0(ushort instruction)
        {
            // 0nnn is ingored by "modern interpreters" so we'll just ignore it
            // to become a modern interpreter.

            switch (instruction)
            {
                case 0x00E0:
                    screen.Clear();
                    break;

                case 0x00EE:
                    registers.PC = registers.Stack.Pop();
                    break;

                case 0x0000:
                    this.Kill();
                    // uh, there is no more program to execute lol
                    break;
            }
        }

        private void instruction_1(ushort instruction)
        {
            // 1 only has one instruction, jump to location.
            // program counter gets set to the last 3 nibbles.
            registers.PC = (ushort)(instruction & 0xFFF);
        }

        private void instruction_2(ushort instruction)
        {
            // Calls subroutine at nnn (last 3 nibbles)
            // The interpreter increments the stack pointer, then puts the current PC on the top of the stack. The PC is then set to nnn.
            registers.Stack.Push(registers.PC);
            registers.PC = (ushort)(instruction & 0xFFF);
        }

        private void instruction_3(ushort instruction)
        {
            // 3xkk
            // Skip next instruction if Vx = kk.
            // The interpreter compares register Vx to kk, and if they are equal, increments the program counter by 2.
            var x = ((instruction & 0xF00) >> 8);

            if(registers.V[x] == (byte)(instruction & 0xFF))
            {
                // PC always gets incremented so we just increment it once extra to increment it twice.
                this.registers.PC += 2;
            }
        }

        private void instruction_4(ushort instruction)
        {
            // 4xkk - SNE Vx, byte
            // Skip next instruction if Vx != kk.
            // The interpreter compares register Vx to kk, and if they are not equal, increments the program counter by 2.
            var x = ((instruction & 0xF00) >> 8);

            if (registers.V[x] != (byte)(instruction & 0xFF))
            {
                // PC always gets incremented so we just increment it once extra to increment it twice.
                this.registers.PC += 2;
            }
        }

        private void instruction_5(ushort instruction)
        {
            // 5xy0 - SE Vx, Vy
            // Skip next instruction if Vx = Vy.
            // The interpreter compares register Vx to register Vy, and if they are equal, increments the program counter by 2.

            var x = ((instruction & 0xF00) >> 8);
            var y = ((instruction & 0xF0) >> 4);

            if(registers.V[x] == registers.V[y])
            {
                this.registers.PC += 2;
            }
        }

        private void instruction_6(ushort instruction)
        {
            // 6xkk - LD Vx, byte
            // Set Vx = kk.
            // The interpreter puts the value kk into register Vx.

            var x = ((instruction & 0xF00) >> 8);
            var kk = ((instruction & 0xFF));

            registers.V[x] = (byte)kk;
        }

        private void instruction_7(ushort instruction)
        {
            // 7xkk - ADD Vx, byte
            // Set Vx = Vx + kk.
            // Adds the value kk to the value of register Vx, then stores the result in Vx.

            var x = ((instruction & 0xF00) >> 8);
            var kk = ((instruction & 0xFF));

            registers.V[x] += (byte)kk;
        }

        private void instruction_8(ushort instruction)
        {
            // 8xyi
            // a lot of bitwise stuff ig
            var x = ((instruction & 0x0F00) >> 8);
            var y = ((instruction & 0x00F0) >> 4);
            var i = (instruction & 0x000F);

            // do operations based on z
            switch (i)
            {
                case 0x0:
                    // store Vy in Vx
                    registers.V[x] = registers.V[y];
                    break;

                case 0x1:
                    // Set Vx = Vx OR Vy.
                    // Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx.
                    registers.V[x] |= registers.V[y];
                    break;

                case 0x2:
                    // Set Vx = Vx AND Vy.
                    // Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx.
                    registers.V[x] &= registers.V[y];
                    break;

                case 0x3:
                    // XOR
                    registers.V[x] ^= registers.V[y];
                    break;

                case 0x4:
                    // Set Vx = Vx + Vy, set VF = carry.
                    // The values of Vx and Vy are added together. 
                    // If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0.
                    // Only the lowest 8 bits of the result are kept, and stored in Vx.
                    var res = (registers.V[x] += registers.V[y]);

                    registers.V[0xF] = 0;
                    if (res > 0xFF)
                    {
                        registers.V[0xF] = 1;
                    }

                    registers.V[x] = res;
                    break;

                case 0x5:
                    // Set Vx = Vx - Vy, set VF = NOT borrow.
                    // If Vx > Vy, then VF is set to 1, otherwise 0.Then Vy is subtracted from Vx, and the results stored in Vx.
                    registers.V[0xF] = 0;

                    if (registers.V[x] > registers.V[y])
                    {
                        registers.V[0xF] = 1;
                    }

                    registers.V[x] -= registers.V[y];
                    break;

                case 0x6:
                    // Set Vx = Vx SHR 1.
                    // If the least - significant bit of Vx is 1, then VF is set to 1, otherwise 0.Then Vx is divided by 2.
                    registers.V[0xF] = (byte)(registers.V[x] & 0x1);
                    registers.V[x] >>= 1;
                    break;

                case 0x7:
                    // Set Vx = Vy - Vx, set VF = NOT borrow.
                    // If Vy > Vx, then VF is set to 1, otherwise 0.Then Vx is subtracted from Vy, and the results stored in Vx.

                    registers.V[0xF] = 0;
                    if (registers.V[y] > registers.V[x])
                    {
                        registers.V[0xF] = 1;
                    }

                    registers.V[x] = (byte)(registers.V[y] - registers.V[x]);
                    break;

                case 0xE:
                    // Set Vx = Vx SHL 1.
                    // If the most - significant bit of Vx is 1, then VF is set to 1, otherwise to 0.Then Vx is multiplied by 2.

                    registers.V[0xF] = (byte)(registers.V[x] & 0x80);
                    registers.V[x] <<= 1;
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

            var x = ((instruction & 0x0F00) >> 8);
            var y = ((instruction & 0x00F0) >> 4);

            if (registers.V[x] != registers.V[y])
            {
                // increase by 1 because we always increase
                this.registers.PC += 2;
            }
        }

        private void instruction_A(ushort instruction)
        {
            // Annn - LD I, addr
            // Set I = nnn.
            // The value of register I is set to nnn.
            registers.I = (ushort)(instruction & 0xFFF);
        }

        private void instruction_B(ushort instruction)
        {
            // Bnnn - JP V0, addr
            // Jump to location nnn + V0.
            // The program counter is set to nnn plus the value of V0.
            var nnn = (ushort)(instruction & 0x0FFF);
            registers.PC = (ushort)(nnn + registers.V[0]);
        }

        private void instruction_C(ushort instruction)
        {
            // Cxkk - RND Vx, byte
            // Set Vx = random byte AND kk.
            // The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk.
            // The results are stored in Vx. See instruction 8xy2 for more information on AND.

            var x = ((instruction & 0x0F00) >> 8);
            var kk = ((instruction & 0xFF));

            byte rand = (byte)(rng.Next(0, int.MaxValue) & 0xFF);
            registers.V[x] = (byte)(rand & kk);
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
            var x = ((instruction & 0x0F00) >> 8);
            var y = ((instruction & 0x00F0) >> 4);
            registers.V[0xF] = 0;
            var amount = (byte)(instruction & 0x000F);

            for (int row = 0; row < amount; row++)
            {
                var sprite = this.ram[registers.I + row];

                for(var col = 0; col < 8; col++)
                {
                    if((sprite & 0x80) > 0)
                    {
                        var erased = screen.Pixel(registers.V[x] + col, registers.V[y] + row);
                        if (erased)
                            registers.V[0xF] = 1;
                    }
                    sprite <<= 1;
                }
            }
        }

        private void instruction_E(ushort instruction)
        {
            var x = ((instruction & 0x0F00) >> 8);
            var last2nibs = (instruction & 0x00FF);

            if(last2nibs == 0x009E && keyboard.isKeyDown(registers.V[x])
                || last2nibs == 0x00A1 && keyboard.isKeyUp(registers.V[x]))
            {
                // every isntruction already increases PC so this would just increase it twice
                registers.PC += 2;
            }
        }

        private void instruction_F(ushort instruction)
        {
            var x = ((instruction & 0x0F00) >> 8);
            var i = (instruction & 0x00FF);

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
                    ram[loc] = (byte)(val / 100);
                    ram[loc + 1] = (byte)((val % 100) / 10);
                    ram[loc + 2] = (byte)(val % 10);
                    break;

                case 0x55:
                    var start = registers.I;

                    for(int index = 0; index <= x; index++)
                    {
                        ram[start + index] = registers.V[index];
                    }
                    break;

                case 0x65:
                    start = registers.I;

                    for (int index = 0; index <= x; index++)
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
