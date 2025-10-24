using MSOProgramLearningApp;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly Character _character;
    
    public string Greeting { get; set; } = "bababa";


    public MainWindowViewModel()
    {
        _character = new Character(Grid.TenSquareFalse());
    }

    public MainWindowViewModel(Character character)
    {
        _character = character;
    }
}