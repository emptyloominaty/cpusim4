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
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window {
        public bool scrollToEnd = true;
        public DebugWindow() {
            InitializeComponent();
        }

        public void UpdateWindow() {
            CpuDebug.Text = App.cpuDebug;
            AssemblerDebug.Text = App.assemblerDebug;
            if (scrollToEnd) {
                CpuDebug.ScrollToEnd();
            }
        }

        private void Btn_CpuDebug_Click(object sender, RoutedEventArgs e) {
            if (App.cpu.debugCpu) {
                App.cpu.debugCpu = false;
                Btn_CpuDebug.Content = "CPU Debug: off";
            } else {
                App.cpu.debugCpu = true;
                Btn_CpuDebug.Content = "CPU Debug: on";
            }
        }

        private void Btn_ScrollToEnd_Click(object sender, RoutedEventArgs e) {
            if (scrollToEnd) {
                scrollToEnd = false;
                Btn_ScrollToEnd.Content = "Scroll to End: off";
            } else {
                scrollToEnd = true;
                Btn_ScrollToEnd.Content = "Scroll to End: on";
            }

        }
    }
}
