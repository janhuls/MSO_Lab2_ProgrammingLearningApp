using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using MSOProgramLearningApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Character _character;
    private OutputDrawer _outputDrawer;
    
    [ObservableProperty]
    private string greeting = "bababa";
    
    [ObservableProperty] // example code of FindExit1
    private string code = "Repeat 3 \n    RepeatUntil WallAhead \n        Move 1 \n    Turn right \nRepeat 2 \n    RepeatUntil GridEdge \n        Move 1 \n    Turn right\n";

    [ObservableProperty] 
    private string output = "Output";
    
    public IRelayCommand MetricsCommand { get; } 
    public IRelayCommand OutputCommand { get; }

    [ObservableProperty] private IImage? outputImageBinding;

    public MainWindowViewModel(Character character)
    {
        _character = character;
        _outputDrawer = new OutputDrawer();
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
        MemoryStream memoryStream = new MemoryStream();
        _outputDrawer.GenerateBitmap(cmds, _character,memoryStream);
        OutputImageBinding = new Bitmap(memoryStream);
        
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