using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MSOAvaloniaApp.ViewModels;

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
        //_viewModel.Greeting = "nooit";
        //Debug.WriteLine("bababab");
    }

    private void MoveHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void RepeatHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void RunHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void MetricsHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}