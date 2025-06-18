using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CpuSim4 {
    public class Cpu {
        public Thread cpuThread;

        public OpCode[] opcodes;

        public long clock = 0;

        public bool cpuRunning = false;
        public bool threadRunning = true;

        public Cpu(OpCode[] opCodes) {
            this.opCodes = opCodes;
       
        }
}
