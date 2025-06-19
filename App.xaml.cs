using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CpuSim4 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static Cpu cpu;
        public static OpCodes opCodes;
        public static string cpuDebug;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            opCodes = new OpCodes();

            cpu = new Cpu(opCodes.codes);
            cpu.StartCpu();


        }
    }
}