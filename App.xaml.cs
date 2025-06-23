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
        public static string cpuDebug = "";
        public static string assemblerDebug = "";
        public static int key = 0;
        public static List<Device> devices = new List<Device>();
        public static List<StorageWindow> storages = new List<StorageWindow>();
        public static List<Display> displays = new List<Display>();

        protected override void OnStartup(StartupEventArgs e) {
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            base.OnStartup(e);

            opCodes = new OpCodes();

            cpu = new Cpu(opCodes.codes);
            cpu.StartCpu();

            devices.Add(new Devices.Keyboard(8, 0, 0xF, 64,0));
            devices.Add(new Devices.Timer(7, 1, 0xF, 64));
            devices.Add(new Devices.VramDisplay(4, 2, 0xF, 64, 524288, 512, 400, 0));
            devices.Add(new Devices.Storage(2, 3, 0x100, 512, 1048576,1));


        }
    }
}