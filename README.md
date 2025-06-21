## CPU
* Architecture: **EMP 4 32-Bit**
* **Load–store** architecture
* 3-Stage Pipeline
* **BTFNT** Branch prediction
* ALU Width: **32-bit** 
* FPU Width: **32-bit** 
* Address Width: **24-bit**
* Bus: **64-bit** Data + **24-bit** Address

* Cache
	- **2kB** Instruction Cache 
	- **2kB** Data Cache
	- **8B** cache line,  **2-way** set associative
	

* Registers: 
    - 32 32-Bit Registers (**r0-r31**)  <br/>
    - RESERVED (**r32**)
    - Program Counter (**r33**)
    - Stack Pointer (**r34**) 
    - Carry (**r35**)
    - Overflow (**r36**)
    - Interrupt Disabled  (**r37**)
	- 32 32-bit Floating Point Registers (**r64-r95**)
    <br/>
* IPC: ****
* Stack: **8192 Bytes**

        
|               |  Start   |   End    |
|---------------|:--------:|:--------:|
| Interrupt Pointers | 0x000000 | 0x0002FF |
| OS RAM        | 0x000300 | 0x001FFF |
| Stack         | 0x002000 | 0x003FFF |
| App RAM       | 0x004000 | 0x3FEFFF |
| Functions RAM | 0x3FF000 | 0x3FFFFF |
| App ROM       | 0x400000 | 0x5FFFFF | 
| Functions ROM | 0x600000 | 0x6FFFFF | 
| OS ROM        | 0x700000 | 0x7FFFFF |
| Devices       | 0x800000 | 0xFFFFFF |

Reset Vector: 0x700000


