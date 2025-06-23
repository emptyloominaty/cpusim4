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

        public static void ConvertFrom32Bit(int val, byte[] output) {
            output[3] = (byte)(val & 0xff);
            output[2] = (byte)((val >> 8) & 0xff);
            output[1] = (byte)((val >> 16) & 0xff);
            output[0] = (byte)((val >> 24) & 0xff);
        }

        public static int ConvertTo24Bit(byte a, byte b, byte c) {
            return (int)((a << 16) | (b << 8) | c);
        }

        public static void ConvertFrom24Bit(int val, byte[] output) {
            output[2] = (byte)(val & 0xff);
            output[1] = (byte)((val >> 8) & 0xff);
            output[0] = (byte)((val >> 16) & 0xff);
        }

        public static ushort ConvertTo16Bit(byte a, byte b) {
            return (ushort)((a << 8) | b);
        }

        public static void ConvertFrom16Bit(int val, byte[] output) {
            output[1] = (byte)(val & 0xff);
            output[0] = (byte)((val >> 8) & 0xff);
        }

        public static int HexToDec(string str) {
            return Convert.ToInt32(str, 16);
        }

        public static bool IsNumeric(string str) {
            double result;
            return double.TryParse(str, out result);
        }
        public static string FormatClock(long clock) {
            if (clock > 1000000) {
                return ((double)Math.Round((clock / 1000000.0) * 10) / 10) + "MHz";
            } else if (clock > 1000) {
                return ((double)Math.Round((clock / 1000.0) * 10) / 10) + "kHz";
            } else {
                return clock + "Hz";
            }
        }

        public static void ConvertFloatToBytes(float value, byte[] output) {
            int intBits = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            ConvertFrom32Bit(intBits, output);
        }
        public static float ConvertBytesToFloat(byte a, byte b, byte c, byte d) {
            int intBits = ConvertTo32Bit(a, b, c, d);
            return BitConverter.Int32BitsToSingle(intBits);
        }

        public static string GetDeviceType(byte type) {
            switch (type) {
                case 0:
                    return "";
                case 1:
                    return "Co-processor";
                case 2:
                    return "Storage";
                case 3:
                    return "Network";
                case 4:
                    return "VRAM+Display";
                case 5:
                    return "GPU";
                case 6:
                    return "User Storage Port";
                case 7:
                    return "Timer";
                case 8:
                    return "Keyboard";
                default:
                    return "";
            }
        }


    }
}
