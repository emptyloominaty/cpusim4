using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CpuSim4 {
    /// <summary>
    /// Interaction logic for Display.xaml
    /// </summary>
    public partial class Display : Window {
        public byte id = 0;
        public byte pxSize = 2;
        int width;
        int height;
        int frameBufferStart;
        int deviceAddress;
        byte colorMode;
        WriteableBitmap bitmap;

        public Display(byte id) {
            InitializeComponent();
            this.id = id;

            deviceAddress = (int)(8388608 + (524288 * id));
            width = Functions.ConvertTo16Bit(Memory.Read(deviceAddress + 0x0100), Memory.Read(deviceAddress + 0x0101));
            height = Functions.ConvertTo16Bit(Memory.Read(deviceAddress + 0x0102), Memory.Read(deviceAddress + 0x0103));
            colorMode = Memory.Read(deviceAddress + 0x0108);
            frameBufferStart = deviceAddress + Functions.ConvertTo24Bit(Memory.Read(deviceAddress + 0x0105), Memory.Read(deviceAddress + 0x0106), Memory.Read(deviceAddress + 0x0107));


            bitmap = new WriteableBitmap(1200, 960, 96, 96, PixelFormats.Bgra32, null);
            DisplayCanvas.Source = bitmap;
        }


        public void UpdateWindow() {
            deviceAddress = (int)(8388608 + (524288 * id));
            width = Functions.ConvertTo16Bit(Memory.Read(deviceAddress + 0x0100), Memory.Read(deviceAddress + 0x0101));
            height = Functions.ConvertTo16Bit(Memory.Read(deviceAddress + 0x0102), Memory.Read(deviceAddress + 0x0103));
            colorMode = Memory.Read(deviceAddress + 0x0108);
            frameBufferStart = deviceAddress + Functions.ConvertTo24Bit(Memory.Read(deviceAddress + 0x0105), Memory.Read(deviceAddress + 0x0106), Memory.Read(deviceAddress + 0x0107));

            if (width > 2048) {
                width = 2048;
            }
            if (height > 2048) {
                height = 2048;
            }

            bitmap.Lock();
            if (colorMode == 1) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        byte colorVal = Memory.Read(frameBufferStart);
                        int red = ((colorVal & 0xE0) >> 5);
                        int green = ((colorVal & 0x1C) >> 2);
                        int blue = (colorVal & 0x03);
                        byte rr = (byte)(red * 36);
                        byte gg = (byte)(green * 36);
                        byte bb = (byte)(blue * 85);
                        Color color = Color.FromArgb(255, rr, gg, bb);
                        DrawPixel(x * pxSize, y * pxSize, color);
                        frameBufferStart++;
                    }
                }
            } else if (colorMode == 2) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        int colorVal = Functions.ConvertTo16Bit(Memory.Read(frameBufferStart), Memory.Read(frameBufferStart + 1));

                        int red = ((colorVal >> 11) & 0x1F);
                        int green = ((colorVal >> 5) & 0x3F);
                        int blue = (colorVal & 0x1F);

                        byte rr = (byte)((red << 3) | (red >> 2));
                        byte gg = (byte)((green << 2) | (green >> 4));
                        byte bb = (byte)((blue << 3) | (blue >> 2));


                        Color color = Color.FromArgb(255, rr, gg, bb);
                        DrawPixel(x * pxSize, y * pxSize, color);
                        frameBufferStart += 2;
                    }
                }
            } else if (colorMode == 3) {
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        byte rr = (byte)Memory.Read(frameBufferStart);
                        byte gg = (byte)Memory.Read(frameBufferStart + 1);
                        byte bb = (byte)Memory.Read(frameBufferStart + 2);
                        Color color = Color.FromArgb(255, rr, gg, bb);
                        DrawPixel(x * pxSize, y * pxSize, color);
                        frameBufferStart += 3;
                    }
                }
            }




            bitmap.AddDirtyRect(new Int32Rect(0, 0, 1200, 960));
            bitmap.Unlock();
            //DisplayCanvas.InvalidateVisual();

        }

        private void DrawPixel(int x, int y, Color color) {
            if (bitmap == null || x < 0 || y < 0 || x + 1 >= bitmap.PixelWidth || y + 1 >= bitmap.PixelHeight) {
                return;
            }

            int stride = bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            int pixelIndex = y * stride + x * 4;

            unsafe {
                byte* pBackBuffer = (byte*)bitmap.BackBuffer;

                for (int row = 0; row < 2; row++) {
                    int rowOffset = row * stride;

                    for (int col = 0; col < 2; col++) {
                        int currentPixelIndex = pixelIndex + rowOffset + col * 4;

                        pBackBuffer[currentPixelIndex] = color.B;
                        pBackBuffer[currentPixelIndex + 1] = color.G;
                        pBackBuffer[currentPixelIndex + 2] = color.R;
                        pBackBuffer[currentPixelIndex + 3] = color.A;
                    }
                }
            }



        }


    }
}