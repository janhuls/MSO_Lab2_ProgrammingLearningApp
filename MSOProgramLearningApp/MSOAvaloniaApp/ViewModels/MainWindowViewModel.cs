using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MSOProgramLearningApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly OutputDrawer _outputDrawer;
    
    [ObservableProperty] // example code of FindExit1
    private string code = "Repeat 3 \n    RepeatUntil WallAhead \n        Move 1 \n    Turn right \nRepeat 2 \n    RepeatUntil GridEdge \n        Move 1 \n    Turn right\n";

    [ObservableProperty] 
    private string output = "Output";
    
    public IRelayCommand MetricsCommand { get; } 
    public IRelayCommand OutputCommand { get; }

    [ObservableProperty] private IImage? outputImageBinding;

    public MainWindowViewModel()
    {
        _outputDrawer = new OutputDrawer();
        OutputCommand = new RelayCommand(GenerateOutput);
        MetricsCommand = new RelayCommand(GenerateMetrics);
    }

    private List<ICommand>? getCommands(string s)
    {
        try
        {
            return new StringParser(s).Parse();
        }
        catch (Exception e)
        {
            Output = e.Message;
            return null;
        }
    }

    private void GenerateMetrics()
    {
        var cmds = getCommands(Code); 
        if (cmds is null)
            return;
        var metrics = new MetricsCalculator(new BasicMetricsStrategy());
        Output = metrics.Calculate(cmds);
    }

    private Character GetCharacter()
    {
        return new Character();
        //TODO
        //read grid size from UI ofz
    }
    private void GenerateOutput()
    {
        var cmds = getCommands(Code);
        if (cmds is null)
            return;
        
        var c = GetCharacter();
        try
        {
            doCommands(c, cmds);
            bindBitmap(c);
            updateOutput(c);
        }
        catch (Exception e)
        {
            Output =  e.Message;
        }
    }

    private void updateOutput(Character c)
    {
        Output = string.Join(", ", c.Moves) + ".";
        Output += $"\nEnd state {c}";
    }
    private void doCommands(Character c, List<ICommand> cmds)
    {
        foreach (var cmd in cmds) 
            cmd.Execute(c);
    }
    private void bindBitmap(Character character)
    {
        MemoryStream memoryStream = new MemoryStream();
        _outputDrawer.GenerateBitmap(character, memoryStream);
        OutputImageBinding = new Bitmap(memoryStream);
    }
}