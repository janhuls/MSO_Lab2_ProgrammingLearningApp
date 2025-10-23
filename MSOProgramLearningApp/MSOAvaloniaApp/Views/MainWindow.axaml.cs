using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MSOAvaloniaApp.ViewModels;
using MSOProgramLearningApp;

namespace MSOAvaloniaApp.Views;

public partial class MainWindow : Window
{
    private Character _character;
    private MainWindowViewModel _vm;
    public MainWindow()
    {
        _character = new Character();
        _vm = new MainWindowViewModel(_character);
        DataContext = _vm;
        
        InitializeComponent();
    }

    private void LoadHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void TurnHandler(object? sender, RoutedEventArgs e)
    {
        _vm.Greeting = "nooit"; // wertk niet want hij doet niet updaten
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