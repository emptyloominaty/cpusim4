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
using System.Windows.Documents;

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

        public CacheLine[] cacheD;
        public CacheLine[] cacheI;

        public long timeClockA = 0;
        public long timeClockB = 0;
        public long timeClockC = 0;
        public long timeClockD = 0;

        public long timeStart;
        public long timeEnd;
        public long timeA;
        public long timeB;
        public long timeC;
        public long timeD;

        public long cyclesDone = 0;
        public long cyclesDoneA = 0;
        public long cyclesDoneB = 0;
        public long instructionsDone = 0;

        public int cyclesTotal = 0;
        public int cyclesExecuting = 0;

        public PipelineStage[] pipeline = new PipelineStage[3];

        public Cpu(OpCode[] opCodes) {
            this.opCodes = opCodes;
            Reset();
        }
        public void Reset() {
            cacheI = new CacheLine[256];
            cacheD = new CacheLine[256];

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

            timeClockA = DateTime.Now.Ticks * 100;
            timeClockB = DateTime.Now.Ticks * 100;
            timeClockC = 0;
            timeClockD = 0;

            cyclesDone = 0;
            cyclesDoneA = 0;
            cyclesDoneB = 0;
            instructionsDone = 0;

            cyclesTotal = 0;
            cyclesExecuting = 0;
            pipeline = new PipelineStage[3];
            pipeline[0] = new PipelineStage { op = 22 };
            pipeline[1] = new PipelineStage { op = 22 };
            pipeline[2] = new PipelineStage { op = 22 };
        }

        public void StartCpu() {
            cpuThread = new Thread(new ThreadStart(Run));
            cpuThread.IsBackground = true;
            cpuThread.Start();
        }

        public void Run() {
            
            while (threadRunning) {
                long timeClockWait = 1000000000 / clockSet;
                while (cpuRunning) {
                    if (maxClock) {
                        Main();
                    } else {
                        timeClockA = DateTime.Now.Ticks * 100;
                        Main();
                        timeClockD = 0;
                        while (timeClockD < timeClockWait) {
                            timeClockB = DateTime.Now.Ticks * 100;
                            timeClockD = timeClockB - timeClockA;
                        }
                    }
                }
                try {
                    Thread.Sleep(100);
                } catch (ThreadInterruptedException e) {
                    throw new Exception("InterruptedException occurred", e);
                }
            }
        }

        public void Main() {
            cyclesDoneA++;
            timeA = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            if (timeA - timeB > 1000) {
                clock = cyclesDoneA - cyclesDoneB;
                timeB = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                cyclesDoneB = cyclesDoneA;
                if (timeA - timeD > 5000) {
                    cyclesTotal /= 2;
                    cyclesExecuting /= 2;
                    timeD = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }
            }
            if (timeA - timeC > 33.3) {
                timeC = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                timeEnd = timeC;
            }
            cyclesTotal++;
            if (halted) {
                return;
            }


            cyclesExecuting++;
            cyclesDone++;


            Execute(pipeline[2]);
            Decode(pipeline[1]);

            pipeline[2] = pipeline[1];
            pipeline[1] = pipeline[0];
            pipeline[0] = Fetch();
        }


        public PipelineStage Fetch() {
            int pc = registers[33];


            PipelineStage ps = new PipelineStage { };

            return ps;
        }


        public void Decode(PipelineStage ps) {
            if (ps.op == 22) {
                return;
            }
        }

        public void Execute(PipelineStage ps) {
            if (ps.op == 22) {
                return;
            }
        }



        public void StopCpu() {
            cpuRunning = false;
        }

    }

    public struct PipelineStage {
        public byte op;
        public byte arg1;
        public byte arg2;
        public byte arg3;
        public byte arg4;
        public byte arg5;
        public byte arg6;

        public int address3; //address +3
        public int address2; //address +2
        public int address1; //address +1
        public int address0; //address +0

        public byte cyclesToExecute; //0=1
        public byte cyclesExecuted;
    }

    public struct CacheLine {
        public bool valid;
        public int tag;
        public byte[] data;
    }

}