### Instructions
| OP - Name  | Cycles | Bytes |     | Byte1 | Byte2 |  Byte3  |  Byte4  |  Byte5  | Byte6 |
|------------|:------:|:-----:|:---:|:-----:|:-----:|:-------:|:-------:|:-------:|:-----:|   
| 0 - STOP   |   1    |   1   |  -  | 0x00  |   -   |    -    |    -    |    -    |   -   |
| 1 - ADD    |   1    |   4   |  -  | 0x01  |  reg  |   reg   |   reg   |    -    |   -   |
| 2 - SUB    |   1    |   4   |  -  | 0x02  |  reg  |   reg   |   reg   |    -    |   -   |
| 3 - LD1    |   1    |   5   |  -  | 0x03  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 4 - ST1    |   1    |   5   |  -  | 0x04  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 5 - LD2    |   1    |   5   |  -  | 0x05  |  reg  | mem2 |  mem1  |  mem0  |   -   |
| 6 - ST2    |   1    |   5   |  -  | 0x06  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 7 - LD3    |   1    |   5   |  -  | 0x07  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 8 - ST3    |   1    |   5   |  -  | 0x08  |  reg  |  mem2  |  mem1  |  mem0  |   -   |  
| 9 - LD4    |   1    |   5   |  -  | 0x09  |  reg  |  mem2  |  mem1  |  mem0  |   -   |
| 10 - ST4    |   1    |   5   |  -  | 0x0a  |  reg  |  mem2  |  mem1  |  mem0  |   -   | 
| 11 - LDI1   |   1    |   3   |  -  | 0x0b  |  reg  |  value  |    -    |    -    |   -   |  
| 12 - LDI2  |   1    |   4   |  -  | 0x0c  |  reg  | value1 | value0 |    -    |   -   |  
| 13 - LDI3  |   1    |   5   |  -  | 0x0d  |  reg  | value2 | value1 | value0 |   -   | 
| 14 - LDI4  |   1    |   6   |  -  | 0x0e  |  reg  | value3 | value2 | value1 |   value0   | 
| 15 - INC   |   1    |   2   |  -  | 0x0f  |  reg  |    -    |    -    |    -    |   -   |
| 16 - DEC   |   1    |   2   |  -  | 0x10  |  reg  |    -    |    -    |    -    |   -   |
| 17 - MUL   |   4   |   4   |  -  | 0x11  |  reg  |   reg   |   reg   |    -    |   -   |
| 18 - DIV   |   6   |   4   |  -  | 0x12  |  reg  |   reg   |   reg   |    -    |   -   |
| 19 - DIVR  |   6   |   4   |  -  | 0x13  |  reg  |   reg   |   reg   |    -    |   -   |
| 20 - ADC   |   1    |   4   |  -  | 0x14  |  reg  |   reg   |   reg   |    -    |   -   |
| 21 - SUC   |   1    |   4   |  -  | 0x15  |  reg  |   reg   |   reg   |    -    |   -   |
| 22 - NOP   |   1    |   1   |  -  | 0x16  |   -   |    -    |    -    |    -    |   -   |
| 23 - JMP   |   1    |   4   |  -  | 0x17  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 24 - JSR   |   1    |   4   |  -  | 0x18  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 25 - RFS   |   1    |   1   |  -  | 0x19  |   -   |    -    |    -    |    -    |   -   |
| 26 - JG    |   1    |   6   |  -  | 0x1a  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 27 - JL    |   1    |   6   |  -  | 0x1b  |  reg  |   reg   |  mem2  |  mem1  | mem0 | 
| 28 - JE    |   1    |   6   |  -  | 0x1c  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 29 - JC    |   1    |   4   |  -  | 0x1d  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 30 - JNG   |   1    |   6   |  -  | 0x1e  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 31 - JNL   |   1    |   6   |  -  | 0x1f  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 32 - JNE   |   1    |   6   |  -  | 0x20  |  reg  |   reg   |  mem2  |  mem1  | mem0 |
| 33 - JNC   |   1    |   4   |  -  | 0x21  | mem2 |  mem1  |  mem0  |    -    |   -   |
| 34 - PUSH1  |   1    |   2   |  -  | 0x22  |  reg  |    -    |    -    |    -    |   -   |
| 35 - PUSH2  |   1    |   2   |  -  | 0x23  |  reg  |    -    |    -    |    -    |   -   |
| 36 - PUSH3  |   1    |   2   |  -  | 0x24  |  reg  |    -    |    -    |    -    |   -   |
| 37 - PUSH4  |   1    |   2   |  -  | 0x25  |  reg  |    -    |    -    |    -    |   -   |
| 38 - POP1  |   1    |   2   |  -  | 0x26  |  reg  |    -    |    -    |    -    |   -   |
| 39 - POP2  |   1    |   2   |  -  | 0x27  |  reg  |    -    |    -    |    -    |   -   |
| 40 - POP3  |   1    |   2   |  -  | 0x28  |  reg  |    -    |    -    |    -    |   -   |
| 41 - POP4  |   1    |   2   |  -  | 0x29  |  reg  |    -    |    -    |    -    |   -   |
| 42 - MOV   |   1    |   3   |  -  | 0x2a  |  reg  |   reg   |    -    |    -    |   -   |
| 43 - ADDE  |   1    |   3   |  -  | 0x2b  |  reg  |   reg   |    -    |    -    |   -   |
| 44 - SUBE  |   1    |   3   |  -  | 0x2c  |  reg  |   reg   |    -    |    -    |   -   |
| 45 - ADDI1 |   1    |   3   |  -  | 0x2d  |  reg  |  value  |    -    |    -    |   -   |
| 46 - ADDI2 |   1    |   4   |  -  | 0x2e  |  reg  | value1 | value0 |    -    |   -   |
| 47 - ADDI3 |   1    |   5   |  -  | 0x2f  |  reg  | value2 | value1 | value0 |   -   |
| 48 - ADDI4 |   1    |   6   |  -  | 0x30  |  reg  | value3 | value2 | value1 |   value0   |
| 49 - SUBI1 |   1    |   3   |  -  | 0x31  |  reg  |  value  |    -    |    -    |   -   |
| 50 - SUBI2 |   1    |   4   |  -  | 0x32  |  reg  | value1 | value0 |    -    |   -   |
| 51 - SUBI3 |   1    |   5   |  -  | 0x33  |  reg  | value2 | value1 | value0 |   -   |
| 52 - SUBI4 |   1    |   6   |  -  | 0x34  |  reg  | value3 | value2 | value1 |   value0   |
| 53 - MULI1 |   4   |   3   |  -  | 0x35  |  reg  |  value  |    -    |    -    |   -   |
| 54 - DIVI1 |   6   |   3   |  -  | 0x36  |  reg  |  value  |    -    |    -    |   -   |
| 55 - SEI   |   1    |   1   |  -  | 0x37  |   -   |    -    |    -    |    -    |   -   |
| 56 - SDI   |   1    |   1   |  -  | 0x38  |   -   |    -    |    -    |    -    |   -   |
| 57 - INT   |   5    |   2   |  -  | 0x39  |  ip   |    -    |    -    |    -    |   -   |
| 58 - RFI   |   5    |   1   |  -  | 0x3a  |   -   |    -    |    -    |    -    |   -   |
| 59 - HLT   |   1    |   1   |  -  | 0x3b  |   -   |    -    |    -    |    -    |   -   |
| 60 - LDR1  |   1    |   3   |  -  | 0x3c  |  reg  |   reg   |    -    |    -    |   -   |
| 61 - STR1  |   1    |   3   |  -  | 0x3d  |  reg  |   reg   |    -    |    -    |   -   |
| 62 - LDR2  |   1    |   3   |  -  | 0x3e  |  reg  |   reg   |    -    |    -    |   -   |
| 63 - STR2  |   1    |   3   |  -  | 0x3f  |  reg  |   reg   |    -    |    -    |   -   |
| 64 - LDR3  |   1    |   3   |  -  | 0x40  |  reg  |   reg   |    -    |    -    |   -   |
| 65 - STR3  |   1    |   3   |  -  | 0x41  |  reg  |   reg   |    -    |    -    |   -   |
| 66 - LDR4  |   1    |   3   |  -  | 0x42  |  reg  |   reg   |    -    |    -    |   -   |
| 67 - STR4  |   1    |   3   |  -  | 0x43  |  reg  |   reg   |    -    |    -    |   -   |
| 68 - ROL   |   1    |   2   |  -  | 0x44  |  reg  |    -    |    -    |    -    |   -   |
| 69 - ROR   |   1    |   2   |  -  | 0x45  |  reg  |    -    |    -    |    -    |   -   |
| 70 - SLL   |   1    |   2   |  -  | 0x46  |  reg  |    -    |    -    |    -    |   -   |
| 71 - SLR   |   1    |   2   |  -  | 0x47  |  reg  |    -    |    -    |    -    |   -   |
| 72 - SRR   |   1    |   2   |  -  | 0x48  |  reg  |    -    |    -    |    -    |   -   |
| 73 - AND   |   1    |   4   |  -  | 0x49  |  reg  |   reg   |   reg   |    -    |   -   |
| 74 - OR    |   1    |   4   |  -  | 0x4a  |  reg  |   reg   |   reg   |    -    |   -   |
| 75 - XOR   |   1    |   4   |  -  | 0x4b  |  reg  |   reg   |   reg   |    -    |   -   |
| 76 - NOT   |   1    |   2   |  -  | 0x4c  |  reg  |    -    |    -    |    -    |   -   |
| 77 - CBT8  |   6   |   2   |  -  | 0x4d  |  reg  |    -    |    -    |    -    |   -   |
| 78 - C8TB  |   6    |   2   |  -  | 0x4e  |  reg  |    -    |    -    |    -    |   -   |
| 79 - SJG    |   1    |   4   |  -  | 0x4f  |  reg  |   reg   |  mem  |  -  | - |
| 80 - SJL    |   1    |   4   |  -  | 0x50  |  reg  |   reg   |  mem  |  -  | - | 
| 81 - SJE    |   1    |   4   |  -  | 0x51  |  reg  |   reg   |  mem  |  -  | - |
| 82 - SJC    |   1    |   2   |  -  | 0x52  | mem |  -  |  - |    -    |   -   |
| 83 - SJNG   |   1    |   4   |  -  | 0x53  |  reg  |   reg   |  mem  |  -  | - |
| 84 - SJNL   |   1    |   4   |  -  | 0x54  |  reg  |   reg   |  mem  |  -  | - |
| 85 - SJNE   |   1    |   4   |  -  | 0x55  |  reg  |   reg   |  mem  |  -  | - |
| 86 - SJNC   |   1    |   2   |  -  | 0x56  | mem |  -  |  -  |    -    |   -   |
| 87 - SJMP   |   1    |   2   |  -  | 0x57  | mem |  -  |  -  |    -    |   -   |
| 88 - JGR    |   1    |   4   |  -  | 0x58  |  reg  |   reg   |  reg  |  -  | - |
| 89 - JLR    |   1    |   4   |  -  | 0x59  |  reg  |   reg   |  reg  |  -  | - | 
| 90 - JER    |   1    |   4   |  -  | 0x5a  |  reg  |   reg   |  reg  |  -  | - |
| 91 - JNGR   |   1    |   4   |  -  | 0x5b  |  reg  |   reg   |  reg  |  -  | - |
| 92 - JNLR   |   1    |   4   |  -  | 0x5c  |  reg  |   reg   |  reg  |  -  | - |
| 93 - JNER   |   1    |   4   |  -  | 0x5d  |  reg  |   reg   |  reg  |  -  | - |
| 94 - JMPR   |   1    |   2   |  -  | 0x5e  | reg |  -  |  -  |    -    |   -   |
| 100 - FLD   |   1    |   5   |  -  | 0x64  | reg[F] |  mem2  |  mem1  |    mem0    |   -   |
| 101 - FST   |   1    |   5   |  -  | 0x65  | reg[F] |  mem2  |  mem1  |    mem0    |   -   |
| 102 - FADD   |   3    |   4   |  -  | 0x66  | reg[F] |  reg[F]  |  reg[F]  |    -    |   -   |
| 103 - FSUB   |   3    |   4   |  -  | 0x67  | reg[F] |  reg[F]  |  reg[F]  |    -    |   -   |
| 104 - FMUL   |   20    |   4   |  -  | 0x68  | reg[F] |  reg[F]  |  reg[F]  |    -    |   -   |
| 105 - FDIV   |   36    |   4   |  -  | 0x69  | reg[F] |  reg[F]  |  reg[F] |    -    |   -   |
| 106 - FIMOV   |   3    |   3   |  -  | 0x6a  | reg[F] |  reg  |  -  |    -    |   -   |
| 107 - IFMOV   |   3    |   3   |  -  | 0x6b  | reg |  reg[F]  |  -  |    -    |   -   |
| 108 - FCOMP   |   6    |   4   |  -  | 0x6c  | reg[F] |  reg[F]  |  reg  |    -    |   -   |


### Interrupts

|  IP   | Interrupt Name |
|:-----:|:--------------:|
| 0-127 |      OS      | 
| 128-255   |   Devices    | 

    
# Memory
* 4MB RAM
* 4MB ROM