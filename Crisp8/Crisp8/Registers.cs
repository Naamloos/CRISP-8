using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp8
{
    internal struct Registers
    {
        /// <summary>
        /// V Registers (0-F)
        /// </summary>
        public byte[] V;

        /// <summary>
        /// Register I, generally used to store memory adresses. Only the lowest (rightmost) 12 bits are used
        /// </summary>
        public ushort I;

        /// <summary>
        /// Register PC (program counter), used by the CPU to store the current executing address.
        /// </summary>
        public ushort PC;

        /// <summary>
        /// Register SP (Stack pointer), used by the CPU to point to the topmost level of the stack.
        /// </summary>
        public byte SP;

        /// <summary>
        /// Delay Timer register.
        /// </summary>
        public byte DT;

        /// <summary>
        /// Sound Timer register.
        /// </summary>
        public byte ST;

        /// <summary>
        /// Stack is a register that consists of 16 short values to represent the adresses the interpreter should return to after finishing a subroutine.
        /// </summary>
        public Stack<ushort> Stack;
    }
}
