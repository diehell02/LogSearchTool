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

        public void SearchButtonClick()
        {
            searchResultStringBuilder = new StringBuilder();
            OnPropertyChanged(nameof(SearchResultText));

            SearchUtil.SearchKeyWord(SearchText, new System.IO.DirectoryInfo(LogFilePath), (result) =>
            {
                searchResultStringBuilder.AppendLine(result);
                OnPropertyChanged(nameof(SearchResultText));
            });
        }
    }
}
