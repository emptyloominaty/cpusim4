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

        public bool maxClock = false;
        public long clockSet = 1;

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

        public bool debugCpu = true;

        public PipelineStage[] pipeline = new PipelineStage[3];
        public bool pipelineStalled;

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
            pipelineStalled = false;
        }

        public void StartCpu() {
            Console.WriteLine("CPU STARTED");
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

            Execute(ref pipeline[2]);
            Decode(ref pipeline[1]);

            if (!pipelineStalled) {
                pipeline[2] = pipeline[1];
                pipeline[1] = pipeline[0];
                pipeline[0] = Fetch();
            }

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
            ps.cyclesToExecute = opCodes[opcode].cycles;
            ps.cyclesExecuted = 0;
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

            if (ps.op == 23 || ps.op == 24 || ps.op == 25 || ps.op == 57 || ps.op == 58) {
                registers[33] = Functions.ConvertTo24Bit(ps.arg1, ps.arg2, ps.arg3);
            } else if ((ps.op <= 26 && ps.op >= 33) || (ps.op >= 94 && ps.op <= 79)) {
                //TODO: Branch Predict
            }

            //prefetch next line
            if (!ReadByteCache(pc + 8, cacheI, out byte _)) {
                FetchCacheLine(pc + 8, cacheI);
            }
                return ps;
        }


        public void Decode(ref PipelineStage ps) {
            //LoadRegisters() DONT (sim performance)
        }

        byte[] store = new byte[4];
        byte[] bytes = new byte[4];

        public void Execute(ref PipelineStage ps) {
            if (debugCpu) {
                App.cpuDebug += " PC:" + (registers[33]).ToString("X6") + " " + instructionsDone + ": " + opCodes[ps.op].name + " " + ps.arg1.ToString("X2") + " " + ps.arg2.ToString("X2") + " " + ps.arg3.ToString("X2") + " " + ps.arg4.ToString("X2") + " " + ps.arg5.ToString("X2")
                    + " - registers - " + " " + registers[0] + " " + registers[1] + " " + registers[2] + " pipeline: " + opCodes[pipeline[0].op].name + " - " + opCodes[pipeline[1].op].name + " - " + opCodes[pipeline[2].op].name +" Stall: "+pipelineStalled+ Environment.NewLine;
            }

            if (ps.cyclesToExecute!=ps.cyclesExecuted) {
                ps.cyclesExecuted++;
                pipelineStalled = true;
                return;
            }


            if (ps.op == 22) {
                return;
            } else if (ps.op == 0) {
                StopCpu();
            }
            pipelineStalled = false;

            bool success;
            int val;
            int load;
            byte byte1;
            byte byte2;
            byte byte3;
            byte byte4;
            int address1;
            int address2;
            int address3;
            int address4;
            int a;
            int b;
            uint ua;
            uint ub;
            uint uval;


            switch (ps.op) {
                case 1: //ADD
                    a = registers[ps.arg1];
                    b = registers[ps.arg2];
                    val = a + b;
                    registers[ps.arg3] = val;
                    //carry
                    ua = (uint)a;
                    ub = (uint)b;
                    uval = ua + ub;
                    registers[35] = (uval < ua || uval < ub) ? 1 : 0;
                    // overflow
                    registers[36] = (((a ^ val) & (b ^ val)) < 0) ? 1 : 0;
                    break;
                case 2: //SUB
                    a = registers[ps.arg1];
                    b = registers[ps.arg2];
                    val = a - b;
                    registers[ps.arg3] = val;
                    //carry
                    registers[35] = ((uint)a >= (uint)b) ? 1 : 0;
                    //overflow
                    registers[36] = (((a ^ b) & (a ^ val)) < 0) ? 1 : 0;
                    break;
                case 3: //LD1
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    if (ReadByteCache(address1, cacheD, out byte1)) {
                        registers[ps.arg1] = byte1;
                    } else {
                        //miss
                        pipelineStalled = true;
                        FetchCacheLine(address1, cacheD);
                        return;
                    }
                    break;
                case 4: //ST1
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    WriteByteCache(address1, cacheD, (byte)registers[ps.arg1]);
                    break;

                case 5: //LD2
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    val = LoadBytes(address1, 2, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                    } else {
                        return;
                    }

                    break;
                case 6: //ST2
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    Functions.ConvertFrom16Bit(registers[ps.arg1],store);
                    WriteByteCache(address1, cacheD, store[0]);
                    WriteByteCache(address1+1, cacheD, store[1]);
                    break;

                case 7: //LD3
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    val = LoadBytes(address1, 3, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                    } else {
                        return;
                    }

                    break;
                case 8: //ST3
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    Functions.ConvertFrom24Bit(registers[ps.arg1], store);
                    WriteByteCache(address1, cacheD, store[0]);
                    WriteByteCache(address1 + 1, cacheD, store[1]);
                    WriteByteCache(address1 + 2, cacheD, store[2]);
                    break;

                case 9: //LD4
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    val = LoadBytes(address1, 4, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                    } else {
                        return;
                    }

                    break;
                case 10: //ST4
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    Functions.ConvertFrom32Bit(registers[ps.arg1], store);
                    WriteByteCache(address1, cacheD, store[0]);
                    WriteByteCache(address1 + 1, cacheD, store[1]);
                    WriteByteCache(address1 + 2, cacheD, store[2]);
                    WriteByteCache(address1 + 3, cacheD, store[3]);
                    break;
                case 11: //LDI1
                    registers[ps.arg1] = ps.arg2;
                    break;
                case 12: //LDI2
                    registers[ps.arg1] = Functions.ConvertTo16Bit(ps.arg2, ps.arg3);
                    break;
                case 13: //LDI3
                    registers[ps.arg1] = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    break;
                case 14: //LDI4
                    registers[ps.arg1] = Functions.ConvertTo32Bit(ps.arg2, ps.arg3, ps.arg4, ps.arg5);
                    break;
                case 15: //INC
                    registers[ps.arg1]++;
                    break;
                case 16: //DEC
                    registers[ps.arg1]--;
                    break;
                case 17: //MUL
                    val = registers[ps.arg1] * registers[ps.arg2];
                    registers[ps.arg3] = val;
                    break;
                case 18: //DIV
                    if (registers[ps.arg2] != 0) {
                        val = registers[ps.arg1] / registers[ps.arg2];
                    } else {
                        val = 0;
                    }
                    registers[ps.arg3] = val;
                    break;
                case 19: //DIVR
                    if (registers[ps.arg2] != 0) {
                        val = registers[ps.arg1] % registers[ps.arg2];
                    } else {
                        val = 0;
                    }
                    registers[ps.arg3] = val;
                    break;
                case 20: //ADC
                    val = registers[ps.arg1] + registers[ps.arg2];
                    if (registers[35] == 1) {
                        val++;
                    }
                    registers[ps.arg3] = val;
                    //TODO: carry
                    break;
                case 21: //SUC
                    val = registers[ps.arg1] - registers[ps.arg2];
                    if (registers[35] == 1) {
                        val--;
                    }
                    registers[ps.arg3] = val;
                    break;
                case 22: //NOP
                    break;
                case 23: //JMP
                    registers[33] = Functions.ConvertTo24Bit(ps.arg1, ps.arg2, ps.arg3);
                    FlushPipeline();
                    break;
            }

            instructionsDone++;
        }

        public void FlushPipeline() {
            pipeline[1] = new PipelineStage { op = 22 };
            pipeline[0] = new PipelineStage { op = 22 };
        }


        //Cache
        public int LoadBytes(int baseAddress, int count, out bool success) {
            int result = 0;
            for (int i = 0; i < count; i++) {
                if (ReadByteCache(baseAddress + i, cacheD, out byte b)) {
                    result = (result << 8) | b;
                } else {
                    FetchCacheLine(baseAddress + i, cacheD);
                    pipelineStalled = true;
                    success = false;
                    return 0;
                }
            }
            success = true;
            return result;
        }

        public void WriteByteCache(int address, CacheSet[] cache, byte value) {
            int offset = address & 0b111;
            int index = (address >> 3) & 0x7F;
            int tag = address >> 10;

            var set = cache[index];
            for (int i = 0; i < 2; i++) {
                if (set.lines[i].valid && set.lines[i].tag == tag) {
                    set.lines[i].data[offset] = value; 
                    break;
                }
            }

           Memory.Write(address,value); 
        }

        public byte deviceReadWait = 0;
        public byte deviceReadWait2 = 0;
        public bool deviceWait = false;

        public bool ReadByteCache(int address, CacheSet[] cache, out byte value) {
            //uncached memory mapped I/O
            if (address > 0x7FFFFF) {
                value = 0;
                if (!deviceWait) {
                    deviceWait = true;
                    deviceReadWait2 = 1;
                }
                if (deviceReadWait == deviceReadWait2) {
                    deviceWait = false;
                    value = Memory.Read(address);
                    deviceReadWait2 = 0;
                    deviceReadWait = 0;
                    pipelineStalled = false;
                } else {
                    pipelineStalled = true;
                    deviceReadWait++;
                }
                    return true;
            }

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


