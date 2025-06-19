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

        public CacheSet[] cacheD;
        public CacheSet[] cacheI;

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
            cacheI = new CacheSet[128];
            cacheD = new CacheSet[128];
            for (int i = 0; i < cacheI.Length; i++) {
                cacheI[i] = new CacheSet();
            }
            for (int i = 0; i < cacheD.Length; i++) {
                cacheD[i] = new CacheSet();
            }

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
            PipelineStage ps = new PipelineStage { op = 22 };
            byte opcode;

            //try to load opcode
            if (!ReadByteCache(pc, cacheI, out opcode)) {
                // miss opcode
                FetchCacheLine(pc, cacheI);
                return ps; //nop
            }

            int instrLen = opCodes[opcode].bytes;
            ps.op = opcode;
            ps.cyclesToExecute = (byte)(1 + opCodes[opcode].cycles);
            //try to load other bytes
            for (int i = 1; i < instrLen; i++) {
                if (!ReadByteCache(pc + i, cacheI, out byte _)) {
                    // miss other bytes
                    FetchCacheLine(pc + i, cacheI);
                    ps.op = 22;
                    return ps;  //nop
                }
            }
            //fetch instruction
            ReadByteCache(pc, cacheI, out ps.op);
            if (instrLen > 1) ReadByteCache(pc + 1, cacheI, out ps.arg1);
            if (instrLen > 2) ReadByteCache(pc + 2, cacheI, out ps.arg2);
            if (instrLen > 3) ReadByteCache(pc + 3, cacheI, out ps.arg3);
            if (instrLen > 4) ReadByteCache(pc + 4, cacheI, out ps.arg4);
            if (instrLen > 5) ReadByteCache(pc + 5, cacheI, out ps.arg5);
            if (instrLen > 6) ReadByteCache(pc + 6, cacheI, out ps.arg6); 

            registers[33] += instrLen;
            return ps;
        }


        public void Decode(PipelineStage ps) {
            if (ps.op == 22) {
                return;
            }

            //TODO:
            ps.address2 = 0;
            ps.address1 = 0;
            ps.address0 = 0;
        }

        public void Execute(PipelineStage ps) {
            if (ps.op == 22) {
                return;
            }
        }


        //Cache
        public bool ReadByteCache(int address, CacheSet[] cache, out byte value) {
            int offset = address & 0b111;
            int index = (address >> 3) & 0x7F;
            int tag = address >> 10;

            var set = cache[index];
            for (int i = 0; i < 2; i++) {
                var line = set.lines[i];
                if (line.valid && line.tag == tag) {
                    value = line.data[offset];
                    return true; 
                }
            }

            value = 0;
            return false; 
        }

        public void FetchCacheLine(int address, CacheSet[] cache) {
            int alignedAddr = address & ~0b111;
            int index = (address >> 3) & 0x7F;
            int tag = address >> 10;

            var set = cache[index];

            Random rng = new Random();
            int replaceIdx = -1;

            for (int i = 0; i < 2; i++) {
                if (!set.lines[i].valid) {
                    replaceIdx = i;
                    break;
                }
            }

            if (replaceIdx == -1) {
                replaceIdx = rng.Next(2);
            }

            var line = new CacheLine {
                valid = true,
                tag = tag,
                data = new byte[8],
            };

            for (int i = 0; i < 8; i++) {
                line.data[i] = Memory.Read(alignedAddr + i);
            }
                
            set.lines[replaceIdx] = line;
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

        public int address2; //address +2
        public int address1; //address +1
        public int address0; //address +0

        public byte cyclesToExecute; 
        public byte cyclesExecuted;
    }

    public struct CacheLine {
        public bool valid;
        public int tag;
        public byte[] data;
    }

    public class CacheSet {
        public CacheLine[] lines = new CacheLine[2];
    }



}


