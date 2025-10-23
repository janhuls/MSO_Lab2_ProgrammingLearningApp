using MSOProgramLearningApp;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel(Character character) : ViewModelBase
{
    private readonly Character _character = character;
    
    public string Greeting { get; } = "bababa";
}