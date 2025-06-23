using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpuSim4.Devices {
    public class Storage : Device {
        public byte[] storageData;
        public Storage(byte type, byte id, int burstStartAddress, int burstSize, int size, byte burstEnabled)
            : base(type, id, burstStartAddress, burstSize, burstEnabled) {
            storageData = new byte[size];
            App.storages.Add(new StorageWindow(id));
        }


        public override void Run() {
            while (true) {
                bool interrupt = false;
                int address = Functions.ConvertTo32Bit(Memory.Data[startAddress + 27], Memory.Data[startAddress + 28], Memory.Data[startAddress + 29], Memory.Data[startAddress + 30]);

                if (Memory.Read(startAddress + 26) == 1) { //read

                    for (int i = burstStartAddress; i < burstStartAddress + burstSize; i++) {
                        Memory.Write(deviceAddress + burstStartAddress + i, storageData[address + i]);
                    }

                    interrupt = true;
                    resetInstructionReg();
                } else if (Memory.Read(startAddress + 26) == 2) { //write

                    for (int i = burstStartAddress; i < burstStartAddress + burstSize; i++) {
                        Write(address + i, Memory.Read(deviceAddress + burstStartAddress + i));
                    }

                    interrupt = true;
                    resetInstructionReg();
                }

                if (interrupt) {
                    Interrupt((byte)(0));
                }

                Thread.Sleep(10);
            }
        }

        void resetInstructionReg() {
            Memory.Write(startAddress + 26, 0, true);
        }

        void Write(int address, byte data) {
            if (address >= 0 && address < storageData.Length) {
                storageData[address] = data;
            }
        }

    }
}
