using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CpuSim4 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public DebugWindow debugWindow = new DebugWindow();
        public MemoryViewer memoryViewerWindow = new MemoryViewer();
        public CacheViewer cacheViewerWindow = new CacheViewer();
        public MainWindow() {
            InitializeComponent();
            Application.Current.MainWindow = this;
            CompositionTarget.Rendering += Main;
        }

        public void Main(object sender, EventArgs e) {
            Cpu cpu = App.cpu;

            if (memoryViewerWindow.IsLoaded) {
                memoryViewerWindow.UpdateWindow();
            }

            if (cacheViewerWindow.IsLoaded) {
                cacheViewerWindow.UpdateWindow();
            }

            if (debugWindow.IsLoaded) {
                debugWindow.UpdateWindow();
            }

            SP_Text.Text = "SP: " + cpu.registers[34].ToString("X6") + " (" + Math.Round(((cpu.registers[34] - 8192.0) / 8192) * 100, 1) + "%)";
            PC_Text.Text = "PC: " + cpu.registers[33].ToString("X6");

            Running.Text = "Running: " + cpu.cpuRunning;
            Clock_Text.Text = Functions.FormatClock(cpu.clock);
            IPC_Text.Text = "IPC: " + Math.Round(cpu.instructionsDone / (cpu.cyclesDone + 1.0), 2);
            Instructions_Done_Text.Text = "Instructions Done: " + cpu.instructionsDone;
            Cycles_Done_Text.Text = "Cycles Done: " + cpu.cyclesDone;


            PS_Stalled_Text.Text = "Fetch|Execute Stalled: " + (cpu.fetchingStalled ? "Y" : "N") + " | " + (cpu.pipelineStalled ? "Y" : "N") + " ";
            PS_Text.Text = "1. " + App.opCodes.codes[cpu.pipeline[0].op].name+" ";
            PS_Text2.Text = "2. " + App.opCodes.codes[cpu.pipeline[1].op].name + " ";
            PS_Text3.Text = "3. " + App.opCodes.codes[cpu.pipeline[2].op].name + " ";
        }

        private void ToggleCpu(object sender, RoutedEventArgs e) {
            if (App.cpu.cpuRunning) {
                App.cpu.cpuRunning = false;
                BtnCpuToggle.Content = "Start";
            } else {
                App.cpu.Reset();
                App.cpu.cpuRunning = true;
                BtnCpuToggle.Content = "Stop";
            }
        }
        private void Btn_LoadASM(object sender, RoutedEventArgs e) {
            Assembler.Assemble(CodeEditor.Text, App.opCodes);
        }

        private void Btn_LoadMC(object sender, RoutedEventArgs e) {
            Assembler.LoadMachineCode(CodeEditor.Text);
        }

        private void Btn_Interrupt(object sender, RoutedEventArgs e) {
            int result = 0;
            if (int.TryParse(TextBox_Interrupt.Text, out result)) {
                App.cpu.Interrupt(byte.Parse(TextBox_Interrupt.Text));
            }
        }

        private void Btn_Debug_Click(object sender, RoutedEventArgs e) {
            ToggleWindow(ref debugWindow);
        }

        private void Btn_MemoryViewer_Click(object sender, RoutedEventArgs e) {
            ToggleWindow(ref memoryViewerWindow);
        }
        private void Btn_CacheViewer_Click(object sender, RoutedEventArgs e) {
            ToggleWindow(ref cacheViewerWindow);
        }


        private void ToggleWindow<T>(ref T window) where T : Window, new() {
            if (window != null && window.IsVisible) {
                window.Hide();
            } else if (window != null && window.IsLoaded) {
                window.Show();
            } else {
                window = new T();
                window.Show();
            }
        }

        private void Clock_TextChanged(object sender, TextChangedEventArgs e) {
            if (sender is TextBox textBox && App.cpu != null) {
                if (long.TryParse(textBox.Text, out long number)) {
                    App.cpu.clockSet = number;
                    if (number > 10000000) {
                        App.cpu.maxClock = true;
                    } else {
                        App.cpu.maxClock = false;
                    }
                }
            }
        }

    }

}
