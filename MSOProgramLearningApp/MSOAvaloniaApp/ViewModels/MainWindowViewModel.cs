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
    private Character Character => new(GetGrid());
    
    [ObservableProperty] // example code of FindExit1
    private string _code = "Repeat 3 \n    RepeatUntil WallAhead \n        Move 1 \n    Turn right \nRepeat 2 \n    RepeatUntil GridEdge \n        Move 1 \n    Turn right\n";

    [ObservableProperty] 
    private string _output = "Output";
    
    [ObservableProperty] private IImage? _outputImageBinding;
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
                BindBitmap(Character); // update the bitmap
            }
        }
    }

    public MainWindowViewModel(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        //_outputDrawer = new OutputDrawer(_mainWindow.GetPictureWidth());
        OutputCommand = new RelayCommand(GenerateOutput);
        MetricsCommand = new RelayCommand(GenerateMetrics);
        LoadProgramCommand = new RelayCommand(LoadProgram);
        LoadGridCommand = new RelayCommand(LoadGrid);
        
        // load default bitmap
        BindBitmap(Character);
    }
    
    public MainWindowViewModel() //ONLY USED FOR THE PREVIEWER IN OUR RIDER IDE
    {
        _mainWindow = null;
        OutputCommand = new RelayCommand(GenerateOutput);
        MetricsCommand = new RelayCommand(GenerateMetrics);
        LoadProgramCommand = new RelayCommand(LoadProgram);
        LoadGridCommand = new RelayCommand(LoadGrid);
    }

    private List<ICommand>? GetCommands(string s)
    {
        // change all tabs to 4 white spaces
        string newString = s.Replace("\t", "    ");
        
        try
        {
            return new StringParser(newString).Parse();
        }
        catch (Exception e)
        {
            Output = e.Message;
            return null;
        }
    }

    private void GenerateMetrics()
    {
        var cmds = GetCommands(Code); 
        if (cmds is null)
            return;
        var metrics = new MetricsCalculator(new BasicMetricsStrategy());
        Output = metrics.Calculate(cmds);
    }
    
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
                return GridParser.Parse(_customGrid);
            default:
                return Grid.XSquareFalse(10);
        }
    }
    
    private void GenerateOutput()
    {
        var cmds = GetCommands(Code);
        if (cmds is null)
            return;
        
        var c = Character;
        try
        {
            DoCommands(c, cmds);
            BindBitmap(c);
            UpdateOutput(c);
        }
        catch (Exception e)
        {
            Output =  e.Message;
        }
    }

    private void UpdateOutput(Character c)
    {
        Output = string.Join(", ", c.Moves) + ".";
        Output += $"\nEnd state {c}";
        if (c.GridHasFinish())
            Output += "\n" + c.HasFinished();
    }
    private void DoCommands(Character c, List<ICommand> cmds)
    {
        foreach (var cmd in cmds) 
            cmd.Execute(c);
    }
    private void BindBitmap(Character character)
    {
        MemoryStream memoryStream = new MemoryStream();
        OutputDrawer.GenerateBitmap(character, memoryStream);
        OutputImageBinding = new Bitmap(memoryStream);
    }

    private async void LoadProgram()
    {
        try
        {
            string? contents = await _mainWindow.GetFileContents();
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
            string? contents = await _mainWindow.GetFileContents();
            if (contents is not null)
            {
                _customGrid = contents;
                SelectedGrid = 4; // set to custom
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
        BindBitmap(Character);
    }

    public void LoadExampleProgram(int index)
    {
        if (index is < 0 or > 5)
            return;
        
        string fileName = $"avares://MSOAvaloniaApp/Assets/Example{index}.txt";

        using TextReader reader = new StreamReader(AssetLoader.Open(new Uri(fileName)));
        Code = reader.ReadToEnd();
    }
}