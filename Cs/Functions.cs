using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuSim4 {
    public static class Functions {
        public static int ConvertTo32Bit(byte a, byte b, byte c, byte d) {
            return (int)((a << 24) | (b << 16) | (c << 8) | d);
        }

        public static byte[] ConvertFrom32Bit(int val) {
            int d = val & 0xff;
            int c = (val >> 8) & 0xff;
            int b = (val >> 16) & 0xff;
            int a = (val >> 24) & 0xff;
            return new byte[] { (byte)a, (byte)b, (byte)c, (byte)d };
        }

        public static int ConvertTo24Bit(byte a, byte b, byte c) {
            return (int)((a << 16) | (b << 8) | c);
        }

        public static byte[] ConvertFrom24Bit(int val) {
            int c = val & 0xff;
            int b = (val >> 8) & 0xff;
            int a = (val >> 16) & 0xff;
            return new byte[] { (byte)a, (byte)b, (byte)c };
        }

        public static ushort ConvertTo16Bit(byte a, byte b) {
            return (ushort)((a << 8) | b);
        }

        public static byte[] ConvertFrom16Bit(int val) {
            int b = (val & 0xff);
            int a = ((val >> 8) & 0xff);
            return new byte[] { (byte)a, (byte)b };
        }

        public static int HexToDec(string str) {
            return Convert.ToInt32(str, 16);
        }


    }
}
