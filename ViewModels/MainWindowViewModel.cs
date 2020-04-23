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
using DynamicData;

namespace LogSearchTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
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

        public IList<SearchUtil.SearchResult> SearchResults
        {
            get;
            private set;
        }

        public AvaloniaList<string> IncludeFiles
        {
            get;
            private set;
        }

        public string IncludeFileName
        {
            get;
            set;
        }

        public bool IsSearchCompleted
        {
            get;
            private set;
        }

        public MainWindowViewModel()
        {
            IncludeFiles = new AvaloniaList<string>()
            {
                "", "action.log", "main.log", "network.log", "rcv.log", "rcv_media_stats.log", "webrtc.log", "websocket.log"
            };
            searchResultText = new AvaloniaList<StringBuilder>();
            SearchResults = new List<SearchUtil.SearchResult>();
        }

        public async void SearchButtonClick()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return;
            }

            SearchResultText.Clear();
            SearchResults.Clear();

            var filesToInclude = new List<string>();

            if (!string.IsNullOrEmpty(IncludeFileName))
            {
                filesToInclude.Add(IncludeFileName);
            }

            await SearchUtil.SearchKeyWord(SearchText, new DirectoryInfo(LogFilePath),
                filesToInclude, (results, isCompleted) =>
                {
                    Console.WriteLine($"{DateTime.Now} search result return");

                    SearchResults.AddRange(results);

                    Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        IsSearchCompleted = isCompleted;

                        foreach (var result in results)
                        {
                            SearchResultText.Add(new StringBuilder(result.FileInfo.Name));
                            SearchResultText.Add(new StringBuilder("----------------------------------------------------------"));

                            SearchResultText.AddRange(result.Content);
                        }
                    });

                    Console.WriteLine($"{DateTime.Now} search result finish");
                });
        }
    }
}
