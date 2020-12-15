using System;
using System.Collections.Generic;
using System.Text;

namespace Crisp8
{
    public struct Registers
    {
        /// <summary>
        /// Register 0
        /// </summary>
        public byte V0;

        /// <summary>
        /// Register 1
        /// </summary>
        public byte V1;

        /// <summary>
        /// Register 2
        /// </summary>
        public byte V2;

        /// <summary>
        /// Register 3
        /// </summary>
        public byte V3;

        /// <summary>
        /// Register 4
        /// </summary>
        public byte V4;

        /// <summary>
        /// Register 5
        /// </summary>
        public byte V5;

        /// <summary>
        /// Register 6
        /// </summary>
        public byte V6;

        /// <summary>
        /// Register 7
        /// </summary>
        public byte V7;

        /// <summary>
        /// Register 8
        /// </summary>
        public byte V8;

        /// <summary>
        /// Register 9
        /// </summary>
        public byte V9;

        /// <summary>
        /// Register A
        /// </summary>
        public byte VA;

        /// <summary>
        /// Register B
        /// </summary>
        public byte VB;

        /// <summary>
        /// Register C
        /// </summary>
        public byte VC;

        /// <summary>
        /// Register D
        /// </summary>
        public byte VD;

        /// <summary>
        /// Register E
        /// </summary>
        public byte VE;

        /// <summary>
        /// Register F, also often used as flags.
        /// </summary>
        public byte VF;


        /// <summary>
        /// Register I, generally used to store memory adresses. Only the lowest (rightmost) 12 bits are used
        /// </summary>
        public short I;


        /// <summary>
        /// Register PC (program counter), used by the CPU to store the current executing address.
        /// </summary>
        public short PC;

        /// <summary>
        /// Register SP (Stack pointer), used by the CPU to point to the topmost level of the stack.
        /// </summary>
        public byte SP;
    }
}
