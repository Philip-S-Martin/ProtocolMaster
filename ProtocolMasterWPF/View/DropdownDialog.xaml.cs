using MaterialDesignThemes.Wpf;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for DropdownDialog.xaml
    /// </summary>
    public partial class DropdownDialog : UserControl, ISelectView
    {
        public DropdownDialog(string[] options, string prompt)
        {
            InitializeComponent();
            SelectList.ItemsSource = options;
            PromptText.Text = prompt;
        }
        public static string DropdownUserSelect(string[] options, string prompt)
        {
            Task<object> dialogTask = Application.Current.Dispatcher.Invoke<Task<object>>(new Func<Task<object>>(() => DropdownDialog.DropdownHandler(options, prompt)));
            dialogTask.Wait();
            return dialogTask.Result as string;
        }
        private static Task<object> DropdownHandler(string[] options, string prompt)
        {
            DropdownDialog dialog = new DropdownDialog(options, prompt);
            return DialogHost.Show(dialog, "PromptDialog");
        }
        public DropdownDialog(int min, int max, string prompt)
        {
            InitializeComponent();
            SelectList.ItemsSource = Enumerable.Range(min, max-min+1);
            PromptText.Text = prompt;
        }
        public static int DropdownUserNumber(int min, int max, string prompt)
        {
            Task<object> dialogTask = Application.Current.Dispatcher.Invoke<Task<object>>(new Func<Task<object>>(() => DropdownDialog.DropdownHandler(min, max, prompt)));
            dialogTask.Wait();
            return (int)dialogTask.Result;
        }
        private static Task<object> DropdownHandler(int min, int max, string prompt)
        {
            DropdownDialog dialog = new DropdownDialog(min, max, prompt);
            return DialogHost.Show(dialog, "PromptDialog");
        }
        public ListBox SelectList => SelectListBox;
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.Close("PromptDialog", SelectList.SelectedItem);
        }
    }
}
