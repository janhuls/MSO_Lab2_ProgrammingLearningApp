using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using MSOProgramLearningApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSOAvaloniaApp.Views;
using Grid = MSOProgramLearningApp.Grid;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private MainWindow _mainWindow;
    private readonly OutputDrawer _outputDrawer;
    
    [ObservableProperty] // example code of FindExit1
    private string code = "Repeat 3 \n    RepeatUntil WallAhead \n        Move 1 \n    Turn right \nRepeat 2 \n    RepeatUntil GridEdge \n        Move 1 \n    Turn right\n";

    [ObservableProperty] 
    private string output = "Output";
    
    [ObservableProperty] private IImage? outputImageBinding;
    public IRelayCommand MetricsCommand { get; } 
    public IRelayCommand OutputCommand { get; }
    public IRelayCommand LoadProgramCommand { get; }
    public IRelayCommand LoadGridCommand { get; }

    private string _customGrid = "";
    private int _selectedGrid = 1;
    public int SelectedGrid
    {
        get => _selectedGrid;
        set
        {
            if (SetProperty(ref _selectedGrid, value))
            {
                bindBitmap(GetCharacter()); // update the bitmap
            }
        }
    }

    public MainWindowViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        _outputDrawer = new OutputDrawer();
        OutputCommand = new RelayCommand(GenerateOutput);
        MetricsCommand = new RelayCommand(GenerateMetrics);
        LoadProgramCommand = new RelayCommand(LoadProgram);
        LoadGridCommand = new RelayCommand(LoadGrid);
        
        // load default bitmap
        bindBitmap(GetCharacter());
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

    private Character GetCharacter() => new(GetGrid());

    private Grid GetGrid()
    {
        switch(_selectedGrid)
        {
            case 0:
                return Grid.XSquareFalse(5);
            case 1:
                return Grid.XSquareFalse(10);
            case 2:
                return Grid.XSquareFalse(15);
            case 3:
                return Grid.XSquareFalse(20);
            case 4:
                if (string.IsNullOrEmpty(_customGrid))
                {
                    Output = "ERROR: No custom grid loaded, Press the load grid button to load a custom grid.\nLoaded a 10x10 grid as default";
                    return Grid.XSquareFalse(10);
                }
                else
                    return GridParser.Parse(_customGrid);
            default:
                return Grid.XSquareFalse(10);
        }
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

    private async void LoadProgram()
    {
        try
        {
            string? contents = await _mainWindow.getFileContents();
            if (contents is not null)
                Code = contents;
        }
        catch (Exception e)
        {
            Output = $"Failed to load program file\nError: {e.Message}";
        }
        
    }

    private async void LoadGrid()
    {
        try
        {
            string? contents = await _mainWindow.getFileContents();
            if (contents is not null)
            {
                SelectedGrid = 4; // set to custom
                _customGrid = contents;
            }
            else
            {
                Output = "ERROR: file is empty or null";
            }
        }
        catch (Exception e)
        {
            Output = $"Failed to load program file\nError: {e.Message}";
        }
    }
}