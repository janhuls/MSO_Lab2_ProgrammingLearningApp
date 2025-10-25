using System.Collections.Generic;
using MSOProgramLearningApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Character _character;   
    
    [ObservableProperty]
    private string greeting = "bababa";
    
    [ObservableProperty]
    private string code = "type ur code here broski";

    [ObservableProperty] 
    private string output = "Output";
    
    public IRelayCommand MetricsCommand { get; } 
    public IRelayCommand OutputCommand { get; }
    
    public MainWindowViewModel(Character character)
    {
        _character = character;
        OutputCommand = new RelayCommand(GenerateOutput);
        MetricsCommand = new RelayCommand(GenerateMetrics);
    }

    private List<ICommand> getCommands(string s)
    {
        return new StringParser(s).Parse();
    }

    private void GenerateMetrics()
    {
        var cmds = getCommands(Code);

        var metrics = new MetricsCalculator(new BasicMetricsStrategy());
        Output = metrics.Calculate(cmds);
    }
    private void GenerateOutput()
    {
        var cmds = getCommands(Code);
        
        var c = _character;
        foreach (var cmd in cmds)
            cmd.Execute(c);

        Output = string.Join(", ", c.Moves) + ".";
        Output += $"\nEnd state {c}";
    }
    public MainWindowViewModel() : this(new Character(Grid.TenSquareFalse()))
    {
    }
}