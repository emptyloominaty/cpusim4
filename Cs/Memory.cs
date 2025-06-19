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
            /*Write(7340032, 3, true);
            Write(7340033, 0, true);
            Write(7340034, 0x70, true);
            Write(7340035, 0, true);
            Write(7340036, 0, true);

            Write(7340037, 3, true);
            Write(7340038, 1, true);
            Write(7340039, 0x70, true);
            Write(7340040, 0, true);
            Write(7340041, 2, true);

            Write(7340042, 1, true);
            Write(7340043, 0, true);
            Write(7340044, 1, true);
            Write(7340045, 2, true);

            Write(7340046, 15, true);
            Write(7340047, 3, true);

            Write(7340048, 14, true);
            Write(7340049, 4, true);
            Write(7340050, 0, true);
            Write(7340051, 1, true);
            Write(7340052, 255, true);
            Write(7340053, 255, true);

            Write(7340054, 26, true);
            Write(7340055, 4, true);
            Write(7340056, 3, true);
            Write(7340057, 0x70, true);
            Write(7340058, 0, true);
            Write(7340059, 0, true);

            Write(7340060, 24, true);
            Write(7340061, 0x70, true);
            Write(7340062, 0x00, true);
            Write(7340063, 0x2F, true);

            Write(7340064, 0, true);

            Write(7340065, 13, true);
            Write(7340066, 0, true);
            Write(7340067, 25, true);
            Write(7340068, 255, true);
            Write(7340069, 255, true);

            Write(7340070, 15, true);
            Write(7340071, 1, true);

            Write(7340072, 26, true);
            Write(7340073, 0, true);
            Write(7340074, 1, true);
            Write(7340075, 0x70, true);
            Write(7340076, 0x00, true);
            Write(7340077, 0x26, true);

            Write(7340078, 25, true);

            Write(7340079, 23, true);
            Write(7340080, 0x70, true);
            Write(7340081, 0x00, true);
            Write(7340082, 0x21, true);*/
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
