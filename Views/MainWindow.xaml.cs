using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LogSearchTool.Utils;
using LogSearchTool.ViewModels;
using SharpDX.Direct3D11;
using SharpDX.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LogSearchTool.Views
{
    public class MainWindow : Window
    {
        private MainWindowViewModel viewModel
        {
            get
            {
                if (DataContext is MainWindowViewModel)
                {
                    return (DataContext as MainWindowViewModel);
                }

                return null;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void ImportButtonClick(object sender, RoutedEventArgs args)
        {
            var dialog = new OpenFolderDialog();

            var result = await dialog.ShowAsync(this);

            if (!string.IsNullOrEmpty(result) && viewModel != null)
            {
                viewModel.LogFilePath = result;
            }
        }

        public void ExtraButtonClick(object sender, RoutedEventArgs args)
        {
            string logFilePath = viewModel.LogFilePath;

            if (!string.IsNullOrEmpty(logFilePath))
            {
                MessageBox.Show(this, "Waitting for Extra", "Extra", MessageBox.MessageBoxButtons.None,
                () =>
                {
                    FileUtil.Extra(new DirectoryInfo(logFilePath));
                });
            }
        }

        public async void ExportButtonClick(object sender, RoutedEventArgs args)
        {
            if (!viewModel.IsSearchCompleted)
            {
                await MessageBox.Show(this, "Please waitting for search complete", "Waitting", MessageBox.MessageBoxButtons.Ok);

                return;
            }

            var dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Log File", Extensions = { "log" } });
            dialog.InitialFileName = $"log_export_{DateTime.Now.Ticks}";

            var path = await dialog.ShowAsync(this);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            using (var streamWriter = File.Create(path))
            {
                var searchResults = viewModel.SearchResults;

                foreach (var searchResult in searchResults)
                {
                    searchResult.Content.ForEach(result =>
                    {
                        streamWriter.Write(System.Text.Encoding.UTF8.GetBytes(result.ToString() + Environment.NewLine));
                    });
                }
            }
        }
    }
}
