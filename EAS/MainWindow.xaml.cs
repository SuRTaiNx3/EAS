using EAS.ViewModels;
using MahApps.Metro.Controls;
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

namespace EAS
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainViewModel MainVM;

        private bool _forceExit = false;

        public MainWindow()
        {
            InitializeComponent();
            MainVM = new MainViewModel();
            MainVM.OnExitApplication = () => { _forceExit = true; this.Close(); };
            this.DataContext = MainVM;

            Uri iconUri = new Uri("pack://application:,,,/Resources/icon.ico", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            this.StateChanged += MetroWindow_StateChanged;

            if (SettingsList.Instance.StartMinimized)
                this.WindowState = WindowState.Minimized;
        }

        private void TextBoxHotKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            KeyModifier modifier = KeyModifier.None;

            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                modifier = KeyModifier.Ctrl;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                modifier |= KeyModifier.Shift;
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
                modifier |= KeyModifier.Alt;
            if ((Keyboard.Modifiers & ModifierKeys.Windows) != 0)
                modifier |= KeyModifier.Win;

            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                key = Key.None;
            }

            //Console.WriteLine("Key: " + key.ToString() + "    Modifier: " + modifier.ToString());
            MainVM.HotKeyToEdit?.HotKeyRecorded(key, modifier);
        }

        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();
        }

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SettingsList.Instance.MinimizeOnExit && !_forceExit)
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized;
            }
        }
    }
}
