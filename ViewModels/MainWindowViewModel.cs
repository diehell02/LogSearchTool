using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using LogSearchTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.IO;
using Avalonia.Collections;

namespace LogSearchTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private List<string> filesToIncludeList
        {
            get
            {
                if (string.IsNullOrEmpty(FilesToInclude))
                {
                    return new List<string>();
                }

                return new List<string>(FilesToInclude.Split(','));
            }
        }

        private List<string> filesToExcludeList
        {
            get
            {
                if (string.IsNullOrEmpty(FilesToExclude))
                {
                    return new List<string>();
                }

                return new List<string>(FilesToExclude.Split(','));
            }
        }

        private string logFilePath;
        private AvaloniaList<StringBuilder> searchResultText;

        public string LogFilePath
        {
            get => logFilePath;
            set
            {
                logFilePath = value;
                OnPropertyChanged(nameof(LogFilePath));
            }
        }

        public string SearchText { get; set; } = string.Empty;

        public AvaloniaList<StringBuilder> SearchResultText
        {
            get => searchResultText;
            private set
            {
                searchResultText = value;
                OnPropertyChanged(nameof(SearchResultText));
            }
        }

        public string FilesToInclude
        {
            get;
            set;
        }

        public string FilesToExclude
        {
            get;
            set;
        }

        public MainWindowViewModel()
        {
            FilesToInclude = ".log";
            searchResultText = new AvaloniaList<StringBuilder>();
        }

        public async void SearchButtonClick()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return;
            }

            SearchResultText.Clear();

            await SearchUtil.SearchKeyWord(SearchText, new DirectoryInfo(LogFilePath),
                filesToIncludeList, filesToExcludeList, results =>
                {
                    Console.WriteLine($"{DateTime.Now} search result return");
                    foreach (var result in results)
                    {
                        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            SearchResultText.Add(new StringBuilder(result.FileInfo.Name));
                            SearchResultText.Add(new StringBuilder("----------------------------------------------------------"));

                            SearchResultText.AddRange(result.Content);
                        });
                    }

                    Console.WriteLine($"{DateTime.Now} search result finish");
                });
        }
    }
}
