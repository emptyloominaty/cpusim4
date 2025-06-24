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

        public List<StackPanel> deviceList = new List<StackPanel>();
        public TextBox textBoxKey;

        public int displayWait = 0;
        public long time1 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

        public MainWindow() {
            InitializeComponent();
            Application.Current.MainWindow = this;
            CompositionTarget.Rendering += Main;

            InitDeviceStackPanel();
        }

        public void Main(object sender, EventArgs e) {
            Cpu cpu = App.cpu;

            long ticks = TimeSpan.TicksPerMillisecond;
            if (ticks < 1) {
                ticks = 1;
            }
            long ms = ((DateTime.Now.Ticks / ticks) - time1);
            long fps = 0;
            if (ms > 0) {
                fps = 1000 / ms;
            }

            if (displayWait < 30) {
                displayWait++;
            } else {
                displayWait = 0;
                for (int i = 0; i < App.displays.Count; i++) {
                    if (App.displays[i].IsLoaded && App.displays[i].IsVisible) {
                        App.displays[i].UpdateWindow();
                    }
                }
            }

            time1 = DateTime.Now.Ticks / ticks;

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

            Register0.Text = "r0: " + cpu.registers[0].ToString("X8");
            Register1.Text = "r1: " + cpu.registers[1].ToString("X8");
            Register2.Text = "r2: " + cpu.registers[2].ToString("X8");
            Register3.Text = "r3: " + cpu.registers[3].ToString("X8");
            Register4.Text = "r4: " + cpu.registers[4].ToString("X8");
            Register5.Text = "r5: " + cpu.registers[5].ToString("X8");
            Register6.Text = "r6: " + cpu.registers[6].ToString("X8");
            Register7.Text = "r7: " + cpu.registers[7].ToString("X8");
            Register8.Text = "r8: " + cpu.registers[8].ToString("X8");
            Register9.Text = "r9: " + cpu.registers[9].ToString("X8");
            Register10.Text = "r10: " + cpu.registers[10].ToString("X8");
            Register11.Text = "r11: " + cpu.registers[11].ToString("X8");
            Register12.Text = "r12: " + cpu.registers[12].ToString("X8");
            Register13.Text = "r13: " + cpu.registers[13].ToString("X8");
            Register14.Text = "r14: " + cpu.registers[14].ToString("X8");
            Register15.Text = "r15: " + cpu.registers[15].ToString("X8");
            Register16.Text = "r16: " + cpu.registers[16].ToString("X8");
            Register17.Text = "r17: " + cpu.registers[17].ToString("X8");
            Register18.Text = "r18: " + cpu.registers[18].ToString("X8");
            Register19.Text = "r19: " + cpu.registers[19].ToString("X8");
            Register20.Text = "r20: " + cpu.registers[20].ToString("X8");
            Register21.Text = "r21: " + cpu.registers[21].ToString("X8");
            Register22.Text = "r22: " + cpu.registers[22].ToString("X8");
            Register23.Text = "r23: " + cpu.registers[23].ToString("X8");
            Register24.Text = "r24: " + cpu.registers[24].ToString("X8");
            Register25.Text = "r25: " + cpu.registers[25].ToString("X8");
            Register26.Text = "r26: " + cpu.registers[26].ToString("X8");
            Register27.Text = "r27: " + cpu.registers[27].ToString("X8");
            Register28.Text = "r28: " + cpu.registers[28].ToString("X8");
            Register29.Text = "r29: " + cpu.registers[29].ToString("X8");
            Register30.Text = "r30: " + cpu.registers[30].ToString("X8");
            Register31.Text = "r31: " + cpu.registers[31].ToString("X8");

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

        private void Btn_Display_Click(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;
            Tuple<int, int> data = (Tuple<int, int>)button.Tag;

            int deviceId = data.Item1;
            int displayId = data.Item2;

            if (App.displays[displayId].IsVisible) {
                App.displays[displayId].Hide();
            } else if (App.displays[displayId].IsLoaded) {
                App.displays[displayId].Show();
            } else {
                App.displays[displayId] = new Display((byte)deviceId);
                App.displays[displayId].Show();
            }
        }

        private void Btn_Storage_Click(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;
            Tuple<int, int> data = (Tuple<int, int>)button.Tag;

            int deviceId = data.Item1;
            int storageId = data.Item2;

            if (App.storages[storageId].IsVisible) {
                App.storages[storageId].Hide();
            } else if (App.storages[storageId].IsLoaded) {
                App.storages[storageId].Show();
            } else {
                App.storages[storageId] = new StorageWindow((byte)deviceId);
                App.storages[storageId].Show();
            }
        }

        private void InitDeviceStackPanel() {
            bool keyBoardDev = false;
            for (int i = 0; i < App.devices.Count; i++) {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                deviceList.Add(stackPanel);

                TextBlock textBox = new TextBlock();
                textBox.Text = "Device_" + i;
                textBox.Margin = new Thickness(0, 0, 10, 0);

                TextBlock textBox_Type = new TextBlock();
                textBox_Type.Text = Functions.GetDeviceType(App.devices[i].type);
                textBox_Type.Margin = new Thickness(0, 0, 10, 0);

                TextBlock textBox_Address = new TextBlock();
                textBox_Address.Text = "0x" + (8388608 + (524288 * i)).ToString("X6");
                textBox_Address.Margin = new Thickness(0, 0, 10, 0);

                stackPanel.Children.Add(textBox);
                stackPanel.Children.Add(textBox_Address);
                stackPanel.Children.Add(textBox_Type);

                int displayIdx = 0;
                int storageIdx = 0;
                if (App.devices[i].type == 8 && !keyBoardDev) {
                    keyBoardDev = true;
                    textBoxKey = new TextBox();
                    textBoxKey.Width = 80;
                    textBoxKey.Height = 30;
                    textBoxKey.Name = "textBoxKey";
                    textBoxKey.KeyDown += OnKeyDownHandler;
                    stackPanel.Children.Add(textBoxKey);
                } else if (App.devices[i].type == 4 || App.devices[i].type == 5) {
                    Button Btn_Display = new Button();
                    Btn_Display.Name = "Btn_Display";
                    Btn_Display.Content = "Display";
                    Btn_Display.Click += Btn_Display_Click;
                    Btn_Display.Tag = Tuple.Create(i, displayIdx);
                    displayIdx++;
                    stackPanel.Children.Add(Btn_Display);
                } else if (App.devices[i].type == 2) {
                    Button Btn_Storage = new Button();
                    Btn_Storage.Name = "Btn_Storage";
                    Btn_Storage.Content = "Storage";
                    Btn_Storage.Click += Btn_Storage_Click;
                    Btn_Storage.Tag = Tuple.Create(i, storageIdx);
                    storageIdx++;
                    stackPanel.Children.Add(Btn_Storage);
                }


                DeviceList.Children.Add(stackPanel);

            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e) {
            App.key = (byte)KeyInterop.VirtualKeyFromKey(e.Key);
            textBoxKey.Text = "";
        }


    }

}
