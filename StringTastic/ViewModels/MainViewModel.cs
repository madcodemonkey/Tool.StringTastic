using System;
using System.ComponentModel;
using System.Windows.Input;

namespace StringTastic.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            RichTextBoxCommon = new RichTextBoxCommonViewModel();

            // initialize commands
            NewCompareCommand = new RelayCommand(_ => OnRequestNewCompare());
            NewManipulationCommand = new RelayCommand(_ => OnRequestNewManipulation());
            CloseCurrentTabCommand = new RelayCommand(_ => OnRequestCloseCurrentTab());
            CloseAllTabsCommand = new RelayCommand(_ => OnRequestCloseAllTabs());
            ExitCommand = new RelayCommand(_ => OnRequestExit());
        }

        public RichTextBoxCommonViewModel RichTextBoxCommon { get; set; }

        // Commands bound from the UI
        public ICommand NewCompareCommand { get; }
        public ICommand NewManipulationCommand { get; }
        public ICommand CloseCurrentTabCommand { get; }
        public ICommand CloseAllTabsCommand { get; }
        public ICommand ExitCommand { get; }

        // Events that the View (MainWindow) can subscribe to in order to perform UI work
        public event EventHandler RequestNewCompare;
        public event EventHandler RequestNewManipulation;
        public event EventHandler RequestCloseCurrentTab;
        public event EventHandler RequestCloseAllTabs;
        public event EventHandler RequestExit;

        private void OnRequestNewCompare()
        {
            RequestNewCompare?.Invoke(this, EventArgs.Empty);
        }

        private void OnRequestNewManipulation()
        {
            RequestNewManipulation?.Invoke(this, EventArgs.Empty);
        }

        private void OnRequestCloseCurrentTab()
        {
            RequestCloseCurrentTab?.Invoke(this, EventArgs.Empty);
        }

        private void OnRequestCloseAllTabs()
        {
            RequestCloseAllTabs?.Invoke(this, EventArgs.Empty);
        }

        private void OnRequestExit()
        {
            RequestExit?.Invoke(this, EventArgs.Empty);
        }

        protected void OnPropertyChanged(string propertyname)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyname));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}