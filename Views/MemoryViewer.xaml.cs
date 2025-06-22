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

namespace CpuSim3 {
    /// <summary>
    /// Interaction logic for MemoryViewer.xaml
    /// </summary>
    public partial class MemoryViewer : Window {
        bool autoRefreshMemory = false;
        public MemoryViewer() {
            InitializeComponent();



        }

        public void UpdateWindow() {
            if (!AddressText.Text.StartsWith("0x") || AddressText.Text.Length <= 2) { //TODO:
                return;
            }
            int address = 0;
            int.TryParse(AddressText.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out address);

            string[] add = new string[32];
            for (int i = 0; i < add.Length; i++) {
                add[i] = (address + (16 * i)).ToString("X6");
            }

            string[] line = new string[32];
            for (int i = 0; i < line.Length; i++) {
                for (int j = i * 16; j < (i + 1) * 16; j++) {
                    line[i] += " " + Memory.Read((int)(address + j)).ToString("X2");
                }
            }

            MemoryLine0.Text = "0x" + add[0] + ": " + line[0];
            MemoryLine1.Text = "0x" + add[1] + ": " + line[1];
            MemoryLine2.Text = "0x" + add[2] + ": " + line[2];
            MemoryLine3.Text = "0x" + add[3] + ": " + line[3];
            MemoryLine4.Text = "0x" + add[4] + ": " + line[4];
            MemoryLine5.Text = "0x" + add[5] + ": " + line[5];
            MemoryLine6.Text = "0x" + add[6] + ": " + line[6];
            MemoryLine7.Text = "0x" + add[7] + ": " + line[7];
            MemoryLine8.Text = "0x" + add[8] + ": " + line[8];
            MemoryLine9.Text = "0x" + add[9] + ": " + line[9];
            MemoryLine10.Text = "0x" + add[10] + ": " + line[10];
            MemoryLine11.Text = "0x" + add[11] + ": " + line[11];
            MemoryLine12.Text = "0x" + add[12] + ": " + line[12];
            MemoryLine13.Text = "0x" + add[13] + ": " + line[13];
            MemoryLine14.Text = "0x" + add[14] + ": " + line[14];
            MemoryLine15.Text = "0x" + add[15] + ": " + line[15];
            MemoryLine16.Text = "0x" + add[16] + ": " + line[16];
            MemoryLine17.Text = "0x" + add[17] + ": " + line[17];
            MemoryLine18.Text = "0x" + add[18] + ": " + line[18];
            MemoryLine19.Text = "0x" + add[19] + ": " + line[19];
            MemoryLine20.Text = "0x" + add[20] + ": " + line[20];
            MemoryLine21.Text = "0x" + add[21] + ": " + line[21];
            MemoryLine22.Text = "0x" + add[22] + ": " + line[22];
            MemoryLine23.Text = "0x" + add[23] + ": " + line[23];
            MemoryLine24.Text = "0x" + add[24] + ": " + line[24];
            MemoryLine25.Text = "0x" + add[25] + ": " + line[25];
            MemoryLine26.Text = "0x" + add[26] + ": " + line[26];
            MemoryLine27.Text = "0x" + add[27] + ": " + line[27];
            MemoryLine28.Text = "0x" + add[28] + ": " + line[28];
            MemoryLine29.Text = "0x" + add[29] + ": " + line[29];
            MemoryLine30.Text = "0x" + add[30] + ": " + line[30];
            MemoryLine31.Text = "0x" + add[31] + ": " + line[31];
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

        private void TextBox_MemoryViewer(object sender, TextChangedEventArgs e) {
            UpdateWindow();
        }

    }


}