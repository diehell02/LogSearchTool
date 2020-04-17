using Avalonia.Controls;
using LogSearchTool.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LogSearchTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private StringBuilder searchResultStringBuilder = new StringBuilder();

        private List<string> filesToIncludeList {
            get {
                if (string.IsNullOrEmpty(FilesToInclude)) {
                    return new List<string>();
                }

                return new List<string>(FilesToInclude.Split(','));
            }
        }

        private List<string> filesToExcludeList {
            get {
                if (string.IsNullOrEmpty(FilesToExclude)) {
                    return new List<string>();
                }

                return new List<string>(FilesToExclude.Split(','));
            }
        }

        private string logFilePath;
        public string LogFilePath { 
            get => logFilePath; 
            set
            {
                logFilePath = value;
                OnPropertyChanged(nameof(LogFilePath));
            }
        }

        public string SearchText { get; set; } = string.Empty;

        public string SearchResultText
        {
            get => searchResultStringBuilder.ToString();
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

        public MainWindowViewModel() {
            FilesToInclude = ".log";
        }

        public void SearchButtonClick()
        {
            if (string.IsNullOrEmpty(SearchText)) 
            {
                return;
            }

            searchResultStringBuilder = new StringBuilder();
            OnPropertyChanged(nameof(SearchResultText));

            SearchUtil.SearchKeyWord(SearchText, new System.IO.DirectoryInfo(LogFilePath),
                filesToIncludeList, filesToExcludeList, (result) =>
            {
                searchResultStringBuilder.AppendLine(result);
                OnPropertyChanged(nameof(SearchResultText));
            });
        }
    }
}
