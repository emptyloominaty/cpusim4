using CpuSim4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CpuSim4 {
    public class Device {
        public Thread deviceThread;
        public int startAddress;
        public int deviceAddress;
        public byte id = 0;
        public byte type = 0;
        public int burstStartAddress;
        public int burstSize;
        public Device(byte type, byte id, int burstStartAddress, int burstSize, byte burstEnabled) {
            this.burstStartAddress = burstStartAddress;
            this.burstSize = burstSize;

            startAddress = 0x800000 + (0x80000 * id);
            deviceAddress = startAddress;
            this.id = id;
            this.type = type;
            //Memory.Data

            for (int i = 0; i <= 0xFF; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id)) + i] = true;
            }
            for (int i = burstStartAddress; i <= burstStartAddress + burstSize; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id)) + i] = true;
            }

            Memory.Data[startAddress + 0] = type; //type 1=co-processor 2=storage 3=network 4=vram+display, 5=gpu, 6=user storage port, 7=timer, 8=keyboard
            Memory.Data[startAddress + 1] = id; //id
            Memory.Data[startAddress + 2] = 1; // 1
            Memory.Data[startAddress + 3] = 0; //
            Memory.Data[startAddress + 4] = 0; //
            Memory.Data[startAddress + 5] = 0; //
            Memory.Data[startAddress + 6] = 0; //
            byte[] bytes = new byte[2];
            Functions.ConvertFrom16Bit(burstStartAddress, bytes);
            Memory.Data[startAddress + 7] = burstEnabled; //Burst IO enabled
            Memory.Data[startAddress + 8] = 0; //InputStartAddress CPU->Device
            Memory.Data[startAddress + 9] = 0; //OutputStartAddress Device->CPU

            Functions.ConvertFrom16Bit(burstSize, bytes);
            Memory.Data[startAddress + 10] = bytes[0]; //IOSize

            Memory.Data[startAddress + 11] = 0; //CpuReady Int6
            Memory.Data[startAddress + 12] = 0; //CpuReady Int7 (Burst)
            Memory.Data[startAddress + 13] = 0; //DeviceReady Int6
            Memory.Data[startAddress + 14] = 0; //DeviceReady Int7 (Burst)

            //Reg0 (4x8B)
            Memory.Data[startAddress + 15] = 0;
            Memory.Data[startAddress + 16] = 0;
            Memory.Data[startAddress + 17] = 0;
            Memory.Data[startAddress + 18] = 0;

            //Reg1 (4x8B)
            Memory.Data[startAddress + 19] = 0;
            Memory.Data[startAddress + 20] = 0;
            Memory.Data[startAddress + 21] = 0;
            Memory.Data[startAddress + 22] = 0;

            //Reg2 (4x8B)
            Memory.Data[startAddress + 23] = 0;
            Memory.Data[startAddress + 24] = 0;
            Memory.Data[startAddress + 25] = 0;
            Memory.Data[startAddress + 26] = 0;

            //Reg3 (4x8B)
            Memory.Data[startAddress + 27] = 0;
            Memory.Data[startAddress + 28] = 0;
            Memory.Data[startAddress + 29] = 0;
            Memory.Data[startAddress + 30] = 0;

            //startAddress + 31 287 //input (burst)
            //starAddress + 288 543 //output (burst)

            App.assemblerDebug += "DEVICE_INITIALISED_" + id + " type:" + type + Environment.NewLine;
            StartDevice();
        }

        public void Write(int address, byte data) {
            Memory.Write(address + startAddress, data, true);
        }

        public byte Read(int address) {
            return Memory.Read(address + startAddress);
        }

        public void Interrupt(byte interrupt) {
            App.cpu.Interrupt((byte)(128 + (id * 8) + interrupt));
        }

        public virtual void Run() {
            while (true) {

                Thread.Sleep(10);
            }
        }


        public void StartDevice() {
            deviceThread = new Thread(new ThreadStart(Run));
            deviceThread.IsBackground = true;
            deviceThread.Start();
        }

    }

}