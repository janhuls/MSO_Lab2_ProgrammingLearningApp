using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MSOAvaloniaApp.ViewModels;
using MSOProgramLearningApp;

namespace MSOAvaloniaApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this);
    }

    
    public async Task<string?> getFileContents()
    {
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
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

    private void GridSizeChanged(object? sender, ContainerIndexChangedEventArgs e)
    {
        //MainWindowViewModel mw = DataContext as MainWindowViewModel;
        //mw.ChangeGrid(e.NewIndex);
    }

    private void GridSizeChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        //Console.WriteLine(e + "  " + e.NewValue);
    }

    private void GridSizeChanged(object? sender, SelectionChangedEventArgs e)
    {
        Console.WriteLine($"aadd: {e.AddedItems} weg: {e.RemovedItems}");
    }
}