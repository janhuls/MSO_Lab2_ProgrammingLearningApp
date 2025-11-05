using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSOAvaloniaApp.Views;
using MSOProgramLearningApp;
using Grid = MSOProgramLearningApp.Grid;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly MainWindow _mainWindow;

    [ObservableProperty] //represents the input field
    private string _code =
        "Repeat 3 \n    RepeatUntil WallAhead \n        Move 1 \n    Turn right \nRepeat 2 \n    RepeatUntil GridEdge \n        Move 1 \n    Turn right\n";

    private string _customGrid = "";

    [ObservableProperty] //represents the output field 
    private string _output = "Output";

    [ObservableProperty] private IImage? _outputImageBinding;
    private int _selectedGrid = 1;

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
        _mainWindow = new MainWindow();
        OutputCommand = new RelayCommand(GenerateOutput);
        MetricsCommand = new RelayCommand(GenerateMetrics);
        LoadProgramCommand = new RelayCommand(LoadProgram);
        LoadGridCommand = new RelayCommand(LoadGrid);
    }

    private Character Character => new(GetGrid());
    public IRelayCommand MetricsCommand { get; }
    public IRelayCommand OutputCommand { get; }
    public IRelayCommand LoadProgramCommand { get; }
    public IRelayCommand LoadGridCommand { get; }

    public int SelectedGrid
    {
        get => _selectedGrid;
        set
        {
            if (SetProperty(ref _selectedGrid, value)) BindBitmap(Character); // update the bitmap
        }
    }

    //Parses the input string
    private List<ICommand>? GetCommands(string s)
    {
        // change all tabs to 4 white spaces
        var newString = s.Replace("\t", "    ");

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

    //Displays the metrics for the code field in the output field
    private void GenerateMetrics()
    {
        var cmds = GetCommands(Code);
        if (cmds is null)
            return;
        var metrics = new MetricsCalculator(new BasicMetricsStrategy());
        Output = metrics.Calculate(cmds);
    }

    //returns the grid currently selected
    private Grid GetGrid()
    {
        switch (_selectedGrid)
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
                    Output =
                        "ERROR: No custom grid loaded, Press the load grid button to load a custom grid.\nLoaded a 10x10 grid as default";
                    return Grid.XSquareFalse(10);
                }

                try
                {
                    return GridParser.Parse(_customGrid);
                }
                catch (Exception e)
                {
                    Output = "ERROR parsing custom grid: " + e.Message + "\nLoaded a 10x10 grid as default";
                    return Grid.XSquareFalse(10);
                }

            case 5:
                var parseText = ReadTextAsset("exampleGrid");
                if (parseText is not null) return GridParser.Parse(parseText);
                Output = "ERROR: Failed to load example grid asset.\nLoaded a 10x10 grid as default";
                return Grid.XSquareFalse(10);

            case 6:
                var parseTexxt = ReadTextAsset("challengeGrid1");
                if (parseTexxt is not null) return GridParser.Parse(parseTexxt);
                Output = "ERROR: Failed to load example grid asset.\nLoaded a 10x10 grid as default";
                return Grid.XSquareFalse(10);

            default:
                return Grid.XSquareFalse(10);
        }
    }

    //runs the code in the code field and returns the result in the output field
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
            Output = e.Message;
        }
    }

    //display result in output field
    private void UpdateOutput(Character c)
    {
        Output = string.Join(", ", c.Moves) + ".";
        Output += $"\nEnd state {c}";
        if (c.GridHasFinish())
            Output += "\n" + c.HasFinished();
    }

    //execute commands on the character
    private static void DoCommands(Character c, List<ICommand> cmds)
    {
        foreach (var cmd in cmds)
            cmd.Execute(c);
    }

    //updates the image displayed
    private void BindBitmap(Character character)
    {
        var memoryStream = new MemoryStream();
        OutputDrawer.GenerateBitmap(character, memoryStream);
        OutputImageBinding = new Bitmap(memoryStream);
    }

    //load file into code field
    private async void LoadProgram()
    {
        try
        {
            var contents = await _mainWindow.GetFileContents();
            if (contents is not null)
                Code = contents;
        }
        catch (Exception e)
        {
            Output = $"Failed to load program file\nError: {e.Message}";
        }
    }

    //load exercise grid file into grid
    private async void LoadGrid()
    {
        try
        {
            var contents = await _mainWindow.GetFileContents();
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

    //Loads example program from assets folder
    public void LoadExampleProgram(int index)
    {
        if (index is < 0 or > 5)
            return;

        Code = ReadTextAsset($"Example{index}") ?? $"ERROR reading example program asset at index {index}";
    }

    //returns name.txt as string, if in assets folder
    private string? ReadTextAsset(string name)
    {
        var fileName = $"avares://MSOAvaloniaApp/Assets/{name}.txt";

        try
        {
            using TextReader reader = new StreamReader(AssetLoader.Open(new Uri(fileName)));
            return reader.ReadToEnd();
        }
        catch (Exception e)
        {
            Output = $"Error reading file: {name} error: {e.Message}";
            return null;
        }
    }
}