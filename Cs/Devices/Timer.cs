using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpuSim4.Devices {
    public class Timer : Device {
        byte timers;
        long[] timeB = new long[16];


        public Timer(byte type, byte id, int burstStartAddress, int burstSize, byte timers = 4, byte burstEnabled = 0)
            : base(type, id, burstStartAddress, burstSize, burstEnabled) {
            this.timers = timers;

            for (int i = 0x100; i <= 0x1000; i++) {
                Memory.DataCanWriteArray[(8388608 + (524288 * id)) + i] = true;
            }

            for (int i = 0; i < timers; i++) {
                timeB[i] = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                Write((int)(0x100 + (i * 5)), 00); //timer enabled

                Write((int)(0x100 + (i * 5) + 1), 00); //timerSetHi
                Write((int)(0x100 + (i * 5) + 2), 00); //timerSetLo

                Write((int)(0x100 + (i * 5) + 3), 00); //timerHi
                Write((int)(0x100 + (i * 5) + 4), 00); //timerLo
            }


        }

        public override void Run() {
            while (true) {
                long time = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                long timeA = time;
                for (int i = 0; i < timers; i++) {
                    if (Read((int)(0x100 + (i * 5))) == 1) {
                        long timerSet = Functions.ConvertTo16Bit(Read((int)(0x100 + (i * 5) + 1)), Read((int)(0x100 + (i * 5) + 2)));
                        int d = (int)(timeA - timeB[i]);
                        byte[] timer = new byte[2];
                        Functions.ConvertFrom16Bit(d,timer);
                        Write((int)(0x100 + (i * 5) + 3), timer[0]);
                        Write((int)(0x100 + (i * 5) + 4), timer[1]);

                        if (timeA - timeB[i] > timerSet) {
                            Interrupt((byte)(i));
                            timeB[i] = time;
                        }


                    }
                }

                Thread.Sleep(1);
            }
        }

    }
}
