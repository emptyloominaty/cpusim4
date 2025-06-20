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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CpuSim4 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public DebugWindow debugWindow = new DebugWindow();
        public MainWindow() {
            InitializeComponent();
            Application.Current.MainWindow = this;
            debugWindow.Show();

            CompositionTarget.Rendering += Main;
        }

        public void Main(object sender, EventArgs e) {


            if (debugWindow.IsLoaded) {
                debugWindow.UpdateWindow();
            }

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

        private void Btn_Debug_Click(object sender, RoutedEventArgs e) {
            if (debugWindow.IsVisible) {
                debugWindow.Hide();
            } else if (debugWindow.IsLoaded) {
                debugWindow.Show();
            } else {
                debugWindow = new DebugWindow();
                debugWindow.Show();
            }
        }


    }

}
