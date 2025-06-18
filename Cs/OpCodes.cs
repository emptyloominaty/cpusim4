using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CpuSim4 {

    public class OpCode {
        public string name;
        public byte bytes;
        public byte cycles;
        public OpCode(string name, byte bytes, byte cycles) {
            this.name = name;
            this.bytes = bytes;
            this.cycles = cycles;
        }
    }


    public class OpCodes {
        public OpCode[] codes = new OpCode[256];
        public Dictionary<string, byte> names = new Dictionary<string, byte>();


        public OpCodes() {
            codes[0] = new OpCode("STOP", 1, 0);
            codes[1] = new OpCode("ADD", 4, 0);
            codes[2] = new OpCode("SUB", 4, 0);
            codes[3] = new OpCode("LD1", 5, 0);
            codes[4] = new OpCode("ST1", 5, 0);
            codes[5] = new OpCode("LD2", 5, 0);
            codes[6] = new OpCode("ST2", 5, 0);
            codes[7] = new OpCode("LD3", 5, 0);
            codes[8] = new OpCode("ST3", 5, 0);
            codes[9] = new OpCode("LD4", 5, 0);
            codes[10] = new OpCode("ST4", 5, 0);
            codes[11] = new OpCode("LDI1", 3, 0);
            codes[12] = new OpCode("LDI2", 4, 0);
            codes[13] = new OpCode("LDI3", 5, 0);
            codes[14] = new OpCode("LDI4", 6, 0);
            codes[15] = new OpCode("INC", 2, 0);
            codes[16] = new OpCode("DEC", 2, 0);
            codes[17] = new OpCode("MUL", 4, 3);
            codes[18] = new OpCode("DIV", 4, 5);
            codes[19] = new OpCode("DIVR", 4, 5);
            codes[20] = new OpCode("ADC", 4, 0);
            codes[21] = new OpCode("SUC", 4, 0);
            codes[22] = new OpCode("NOP", 1, 0);
            codes[23] = new OpCode("JMP", 4, 0);
            codes[24] = new OpCode("JSR", 4, 0);
            codes[25] = new OpCode("RFS", 1, 0);
            codes[26] = new OpCode("JG", 6, 0);
            codes[27] = new OpCode("JL", 6, 0);
            codes[28] = new OpCode("JE", 6, 0);
            codes[29] = new OpCode("JC", 4, 0);
            codes[30] = new OpCode("JNG", 6, 0);
            codes[31] = new OpCode("JNL", 6, 0);
            codes[32] = new OpCode("JNE", 6, 0);
            codes[33] = new OpCode("JNC", 4, 0);
            codes[34] = new OpCode("PUSH1", 2, 0);
            codes[35] = new OpCode("PUSH2", 2, 0);
            codes[36] = new OpCode("PUSH3", 2, 0);
            codes[37] = new OpCode("PUSH4", 2, 0);
            codes[38] = new OpCode("POP1", 2, 0);
            codes[39] = new OpCode("POP2", 2, 0);
            codes[40] = new OpCode("POP3", 2, 0);
            codes[41] = new OpCode("POP4", 2, 0);
            codes[42] = new OpCode("MOV", 3, 0);
            codes[43] = new OpCode("ADDE", 3, 0);
            codes[44] = new OpCode("SUBE", 3, 0);
            codes[45] = new OpCode("ADDI1", 3, 0);
            codes[46] = new OpCode("ADDI2", 4, 0);
            codes[47] = new OpCode("ADDI3", 5, 0);
            codes[48] = new OpCode("ADDI4", 6, 0);
            codes[49] = new OpCode("SUBI1", 3, 0);
            codes[50] = new OpCode("SUBI2", 4, 0);
            codes[51] = new OpCode("SUBI3", 5, 0);
            codes[52] = new OpCode("SUBI4", 6, 0);
            codes[53] = new OpCode("MULI1", 3, 3);
            codes[54] = new OpCode("DIVI1", 3, 5);
            codes[55] = new OpCode("SEI", 1, 0);
            codes[56] = new OpCode("SDI", 1, 0);
            codes[57] = new OpCode("INT", 2, 0);
            codes[58] = new OpCode("RFI", 1, 0);
            codes[59] = new OpCode("HLT", 1, 0);
            codes[60] = new OpCode("LDR1", 3, 0);
            codes[61] = new OpCode("STR1", 3, 0);
            codes[62] = new OpCode("LDR2", 3, 0);
            codes[63] = new OpCode("STR2", 3, 0);
            codes[64] = new OpCode("LDR3", 3, 0);
            codes[65] = new OpCode("STR3", 3, 0);
            codes[66] = new OpCode("LDR4", 3, 0);
            codes[67] = new OpCode("STR4", 3, 0);
            codes[68] = new OpCode("ROL", 2, 0);
            codes[69] = new OpCode("ROR", 2, 0);
            codes[70] = new OpCode("SLL", 2, 0);
            codes[71] = new OpCode("SLR", 2, 0);
            codes[72] = new OpCode("SRR", 2, 0);
            codes[73] = new OpCode("AND", 4, 0);
            codes[74] = new OpCode("OR", 4, 0);
            codes[75] = new OpCode("XOR", 4, 0);
            codes[76] = new OpCode("NOT", 2, 0);
            codes[77] = new OpCode("CBT8", 2, 3);
            codes[78] = new OpCode("C8TB", 2, 3);

            codes[79] = new OpCode("SJG", 4, 0);
            codes[80] = new OpCode("SJL", 4, 0);
            codes[81] = new OpCode("SJE", 4, 0);
            codes[82] = new OpCode("SJC", 2, 0);
            codes[83] = new OpCode("SJNG", 4, 0);
            codes[84] = new OpCode("SJNL", 4, 0);
            codes[85] = new OpCode("SJNE", 4, 0);
            codes[86] = new OpCode("SJNC", 2, 0);
            codes[87] = new OpCode("SJMP", 2, 0);

            codes[88] = new OpCode("JGR", 4, 0);
            codes[89] = new OpCode("JLR", 4, 0);
            codes[90] = new OpCode("JER", 4, 0);
            codes[91] = new OpCode("JNGR", 4, 0);
            codes[92] = new OpCode("JNLR", 4, 0);
            codes[93] = new OpCode("JNER", 4, 0);
            codes[94] = new OpCode("JMPR", 2, 0);

            codes[100] = new OpCode("FLD", 5, 0);
            codes[101] = new OpCode("FST", 5, 0);
            codes[102] = new OpCode("FADD", 4, 2);
            codes[103] = new OpCode("FSUB", 4, 2);
            codes[104] = new OpCode("FMUL", 4, 19);
            codes[105] = new OpCode("FDIV", 4, 35);
            codes[106] = new OpCode("FIMOV", 3, 2); //float reg -> integer reg
            codes[107] = new OpCode("IFMOV", 3, 2); //integer reg -> float reg

            
            for (int i = 0; i < codes.Length; i++) {
                if (codes[i] != null) {
                    names[codes[i].name] = (byte)i;
                }

            }
        }
    }
}
