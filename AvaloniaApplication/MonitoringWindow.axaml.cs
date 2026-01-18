using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AvaloniaApplication;

public partial class MonitoringWindow : Window
{
    private bool _adminOpened;
    public MonitoringWindow()
    {
        InitializeComponent();
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
    private void AdminButton_Click(object? sender, RoutedEventArgs e)
    {
        _adminOpened = !_adminOpened;
        AdminSubMenu.IsVisible = _adminOpened;
    }
    
    private void TradingMachines_Click(object? sender, RoutedEventArgs e)
    {
        var window = new TradingMachineWindow();
        window.Show();
        this.Close();
    }
    private void GeneralState_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        ResetButtons(button.Parent as Panel);
        HighlightButton(button);

        // пример логики
        var tag = button.Tag?.ToString();
        System.Diagnostics.Debug.WriteLine($"Общее состояние: {tag}");
    }
    
    private void Connection_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        ResetButtons(button.Parent as Panel);
        HighlightButton(button);

        var tag = button.Tag?.ToString();
        System.Diagnostics.Debug.WriteLine($"Подключение: {tag}");
    }
    
    private void ExtraStatus_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        HighlightButton(button);

        var tag = button.Tag?.ToString();
        System.Diagnostics.Debug.WriteLine($"Доп статус: {tag}");
    }
    
    private void ResetButtons(Panel? panel)
    {
        if (panel == null)
            return;

        foreach (var child in panel.Children)
        {
            if (child is Button btn)
            {
                btn.Background = Brushes.Transparent;
            }
        }
    }

    private void HighlightButton(Button button)
    {
        button.Background = new SolidColorBrush(Color.Parse("#0078D7"));
    }
    
    private void ApplyGeneralState_Click(object? sender, RoutedEventArgs e)
    {
        // TODO: применить фильтр общего состояния
    }

    private void ClearGeneralState_Click(object? sender, RoutedEventArgs e)
    {
        // TODO: сбросить фильтр общего состояния
    }
    
    private void Exit_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
