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
            codes[57] = new OpCode("INT", 2, 2);
            codes[58] = new OpCode("RFI", 1, 1);
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
            codes[72] = new OpCode("SAR", 2, 0);
            codes[73] = new OpCode("AND", 4, 0);
            codes[74] = new OpCode("OR", 4, 0);
            codes[75] = new OpCode("XOR", 4, 0);
            codes[76] = new OpCode("NOT", 2, 0);
            codes[77] = new OpCode("CBT8", 2, 5);
            codes[78] = new OpCode("C8TB", 2, 5);

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
            codes[108] = new OpCode("FCOMP", 4, 5);

            codes[109] = new OpCode("NOP", 1, 0);
            codes[110] = new OpCode("NOP", 1, 0);
            codes[111] = new OpCode("NOP", 1, 0);
            codes[112] = new OpCode("NOP", 1, 0);
            codes[113] = new OpCode("NOP", 1, 0);
            codes[114] = new OpCode("NOP", 1, 0);
            codes[115] = new OpCode("NOP", 1, 0);
            codes[116] = new OpCode("NOP", 1, 0);
            codes[117] = new OpCode("NOP", 1, 0);
            codes[118] = new OpCode("NOP", 1, 0);
            codes[119] = new OpCode("NOP", 1, 0);
            codes[120] = new OpCode("NOP", 1, 0);
            codes[121] = new OpCode("NOP", 1, 0);
            codes[122] = new OpCode("NOP", 1, 0);
            codes[123] = new OpCode("NOP", 1, 0);
            codes[124] = new OpCode("NOP", 1, 0);
            codes[125] = new OpCode("NOP", 1, 0);
            codes[126] = new OpCode("NOP", 1, 0);
            codes[127] = new OpCode("NOP", 1, 0);
            codes[128] = new OpCode("NOP", 1, 0);
            codes[129] = new OpCode("NOP", 1, 0);
            codes[130] = new OpCode("NOP", 1, 0);
            codes[131] = new OpCode("NOP", 1, 0);
            codes[132] = new OpCode("NOP", 1, 0);
            codes[133] = new OpCode("NOP", 1, 0);
            codes[134] = new OpCode("NOP", 1, 0);
            codes[135] = new OpCode("NOP", 1, 0);
            codes[136] = new OpCode("NOP", 1, 0);
            codes[137] = new OpCode("NOP", 1, 0);
            codes[138] = new OpCode("NOP", 1, 0);
            codes[139] = new OpCode("NOP", 1, 0);
            codes[140] = new OpCode("NOP", 1, 0);
            codes[141] = new OpCode("NOP", 1, 0);
            codes[142] = new OpCode("NOP", 1, 0);
            codes[143] = new OpCode("NOP", 1, 0);
            codes[144] = new OpCode("NOP", 1, 0);
            codes[145] = new OpCode("NOP", 1, 0);
            codes[146] = new OpCode("NOP", 1, 0);
            codes[147] = new OpCode("NOP", 1, 0);
            codes[148] = new OpCode("NOP", 1, 0);
            codes[149] = new OpCode("NOP", 1, 0);
            codes[150] = new OpCode("NOP", 1, 0);
            codes[151] = new OpCode("NOP", 1, 0);
            codes[152] = new OpCode("NOP", 1, 0);
            codes[153] = new OpCode("NOP", 1, 0);
            codes[154] = new OpCode("NOP", 1, 0);
            codes[155] = new OpCode("NOP", 1, 0);
            codes[156] = new OpCode("NOP", 1, 0);
            codes[157] = new OpCode("NOP", 1, 0);
            codes[158] = new OpCode("NOP", 1, 0);
            codes[159] = new OpCode("NOP", 1, 0);
            codes[160] = new OpCode("NOP", 1, 0);
            codes[161] = new OpCode("NOP", 1, 0);
            codes[162] = new OpCode("NOP", 1, 0);
            codes[163] = new OpCode("NOP", 1, 0);
            codes[164] = new OpCode("NOP", 1, 0);
            codes[165] = new OpCode("NOP", 1, 0);
            codes[166] = new OpCode("NOP", 1, 0);
            codes[167] = new OpCode("NOP", 1, 0);
            codes[168] = new OpCode("NOP", 1, 0);
            codes[169] = new OpCode("NOP", 1, 0);
            codes[170] = new OpCode("NOP", 1, 0);
            codes[171] = new OpCode("NOP", 1, 0);
            codes[172] = new OpCode("NOP", 1, 0);
            codes[173] = new OpCode("NOP", 1, 0);
            codes[174] = new OpCode("NOP", 1, 0);
            codes[175] = new OpCode("NOP", 1, 0);
            codes[176] = new OpCode("NOP", 1, 0);
            codes[177] = new OpCode("NOP", 1, 0);
            codes[178] = new OpCode("NOP", 1, 0);
            codes[179] = new OpCode("NOP", 1, 0);
            codes[180] = new OpCode("NOP", 1, 0);
            codes[181] = new OpCode("NOP", 1, 0);
            codes[182] = new OpCode("NOP", 1, 0);
            codes[183] = new OpCode("NOP", 1, 0);
            codes[184] = new OpCode("NOP", 1, 0);
            codes[185] = new OpCode("NOP", 1, 0);
            codes[186] = new OpCode("NOP", 1, 0);
            codes[187] = new OpCode("NOP", 1, 0);
            codes[188] = new OpCode("NOP", 1, 0);
            codes[189] = new OpCode("NOP", 1, 0);
            codes[190] = new OpCode("NOP", 1, 0);
            codes[191] = new OpCode("NOP", 1, 0);
            codes[192] = new OpCode("NOP", 1, 0);
            codes[193] = new OpCode("NOP", 1, 0);
            codes[194] = new OpCode("NOP", 1, 0);
            codes[195] = new OpCode("NOP", 1, 0);
            codes[196] = new OpCode("NOP", 1, 0);
            codes[197] = new OpCode("NOP", 1, 0);
            codes[198] = new OpCode("NOP", 1, 0);
            codes[199] = new OpCode("NOP", 1, 0);
            codes[200] = new OpCode("NOP", 1, 0);
            codes[201] = new OpCode("NOP", 1, 0);
            codes[202] = new OpCode("NOP", 1, 0);
            codes[203] = new OpCode("NOP", 1, 0);
            codes[204] = new OpCode("NOP", 1, 0);
            codes[205] = new OpCode("NOP", 1, 0);
            codes[206] = new OpCode("NOP", 1, 0);
            codes[207] = new OpCode("NOP", 1, 0);
            codes[208] = new OpCode("NOP", 1, 0);
            codes[209] = new OpCode("NOP", 1, 0);
            codes[210] = new OpCode("NOP", 1, 0);
            codes[211] = new OpCode("NOP", 1, 0);
            codes[212] = new OpCode("NOP", 1, 0);
            codes[213] = new OpCode("NOP", 1, 0);
            codes[214] = new OpCode("NOP", 1, 0);
            codes[215] = new OpCode("NOP", 1, 0);
            codes[216] = new OpCode("NOP", 1, 0);
            codes[217] = new OpCode("NOP", 1, 0);
            codes[218] = new OpCode("NOP", 1, 0);
            codes[219] = new OpCode("NOP", 1, 0);
            codes[220] = new OpCode("NOP", 1, 0);
            codes[221] = new OpCode("NOP", 1, 0);
            codes[222] = new OpCode("NOP", 1, 0);
            codes[223] = new OpCode("NOP", 1, 0);
            codes[224] = new OpCode("NOP", 1, 0);
            codes[225] = new OpCode("NOP", 1, 0);
            codes[226] = new OpCode("NOP", 1, 0);
            codes[227] = new OpCode("NOP", 1, 0);
            codes[228] = new OpCode("NOP", 1, 0);
            codes[229] = new OpCode("NOP", 1, 0);
            codes[230] = new OpCode("NOP", 1, 0);
            codes[231] = new OpCode("NOP", 1, 0);
            codes[232] = new OpCode("NOP", 1, 0);
            codes[233] = new OpCode("NOP", 1, 0);
            codes[234] = new OpCode("NOP", 1, 0);
            codes[235] = new OpCode("NOP", 1, 0);
            codes[236] = new OpCode("NOP", 1, 0);
            codes[237] = new OpCode("NOP", 1, 0);
            codes[238] = new OpCode("NOP", 1, 0);
            codes[239] = new OpCode("NOP", 1, 0);
            codes[240] = new OpCode("NOP", 1, 0);
            codes[241] = new OpCode("NOP", 1, 0);
            codes[242] = new OpCode("NOP", 1, 0);
            codes[243] = new OpCode("NOP", 1, 0);
            codes[244] = new OpCode("NOP", 1, 0);
            codes[245] = new OpCode("NOP", 1, 0);
            codes[246] = new OpCode("NOP", 1, 0);
            codes[247] = new OpCode("NOP", 1, 0);
            codes[248] = new OpCode("NOP", 1, 0);
            codes[249] = new OpCode("NOP", 1, 0);
            codes[250] = new OpCode("NOP", 1, 0);
            codes[251] = new OpCode("NOP", 1, 0);
            codes[252] = new OpCode("NOP", 1, 0);
            codes[253] = new OpCode("NOP", 1, 0);
            codes[254] = new OpCode("NOP", 1, 0);
            codes[255] = new OpCode("NOP", 1, 0);


            for (int i = 0; i < codes.Length; i++) {
                if (codes[i] != null) {
                    names[codes[i].name] = (byte)i;
                }

            }
        }
    }
}
