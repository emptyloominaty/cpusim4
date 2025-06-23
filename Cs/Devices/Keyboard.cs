using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpuSim4.Devices {
    public class Keyboard : Device {

        public Keyboard(byte type, byte id, int burstStartAddress, int burstSize, byte burstEnabled)
            : base(type, id, burstStartAddress, burstSize, burstEnabled) {
        }

        public override void Run() {
            while (true) {
                if (App.key > 0 && Read(0x17) == 0) {
                    Write(0x18, (byte)App.key);
                    App.key = 0;
                    Interrupt(0);
                }

                Thread.Sleep(10);
            }
        }

    }
}
