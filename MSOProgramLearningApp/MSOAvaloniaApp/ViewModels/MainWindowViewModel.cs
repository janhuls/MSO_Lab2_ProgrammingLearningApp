using System.Reactive;
using System.Reactive.Concurrency;
using MSOProgramLearningApp;
using ReactiveUI;
using System.Threading.Tasks;

namespace MSOAvaloniaApp.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    public string Greeting { get; set; } = "bababa";
    
    private readonly Character _character;
    public ReactiveCommand<Unit, Unit> OutputCommand { get; }
    
    private string _code = "type ur code here broski";

    public string Code
    {
        get => _code;
        set => this.RaiseAndSetIfChanged(ref _code, value);
    }
    private string _output = "Output";
    public string Output
    {
        get => _output;
        private set => this.RaiseAndSetIfChanged(ref _output, value);
    }

    public MainWindowViewModel(Character character)
    {
        _character = character;
        OutputCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // parse code off the UI thread
            var cmds = await Task.Run(() => new StringParser(Code).Parse());

            // update UI on main thread
            var c = _character;
            foreach (var cmd in cmds)
                cmd.Execute(c);

            Output = string.Join(", ", c.Moves) + ".";
            Output += $"\nEnd state {c}";
        }, outputScheduler: RxApp.MainThreadScheduler);
    }

    public MainWindowViewModel() : this(new Character(Grid.TenSquareFalse()))
    {
    }
}