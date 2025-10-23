using System;
using CommunityToolkit.Mvvm.Input;
using MSOProgramLearningApp;
using ICommand = System.Windows.Input.ICommand;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel(Character character) : ViewModelBase
{
    private readonly Character _character = character;
    
    public string Greeting { get; set; } = "bababa";
}