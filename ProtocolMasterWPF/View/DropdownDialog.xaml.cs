using MaterialDesignThemes.Wpf;
using System;
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
        public DropdownDialog(string[] options)
        {
            InitializeComponent();
            SelectList.ItemsSource = options;
        }
        public static string DropdownUserSelect(string[] options)
        {
            Task<object> dialogTask = Application.Current.Dispatcher.Invoke<Task<object>>(new Func<Task<object>>(() => DropdownDialog.DropdownHandler(options)));
            dialogTask.Wait();

            return dialogTask.Result as string;
        }
        private static Task<object> DropdownHandler(string[] options)
        {
            DropdownDialog dialog = new DropdownDialog(options);
            return DialogHost.Show(dialog, "PromptDialog");
        }
        public ListBox SelectList => SelectListBox;
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.Close("PromptDialog", SelectList.SelectedItem);
        }
    }
}
