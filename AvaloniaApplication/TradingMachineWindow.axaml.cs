using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication.ViewModels;

namespace AvaloniaApplication;

public partial class TradingMachineWindow : Window
{

    private bool _adminOpened;

    public TradingMachineWindow()
    {
        InitializeComponent();
        DataContext = new TradingMachineViewModel();
    }

    private void AdminButton_Click(object? sender, RoutedEventArgs e)
    {
        _adminOpened = !_adminOpened;
        AdminSubMenu.IsVisible = _adminOpened;
    }
    

    private void Head_Click(object? sender, RoutedEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
        this.Close();
    }
     private void UserButton_Click(object sender, RoutedEventArgs e)
     {
         UserPopup.IsVisible = !UserPopup.IsVisible;
        
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