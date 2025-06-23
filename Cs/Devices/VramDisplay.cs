using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpuSim4.Devices {
    public class VramDisplay : Device {

        public VramDisplay(byte type, byte id, int burstStartAddress, int burstSize, int vramSize, int screenWidth, int screenHeight, byte burstEnabled)
            : base(type, id, burstStartAddress, burstSize, burstEnabled) {

            byte[] screenWidthB = new byte[2];
            Functions.ConvertFrom16Bit(screenWidth, screenWidthB);
            Write(0x0100, screenWidthB[0]);
            Write(0x0101, screenWidthB[1]);
            byte[] screenHeightB = new byte[2];
            Functions.ConvertFrom16Bit(screenHeight, screenHeightB);
            Write(0x0102, screenHeightB[0]);
            Write(0x0103, screenHeightB[1]);
            //Frame Buffer
            Write(0x0104, 0x00);
            Write(0x0105, 0x00);
            Write(0x0106, 0x10);
            Write(0x0107, 0x00);
            //Color Mode  1=8bit 2=16bit 3=24bit
            Write(0x0108, 0x01);



            for (int i = 0x100; i < vramSize + 0x0100; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id)) + i] = true;
            }


            Write(0x1000, 0xFF);
            Write(0x1001, 0xFF);
            Write(0x1002, 0xFF);
            Write(0x1003, 0xFF);

            App.displays.Add(new Display(id));
        }

    }
}
