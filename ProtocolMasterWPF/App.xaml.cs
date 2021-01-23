using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProtocolMasterWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public LogViewModel LogVM { get; private set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeLog();
        }
        private void InitializeLog()
        {
            LogVM = new LogViewModel();
            Log.ErrorPrinter = LogVM.LogText.Add;
            Log.OutputPrinter = LogVM.LogText.Add;
            Log.Out("Output Log Running");
            Log.Error("Error Log Running");
            Task logThreadTask = new Task(() => Log.Error("Log working in parallel thread"));
            logThreadTask.Start();
        }
    }
}
