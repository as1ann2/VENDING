using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApplication
{
    public partial class MainWindow : Window
    {
        private bool _menuOpened = true;
        private bool _adminOpened;
        private bool _userOpened;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BurgerButton_Click(object? sender, RoutedEventArgs e)
        {
            var col = MainContentGrid.ColumnDefinitions[0];
            col.Width = _menuOpened ? new GridLength(0) : new GridLength(260);
            _menuOpened = !_menuOpened;
        }

        private void AdminButton_Click(object? sender, RoutedEventArgs e)
        {
            _adminOpened = !_adminOpened;
            AdminSubMenu.IsVisible = _adminOpened;
        }

        private void UserButton_Click(object? sender, RoutedEventArgs e)
        {
            _userOpened = !_userOpened;
            UserPopup.IsVisible = _userOpened;
            UserArrow.Text = _userOpened ? "⌄" : "›";
        }
        
        private void TradingMachines_Click(object? sender, RoutedEventArgs e)
        {
            var window = new TradingMachineWindow();
            window.Show();
            this.Close();
        }
        
        private void Monitoring_Click(object? sender, RoutedEventArgs e)
        {
            var window = new MonitoringWindow();
            window.Show();
            this.Close();
        }
        
        private void Exit_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
     

    }
}