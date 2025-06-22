using CpuSim4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CpuSim4 {
    /// <summary>
    /// Interaction logic for CacheViewer.xaml
    /// </summary>
    public partial class CacheViewer : Window {
        bool autoRefreshMemory = false;
        bool instCache = true;
        public CacheViewer() {
            InitializeComponent();
        }

        public void UpdateWindow() {
            int offset = 0;
            int.TryParse(OffsetText.Text, out offset);
            offset = Math.Clamp(offset, 0, 224);

            if (!App.cpu.cpuRunning || !App.cpu.threadRunning) {
                return;
            }
            StringBuilder[] cacheLines = new StringBuilder[256];

            for (int tag = 0; tag < 128; tag++) {
                for (int set = 0; set < 2; set++) {
                    int lineIndex = tag * 2 + set;
                    cacheLines[lineIndex] = new StringBuilder();
                    cacheLines[lineIndex].Append($"Index {tag:D3} Set {set}:");
                    CacheLine? maybeLine;
                    if (instCache) {
                        maybeLine = App.cpu.cacheI[tag]?.lines?[set];
                    } else {
                        maybeLine = App.cpu.cacheD[tag]?.lines?[set];
                    }
                    byte[] data = new byte[8];
                    if (maybeLine != null && maybeLine is CacheLine line && line.data != null) {
                        for (int b = 0; b < Math.Min(8, line.data.Length); b++) {
                            data[b] = line.data[b];
                        }
                    }

                    for (int b = 0; b < 8; b++) {
                        cacheLines[lineIndex].Append(" " + data[b].ToString("X2"));
                    }
                }
            }

            for (int i = 0; i < 32; i++) {
                int lineIndex = offset + i;

                if (lineIndex >= cacheLines.Length) {
                    break;
                }

                string text = cacheLines[lineIndex].ToString();

                switch (i) {
                    case 0: MemoryLine0.Text = text; break;
                    case 1: MemoryLine1.Text = text; break;
                    case 2: MemoryLine2.Text = text; break;
                    case 3: MemoryLine3.Text = text; break;
                    case 4: MemoryLine4.Text = text; break;
                    case 5: MemoryLine5.Text = text; break;
                    case 6: MemoryLine6.Text = text; break;
                    case 7: MemoryLine7.Text = text; break;
                    case 8: MemoryLine8.Text = text; break;
                    case 9: MemoryLine9.Text = text; break;
                    case 10: MemoryLine10.Text = text; break;
                    case 11: MemoryLine11.Text = text; break;
                    case 12: MemoryLine12.Text = text; break;
                    case 13: MemoryLine13.Text = text; break;
                    case 14: MemoryLine14.Text = text; break;
                    case 15: MemoryLine15.Text = text; break;
                    case 16: MemoryLine16.Text = text; break;
                    case 17: MemoryLine17.Text = text; break;
                    case 18: MemoryLine18.Text = text; break;
                    case 19: MemoryLine19.Text = text; break;
                    case 20: MemoryLine20.Text = text; break;
                    case 21: MemoryLine21.Text = text; break;
                    case 22: MemoryLine22.Text = text; break;
                    case 23: MemoryLine23.Text = text; break;
                    case 24: MemoryLine24.Text = text; break;
                    case 25: MemoryLine25.Text = text; break;
                    case 26: MemoryLine26.Text = text; break;
                    case 27: MemoryLine27.Text = text; break;
                    case 28: MemoryLine28.Text = text; break;
                    case 29: MemoryLine29.Text = text; break;
                    case 30: MemoryLine30.Text = text; break;
                    case 31: MemoryLine31.Text = text; break;
                }
            }

        }

        private void Btn_AutoRefresh(object sender, RoutedEventArgs e) {
            if (autoRefreshMemory) {
                BtnAutoRefresh.Content = "Auto Refresh: Off";
                autoRefreshMemory = false;
            } else {
                BtnAutoRefresh.Content = "Auto Refresh: On";
                autoRefreshMemory = true;
            }
        }

        private void Btn_Cache(object sender, RoutedEventArgs e) {
            if (!instCache) {
                BtnAutoRefresh.Content = "Instruction Cache";
                instCache = true;
            } else {
                BtnAutoRefresh.Content = "Data Cache";
                instCache = false;
            }
        }

        private void TextBox_MemoryViewer(object sender, TextChangedEventArgs e) {
            UpdateWindow();
        }

    }


}