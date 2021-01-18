using MahApps.Metro.Controls;
using ProtocolMaster.Model.Protocol.Driver;
using ProtocolMaster.Model.Protocol.Interpreter;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            App.Instance.ExtensionSystem.InterpreterManager.OnOptionsLoaded += LoadInterpreters;
            App.Instance.ExtensionSystem.DriverManager.OnOptionsLoaded += LoadDrivers;
        }

        private void Click_Documentation(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://sites.google.com/view/protocolmaster/docs");
        }

        private void Click_Extension_Folder(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "C:\\Users\\phili\\source\\repos\\Philip-S-Martin\\ProtocolMaster\\ProtocolMaster\\bin\\Debug\\net48\\Extensions",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void Click_Protocol_Folder(object sender, EventArgs e)
        {
            string target = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ProtocolMaster\\Protocols";
            Directory.CreateDirectory(target);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = target,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void Click_Log_Folder(object sender, EventArgs e)
        {
            ProtocolMaster.Model.Debug.Log.Instance.OpenFolder();
        }

        private void OnLoad(object sender, EventArgs e)
        {

        }

        // Window Closing Operations
        private void Window_Closed(object sender, EventArgs e)
        {
            App.Instance.Window_Closed();
        }

        public void LoadDrivers(object sender, EventArgs e)
        {
            DriverDropdown.Items.Clear();
            foreach (DriverMeta meta in App.Instance.ExtensionSystem.DriverManager.Options)
            {
                ListDriver(meta);
            }
        }
        public void ListDriver(DriverMeta data)
        {
            MenuItem newDriver = new MenuItem
            {
                Header = data.ToString()
            };
            newDriver.Resources.Add("data", data);
            newDriver.Click += new RoutedEventHandler(DriverClickHandler);
            DriverDropdown.Items.Add(newDriver);
        }

        public void DriverClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            DriverMeta data = src.Resources["data"] as DriverMeta;

            App.Instance.ExtensionSystem.DriverManager.Selected = data;
            ShowSelectedDriver();
        }

        public void ShowSelectedDriver()
        {
            SelectedDriver.Header = "Driver: " + App.Instance.ExtensionSystem.DriverManager.Selected.ToString();
        }

        public void LoadInterpreters(object sender, EventArgs e)
        {
            InterpreterDropdown.Items.Clear();
            foreach (InterpreterMeta meta in App.Instance.ExtensionSystem.InterpreterManager.Options)
            {
                ListInterpreter(meta);
            }
        }

        public void ListInterpreter(InterpreterMeta data)
        {
            MenuItem newInterpreter = new MenuItem
            {
                Header = data.ToString()
            };
            newInterpreter.Resources.Add("data", data);
            newInterpreter.Click += new RoutedEventHandler(InterpreterClickHandler);
            InterpreterDropdown.Items.Add(newInterpreter);
        }

        public void InterpreterClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem src = e.Source as MenuItem;
            InterpreterMeta data = src.Resources["data"] as InterpreterMeta;
            App.Instance.ExtensionSystem.InterpreterManager.Selected = data;
            ShowSelectedInterpreter();
        }

        public void ShowSelectedInterpreter()
        {
            SelectedInterpreter.Header = "Interpreter: " + App.Instance.ExtensionSystem.InterpreterManager.Selected.ToString();
        }
    }
}
