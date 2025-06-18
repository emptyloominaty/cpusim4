using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CpuSim4 {
    public class Cpu {
        public Thread cpuThread;
        public OpCode[] opCodes;

        public long clock = 0;

        public bool cpuRunning = false;
        public bool threadRunning = true;

        public bool halted = false;
        public bool interruptHw = false;

        public bool maxClock = true;
        public long clockSet = 100000;

        public int[] registers;
        public float[] registersF;

        public Cpu(OpCode[] opCodes) {
            this.opCodes = opCodes;
            Reset();
        }
        public void Reset() {
            registers = new int[38];
            for (int i = 0; i < registers.Length; i++) {
                registers[i] = 0;
            }
            registers[33] = 7340032;
            registers[34] = 8192;
            registers[35] = 0;
            registers[36] = 0;
            registers[37] = 0;

            registersF = new float[32];
            for (int i = 0; i < registersF.Length; i++) {
                registersF[i] = 0f;
            }

            interruptHw = false;
            halted = false;
        }
    }
}
