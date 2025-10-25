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
    
    public IRelayCommand OutputCommand { get; }
    
    public MainWindowViewModel(Character character)
    {
        _character = character;
        OutputCommand = new RelayCommand(GenerateOutput);
    }

    private void GenerateOutput()
    {
        var cmds = new StringParser(Code).Parse();
        
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