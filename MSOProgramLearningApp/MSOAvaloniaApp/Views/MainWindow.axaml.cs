using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MSOAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void LoadHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void TurnHandler(object? sender, RoutedEventArgs e)
    {
        //Greeting.Text = " bababababa";
        Debug.WriteLine("bababab");
    }

    private void MoveHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void RepeatHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}