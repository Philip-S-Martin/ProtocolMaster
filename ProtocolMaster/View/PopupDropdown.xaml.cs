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

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for PopupDropdown.xaml
    /// </summary>
    public partial class PopupDropdown : Window
    {
        public PopupDropdown(string[] options)
        {
            InitializeComponent();
            this.Left = (App.Window.Width - this.Width) / 2;
            this.Top = (App.Window.Height - this.Height) / 2;
            foreach (string s in options)
                Dropdown.Items.Add(s);
            Dropdown.SelectionChanged += DropdownSelectionHandler;
        }
        
        private void DropdownSelectionHandler(Object sender, SelectionChangedEventArgs e)
        {
            this.Close();
        }

        public static string PopupNow(string[] options)
        {
            PopupDropdown popup = new PopupDropdown(options);
            App.Window.IsEnabled = false;
            popup.ShowDialog();
            App.Window.IsEnabled = true;
            return popup.DropdownResult();
        }

        public string DropdownResult()
        {
            return Dropdown.SelectedItem as string;
        }
    }
}
