using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
        public int interruptId = 0;

        public bool maxClock = false;
        public long clockSet = 2;

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


        public bool debugCpu = false;

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

            interruptId = 0;
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
            fetchingStalled = false;
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

            if (cpuRunning && !pipelineStalled) {
                pipeline[2] = pipeline[1];
                pipeline[1] = pipeline[0];
                pipeline[0] = Fetch();
            }

            if (debugCpu) {
                PipelineStage ps = pipeline[2];
                App.cpuDebug += " PC:" + (pcFetch).ToString("X6") +"("+ opCodes[opFetched].name + ")"+ " | " + instructionsDone + ": " + opCodes[ps.op].name + " " + ps.arg1.ToString("X2") + " " + ps.arg2.ToString("X2") + " " + ps.arg3.ToString("X2") + " " + ps.arg4.ToString("X2") + " " + ps.arg5.ToString("X2")
                    + " - registers - " + " " + registers[0] + " " + registers[1] + " " + registers[2] + " pipeline: " + opCodes[pipeline[0].op].name + " - " + opCodes[pipeline[1].op].name + " - " + opCodes[pipeline[2].op].name + " Stall: " + (fetchingStalled ? "Y" : "N") + " / " + (pipelineStalled ? "Y" : "N") +" | Stack:"+ registers[34].ToString("X6")+ Environment.NewLine;
            }

        }

        public bool fetchingStalled = false;
        public PipelineStage Fetch() {
            int pc = registers[33];
            pcFetch = pc;
            PipelineStage ps = new PipelineStage { op = 22 };
            byte opcode;
            opFetched = 22;
            if (fetchingStalled || halted) {
                return ps;
            }

            if (interruptHw) {
                fetchingStalled = true;
                interruptHw = false;
                ps.op = 57;
                ps.arg1 = (byte)interruptId;
                return ps;
            }

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
            opFetched = opcode;
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

            if (ps.op == 23) { //JMP
                registers[33] = Functions.ConvertTo24Bit(ps.arg1, ps.arg2, ps.arg3);
            } else if (ps.op == 24) { //JSR
                ps.returnAddress = registers[33];
                registers[33] = Functions.ConvertTo24Bit(ps.arg1, ps.arg2, ps.arg3);
            } else if (ps.op == 25 || ps.op == 58) { //RFS,RFI
                fetchingStalled = true;
            } else if (ps.op == 57 ) { //INT
                fetchingStalled = true;
            } else if (ps.op == 94) { //JMPR
                fetchingStalled = true;
            } else if (ps.op == 87) { //SJMP
                registers[33] += ps.arg1;
            } else if ((ps.op >= 26 && ps.op <= 33)) { //Conditional jumps
                int jumpAddress = Functions.ConvertTo24Bit(ps.arg3, ps.arg4, ps.arg5);
                PredictBranch(ref ps, jumpAddress);
            } else if (ps.op >= 88 && ps.op <= 93) { //Indirect conditional jumps
                fetchingStalled = true;
            } else if (ps.op >= 79 && ps.op <= 86) { //Short conditional jumps
                int jumpAddress = registers[33] + ps.arg3 - 128;
                PredictBranch(ref ps, jumpAddress);
            } else if (ps.op == 59) {
                fetchingStalled = true;
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
        byte opFetched = 22;
        int pcFetch = 0;

        public void Execute(ref PipelineStage ps) {
            if (ps.cyclesToExecute!=ps.cyclesExecuted) {
                ps.cyclesExecuted++;
                pipelineStalled = true;
                return;
            }

            pipelineStalled = false;
            if (ps.op == 22) {
                return;
            } else if (ps.op == 0) {
                StopCpu();
            }


            bool success;
            int val;
            byte byte1;
            int address1;
            int a;
            int b;
            uint ua;
            uint ub;
            uint uval;
            long temp;
            float fa;
            float fb;


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
                    a = registers[ps.arg1];
                    b = registers[ps.arg2];
                    temp = (long)(uint)a + (uint)b + registers[35];
                    val = (int)temp;
                    registers[ps.arg3] = val;
                    //carry
                    registers[35] = (temp >> 32) != 0 ? 1 : 0;
                    //overflow
                    registers[36] = (((a ^ val) & (b ^ val)) < 0) ? 1 : 0;
                    break;
                case 21: //SUC
                    a = registers[ps.arg1];
                    b = registers[ps.arg2];
                    temp = (long)(uint)a - (uint)b - registers[35];
                    val = (int)temp;
                    registers[ps.arg3] = val;
                    //carry
                    registers[35] = ((ulong)(uint)a < ((ulong)(uint)b + (ulong)registers[35])) ? 1 : 0;
                    //overflow
                    registers[36] = (((a ^ b) & (a ^ val)) < 0) ? 1 : 0;
                    break;
                case 22: //NOP
                    break;
                case 23: //JMP
                    /*registers[33] = Functions.ConvertTo24Bit(ps.arg1, ps.arg2, ps.arg3);
                    FlushPipeline();*/
                    break;
                case 24: //JSR
                    Functions.ConvertFrom24Bit(ps.returnAddress, bytes);
                    WriteByteCache(registers[34], cacheD, bytes[0]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[1]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[2]);
                    registers[34]++;
                    break;
                case 25: //RFS
                    val = LoadBytes(registers[34]-3, 3, out success);
                    if (success) {
                        registers[33] = val;
                        registers[34] -= 3;
                        fetchingStalled = false;
                        //ps.op = 22; //fix for infinite stall
                    } else {
                        return;
                    }
                    break;
                case 26: //JG
                    success = registers[ps.arg1] > registers[ps.arg2];
                    ConditionalJump(ref ps, success);
                    break;
                case 27: //JL
                    success = registers[ps.arg1] < registers[ps.arg2];
                    ConditionalJump(ref ps, success);
                    break;
                case 28: //JE
                    success = registers[ps.arg1] == registers[ps.arg2];
                    ConditionalJump(ref ps, success);
                    break;
                case 29: //JC
                    success = registers[35] == 1;
                    ConditionalJump(ref ps, success);
                    break;
                case 30: //JNG
                    success = !(registers[ps.arg1] > registers[ps.arg2]);
                    ConditionalJump(ref ps, success);
                    break;
                case 31: //JNL
                    success = !(registers[ps.arg1] < registers[ps.arg2]);
                    ConditionalJump(ref ps, success);
                    break;
                case 32: //JNE
                    success = !(registers[ps.arg1] == registers[ps.arg2]);
                    ConditionalJump(ref ps, success);
                    break;
                case 33: //JNC
                    success = registers[35] == 0;
                    ConditionalJump(ref ps, success);
                    break;
                case 34: //PSH1
                    Functions.ConvertFrom24Bit(registers[ps.arg1], bytes);
                    WriteByteCache(registers[34], cacheD, bytes[2]);
                    registers[34]++;
                    break;
                case 35: //PSH2
                    Functions.ConvertFrom24Bit(registers[ps.arg1], bytes);
                    WriteByteCache(registers[34], cacheD, bytes[1]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[2]);
                    registers[34]++;
                    break;
                case 36: //PSH3
                    Functions.ConvertFrom24Bit(registers[ps.arg1], bytes);
                    WriteByteCache(registers[34], cacheD, bytes[0]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[1]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[2]);
                    registers[34]++;
                    break;
                case 37: //PSH4
                    Functions.ConvertFrom32Bit(registers[ps.arg1], bytes);
                    WriteByteCache(registers[34], cacheD, bytes[0]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[1]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[2]);
                    registers[34]++;
                    WriteByteCache(registers[34], cacheD, bytes[3]);
                    registers[34]++;
                    break;
                case 38: //POP1
                    val = LoadBytes(registers[34] - 1, 1, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                        registers[34] -= 1;
                    } else {
                        return;
                    }
                    break;
                case 39: //POP2
                    val = LoadBytes(registers[34] - 2, 2, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                        registers[34] -= 2;
                    } else {
                        return;
                    }
                    break;
                case 40: //POP3
                    val = LoadBytes(registers[34] - 3, 3, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                        registers[34] -= 3;
                    } else {
                        return;
                    }
                    break;
                case 41: //POP4
                    val = LoadBytes(registers[34] - 4, 4, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                        registers[34] -= 4;
                    } else {
                        return;
                    }
                    break;
                case 42: //MOV
                    registers[ps.arg2] = registers[ps.arg1];
                    break;
                case 43: //ADDE
                    val = registers[ps.arg1] + registers[ps.arg2];
                    registers[ps.arg1] = val;
                    break;
                case 44: //SUE
                    val = registers[ps.arg1] - registers[ps.arg2];
                    registers[ps.arg1] = val;
                    break;
                case 45: //ADDI1
                    val = registers[ps.arg1] + ps.arg2;
                    registers[ps.arg1] = val;
                    break;
                case 46: //ADDI2
                    val = registers[ps.arg1] + Functions.ConvertTo16Bit(ps.arg2, ps.arg3);
                    registers[ps.arg1] = val;
                    break;
                case 47: //ADDI3
                    val = registers[ps.arg1] + Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    registers[ps.arg1] = val;
                    break;
                case 48: //ADDI4
                    val = registers[ps.arg1] + Functions.ConvertTo32Bit(ps.arg2, ps.arg3, ps.arg4, ps.arg5);
                    registers[ps.arg1] = val;
                    break;
                case 49: //SUBI1
                    val = registers[ps.arg1] - ps.arg2;
                    registers[ps.arg1] = val;
                    break;
                case 50: //SUBI2
                    val = registers[ps.arg1] - Functions.ConvertTo16Bit(ps.arg2, ps.arg3);
                    registers[ps.arg1] = val;
                    break;
                case 51: //SUBI3
                    val = registers[ps.arg1] - Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    registers[ps.arg1] = val;
                    break;
                case 52: //SUBI4
                    val = registers[ps.arg1] - Functions.ConvertTo32Bit(ps.arg2, ps.arg3, ps.arg4, ps.arg5);
                    registers[ps.arg1] = val;
                    break;
                case 53: //MULI1
                    val = registers[ps.arg1] * ps.arg2;
                    registers[ps.arg1] = val;
                    break;
                case 54: //DIVI1
                    if (registers[ps.arg2] != 0) {
                        val = registers[ps.arg1] / ps.arg2;
                    } else {
                        val = 0;
                    }
                    registers[ps.arg1] = val;
                    break;
                case 55: //SEI
                    registers[37] = 0;
                    break;
                case 56: //DEI
                    registers[37] = 1;
                    break;
                case 57: //INT 
                    if (registers[37] == 1) {
                        return;
                    }
                    val = LoadBytes(ps.arg1 * 3, 3, out success);
                    if (success) {
                        Functions.ConvertFrom24Bit(registers[33], bytes);
                        WriteByteCache(registers[34], cacheD, bytes[0]);
                        registers[34]++;
                        WriteByteCache(registers[34], cacheD, bytes[1]);
                        registers[34]++;
                        WriteByteCache(registers[34], cacheD, bytes[2]);
                        registers[34]++;

                        WriteByteCache(registers[34], cacheD, (byte)registers[35]);
                        registers[34]++;
                        WriteByteCache(registers[34], cacheD, (byte)registers[36]);
                        registers[34]++;
                        if (val!=0) {
                            registers[33] = val;
                        }
                        fetchingStalled = false;
                        FlushPipeline();
                    } else {
                        return;
                    }
                    
                    break;
                case 58: //RFI
                    val = LoadBytes(registers[34] - 1, 1, out success);
                    if (success) {
                        registers[34]--;
                        registers[36] = val;
                    }
                    val = LoadBytes(registers[34] - 1, 1, out success);
                    if (success) {
                        registers[34]--;
                        registers[35] = val;
                    }
                    val = LoadBytes(registers[34] - 3, 3, out success);
                    if (success) {
                        registers[34]-= 3;
                        registers[33] = val;
                        fetchingStalled = false;
                    }
                    break;
                case 59: //HLT
                    halted = true;
                    fetchingStalled = true;
                    registers[37] = 0;
                    FlushPipeline();
                    break;
                case 60: //LDR1
                    if (ReadByteCache(registers[ps.arg2], cacheD, out byte1)) {
                        registers[ps.arg1] = byte1;
                    } else {
                        //miss
                        pipelineStalled = true;
                        FetchCacheLine(registers[ps.arg2], cacheD);
                        return;
                    }
                    break;
                case 61: //STR1
                    WriteByteCache(registers[ps.arg2], cacheD, (byte)registers[ps.arg1]);
                    break;
                case 62: //LDR2
                    val = LoadBytes(registers[ps.arg2], 2, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                    } else {
                        return;
                    }
                    break;
                case 63: //STR2
                    Functions.ConvertFrom16Bit(registers[ps.arg1], store);
                    WriteByteCache(registers[ps.arg2], cacheD, store[0]);
                    WriteByteCache(registers[ps.arg2] + 1, cacheD, store[1]);
                    break;
                case 64: //LDR3
                    val = LoadBytes(registers[ps.arg2], 3, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                    } else {
                        return;
                    }
                    break;
                case 65: //STR3
                    Functions.ConvertFrom24Bit(registers[ps.arg1], store);
                    WriteByteCache(registers[ps.arg2], cacheD, store[0]);
                    WriteByteCache(registers[ps.arg2] + 1, cacheD, store[1]);
                    WriteByteCache(registers[ps.arg2] + 2, cacheD, store[2]);
                    break;
                case 66: //LDR4
                    val = LoadBytes(registers[ps.arg2], 4, out success);
                    if (success) {
                        registers[ps.arg1] = val;
                    } else {
                        return;
                    }
                    break;
                case 67: //STR4
                    Functions.ConvertFrom32Bit(registers[ps.arg1], store);
                    WriteByteCache(registers[ps.arg2], cacheD, store[0]);
                    WriteByteCache(registers[ps.arg2] + 1, cacheD, store[1]);
                    WriteByteCache(registers[ps.arg2] + 2, cacheD, store[2]);
                    WriteByteCache(registers[ps.arg2] + 2, cacheD, store[3]);
                    break;
                case 68: //ROL
                    val = (registers[ps.arg1] << 1) | (registers[ps.arg1] >> (32 - 1));
                    registers[ps.arg1] = val;
                    break;
                case 69: //ROR
                    val = (registers[ps.arg1] >> 1) | (registers[ps.arg1] << (32 - 1));
                    registers[ps.arg1] = val;
                    break;
                case 70: //SLL
                    val = registers[ps.arg1];
                    App.cpu.registers[35] = (val & 0x80000000) != 0 ? 1 : 0;
                    registers[ps.arg1] = val << 1;
                    break;
                case 71: //SLR
                    val = registers[ps.arg1];
                    App.cpu.registers[35] = (val & 0x00000001) != 0 ? 1 : 0;
                    registers[ps.arg1] = (int)((uint)val >> 1);
                    break;
                case 72: //SAR
                    val = registers[ps.arg1] >> 1;
                    registers[ps.arg1] = val;
                    break;
                case 73: //AND
                    val = registers[ps.arg1] & registers[ps.arg2];
                    registers[ps.arg3] = val;
                    break;
                case 74: //OR
                    val = registers[ps.arg1] | registers[ps.arg2];
                    registers[ps.arg3] = val;
                    break;
                case 75: //XOR
                    val = registers[ps.arg1] ^ registers[ps.arg2];
                    registers[ps.arg3] = val;
                    break;
                case 76: //NOT
                    val = ~registers[ps.arg1];
                    registers[ps.arg1] = val;
                    break;
                case 77: //CBT8
                    if (ps.arg1 > 24) {
                        return;
                    }
                    val = registers[ps.arg1];
                    registers[ps.arg1] = val & 0x01;
                    registers[ps.arg1 + 1] = (val >> 1) & 0x01;
                    registers[ps.arg1 + 2] = (val >> 2) & 0x01;
                    registers[ps.arg1 + 3] = (val >> 3) & 0x01;
                    registers[ps.arg1 + 4] = (val >> 4) & 0x01;
                    registers[ps.arg1 + 5] = (val >> 5) & 0x01;
                    registers[ps.arg1 + 6] = (val >> 6) & 0x01;
                    registers[ps.arg1 + 7] = (val >> 7) & 0x01;
                    break;
                case 78: //C8TB
                    if (ps.arg1 > 24) {
                        return;
                    }
                    val = registers[ps.arg1];
                    val |= registers[ps.arg1 + 1] << 1;
                    val |= registers[ps.arg1 + 2] << 2;
                    val |= registers[ps.arg1 + 3] << 3;
                    val |= registers[ps.arg1 + 4] << 4;
                    val |= registers[ps.arg1 + 5] << 5;
                    val |= registers[ps.arg1 + 6] << 6;
                    val |= registers[ps.arg1 + 7] << 7;
                    registers[ps.arg1] = val;
                    break;
                case 79: //SJG  
                    success = registers[ps.arg1] > registers[ps.arg2];
                    ConditionalJump(ref ps, success);
                    break;
                case 80: //SJL  
                    success = registers[ps.arg1] < registers[ps.arg2];
                    ConditionalJump(ref ps, success);
                    break;
                case 81: //SJE  
                    success = registers[ps.arg1] == registers[ps.arg2];
                    ConditionalJump(ref ps, success);
                    break;
                case 82: //SJC
                    success = registers[35] == 1;
                    ConditionalJump(ref ps, success);
                    break;
                case 83: //SJNG
                    success = !(registers[ps.arg1] > registers[ps.arg2]);
                    ConditionalJump(ref ps, success);
                    break;
                case 84: //SJNL
                    success = !(registers[ps.arg1] < registers[ps.arg2]);
                    ConditionalJump(ref ps, success);
                    break;
                case 85: //SJNE
                    success = !(registers[ps.arg1] == registers[ps.arg2]);
                    ConditionalJump(ref ps, success);
                    break;
                case 86: //SJNC
                    success = registers[35] == 0;
                    ConditionalJump(ref ps, success);
                    break;
                case 87: //SJMP
                    break;
                case 88: //JGR
                    if (registers[ps.arg1] > registers[ps.arg2]) {
                        registers[33] = registers[ps.arg3];
                    }
                    fetchingStalled = false;
                    break;
                case 89: //JLR
                    if (registers[ps.arg1] < registers[ps.arg2]) {
                        registers[33] = registers[ps.arg3];
                    }
                    fetchingStalled = false;
                    break;
                case 90: //JER
                    if (registers[ps.arg1] == registers[ps.arg2]) {
                        registers[33] = registers[ps.arg3];
                    }
                    fetchingStalled = false;
                    break;
                case 91: //JNGR
                    if (!(registers[ps.arg1] > registers[ps.arg2])) {
                        registers[33] = registers[ps.arg3];
                    }
                    fetchingStalled = false;
                    break;
                case 92: //JNLR
                    if (!(registers[ps.arg1] < registers[ps.arg2])) {
                        registers[33] = registers[ps.arg3];
                    }
                    fetchingStalled = false;
                    break;
                case 93: //JNER
                    if (!(registers[ps.arg1] == registers[ps.arg2])) {
                        registers[33] = registers[ps.arg3];
                    }
                    fetchingStalled = false;
                    break;
                case 94: //JMPR
                    registers[33] = registers[ps.arg1];
                    fetchingStalled = false;
                    break;
                case 95: //JCR
                    if (registers[35] == 1) {
                        registers[33] = registers[ps.arg1];
                    }
                    fetchingStalled = false;
                    break;
                case 96: //JNCR
                    if (registers[35] == 0) {
                        registers[33] = registers[ps.arg1];
                    }
                    fetchingStalled = false;
                    break;
                case 97: //BT
                    registers[35] = (registers[ps.arg1] >> ps.arg2) & 1;
                    break;
                case 98: //BTS
                    registers[35] = (registers[ps.arg1] >> ps.arg2) & 1;
                    registers[ps.arg1] = registers[ps.arg1] | (1 << ps.arg2);
                    break;
                case 99: //BTR
                    registers[35] = (registers[ps.arg1] >> ps.arg2) & 1;
                    registers[ps.arg1] = registers[ps.arg1] & ~(1 << ps.arg2);
                    break;
                case 100: //FLD
                    byte1 = GetFPRegister(ps.arg1);
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    val = LoadBytes(address1, 4, out success);
                    if (success) {
                        registersF[byte1] = BitConverter.ToSingle(BitConverter.GetBytes(val), 0);
                    } else {
                        return;
                    }
                    break;
                case 101: //FST
                    byte1 = GetFPRegister(ps.arg1);
                    address1 = Functions.ConvertTo24Bit(ps.arg2, ps.arg3, ps.arg4);
                    Functions.ConvertFloatToBytes(registersF[byte1], store);
                    WriteByteCache(address1, cacheD, store[0]);
                    WriteByteCache(address1 + 1, cacheD, store[1]);
                    WriteByteCache(address1 + 2, cacheD, store[2]);
                    WriteByteCache(address1 + 3, cacheD, store[3]);
                    break;
                case 102: //FADD
                    registersF[GetFPRegister(ps.arg3)] = registersF[GetFPRegister(ps.arg1)] + registersF[GetFPRegister(ps.arg2)];
                    break;
                case 103: //FSUB
                    registersF[GetFPRegister(ps.arg3)] = registersF[GetFPRegister(ps.arg1)] - registersF[GetFPRegister(ps.arg2)];
                    break;
                case 104: //FMUL
                    registersF[GetFPRegister(ps.arg3)] = registersF[GetFPRegister(ps.arg1)] * registersF[GetFPRegister(ps.arg2)];
                    break;
                case 105: //FDIV
                    fa = registersF[GetFPRegister(ps.arg1)];
                    fb = registersF[GetFPRegister(ps.arg2)];

                    if (fb != 0) {
                        registersF[GetFPRegister(ps.arg3)] = fa / fb;
                    } else {
                        registersF[GetFPRegister(ps.arg3)] = 0;
                    }
                    break;
                case 106: //FIMOV
                    registers[ps.arg2] = (int)(registersF[GetFPRegister(ps.arg1)]);
                    break;
                case 107: //IFMOV
                    registersF[GetFPRegister(ps.arg2)] = (float)(registers[ps.arg1]);
                    break;
                case 108: //FCOMP
                    fa = registersF[GetFPRegister(ps.arg1)];
                    fb = registersF[GetFPRegister(ps.arg2)];
                    if (fa > fb) {
                        registers[ps.arg3] = 1;
                    } else if (fa < fb) {
                        registers[ps.arg3] = -1;
                    } else {
                        registers[ps.arg3] = 0;
                    }
                    break;

            }



            instructionsDone++;
        }

        public byte GetFPRegister(byte input) {
            if (input>=64 && input<=95) {
                return (byte)(input - 64);
            }
            return 0;
        }

        public void ConditionalJump(ref PipelineStage ps, bool success) {  
            if (success && !ps.branchTaken) {
                registers[33] = Functions.ConvertTo24Bit(ps.arg3, ps.arg4, ps.arg5);
                FlushPipeline();
            } else if (!success && ps.branchTaken) {
                registers[33] = ps.returnAddress;
                FlushPipeline();
            }
        }

        public void FlushPipeline() {
            pipeline[1] = new PipelineStage { op = 22 };
            pipeline[0] = new PipelineStage { op = 22 };
            fetchingStalled = false;
        }

        public void PredictBranch(ref PipelineStage ps, int jumpAddress) {
            if (jumpAddress > registers[33]) {
                ps.branchTaken = false;
            } else {
                ps.returnAddress = registers[33];
                registers[33] = jumpAddress;
                ps.branchTaken = true;
            }
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

        public void Interrupt(byte id) {
            if (registers[37] == 0) {
                interruptHw = true;
                interruptId = id;
                halted = false;
                FlushPipeline();
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

        public int returnAddress;

        public byte cyclesToExecute; 
        public byte cyclesExecuted;

        public bool branchTaken;
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


