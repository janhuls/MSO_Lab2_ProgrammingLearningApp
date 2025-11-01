using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MSOAvaloniaApp.ViewModels;

namespace MSOAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this);
    }
    //open the file explorer
    public async Task<string?> GetFileContents()
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        if (topLevel == null) return null;
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.TextPlain] // only text files are allowed
        });

        if (files.Count >= 1)
        {
            // Open reading stream from the first file.
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);
            // Reads all the content of file as a text.
            return await streamReader.ReadToEndAsync();
        }

        return null;
    }
    //load the examples through button clicks
    private void LoadBuildInExample(object? sender, RoutedEventArgs e)
    {
        MainWindowViewModel? vm = (MainWindowViewModel)DataContext!;
        if(vm == null)
            throw new NullReferenceException("MainWindowViewModel is null (je bent cooked)");
        
        Button? button = sender as Button;
        if (button == null)
            throw new NullReferenceException("button is null (je bent cooked)");

        string? text = button.Content as string;
        if (text == null) throw new NullReferenceException("text is null (je bent cooked)");
        
        int index = int.Parse(text.Split()[2]);
        vm.LoadExampleProgram(index);
    }
}