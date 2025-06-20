using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim4 {
    public static class Memory {
        public static byte[] Data = new byte[16777216];
        public static bool[] DataCanWriteArray = new bool[16777216];

        static Memory() {
            Init();
        }

        public static void Init() {
            Data = new byte[16777216];
            DataCanWriteArray = new bool[16777216];
            for (int i = 0; i < Data.Length; i++) {
                Data[i] = 0;
                DataCanWriteArray[i] = true;
            }

            for (int i = 0x400000; i <= 0xffffff; i++) {
                DataCanWriteArray[i] = false;
            }

            //TEST CODE
            Write(7340032, 3, true);
            Write(7340033, 0, true);
            Write(7340034, 0x70, true);
            Write(7340035, 0, true);
            Write(7340036, 0, true);

            Write(7340037, 3, true);
            Write(7340038, 1, true);
            Write(7340039, 0x70, true);
            Write(7340040, 0, true);
            Write(7340041, 6, true);

            Write(7340042, 15, true);
            Write(7340043, 1, true);

            Write(7340044, 26, true);
            Write(7340045, 0, true);
            Write(7340046, 1, true);
            Write(7340047, 0x70, true);
            Write(7340048, 0x00, true);
            Write(7340049, 0x0A, true);

            Write(7340050, 24, true);
            Write(7340051, 0x70, true);
            Write(7340052, 0x00, true);
            Write(7340053, 0x17, true);

            Write(7340054, 0, true);

            Write(7340055, 15, true);
            Write(7340056, 0, true);

            Write(7340057, 16, true);
            Write(7340058, 0, true);

            Write(7340059, 25, true);




        }

        public static byte Read(int address) {
            if (address < 0 || address >= Data.Length) {
                return 0;
            }
            return Data[address];
        }
        public static void Write(int address, byte data, bool forceWrite = false) {
            if (address >= 0 && address < Data.Length) {
                if (DataCanWriteArray[address] || forceWrite) {
                    Data[address] = data;
                }
            }
        }

    }
}
