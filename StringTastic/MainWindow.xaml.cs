using Newtonsoft.Json.Linq;
using StringTastic.ViewModels;
using StringTastic.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _Model = new MainViewModel();
        private int _compareCount = 0;
        private int _manipulationCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _Model;

            // Create initial tabs to preserve previous behavior
            CreateCompareTab();
            CreateManipulationTab();
            SetDropDownMenuToBeRightAligned();
        }

        private static void SetDropDownMenuToBeRightAligned()
        {
            var menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            Action setAlignmentValue = () =>
            {
                if (SystemParameters.MenuDropAlignment && menuDropAlignmentField != null) menuDropAlignmentField.SetValue(null, false);
            };

            setAlignmentValue();

            SystemParameters.StaticPropertyChanged += (sender, e) =>
            {
                setAlignmentValue();
            };
        }

        private void NewCompareMenu_Click(object sender, RoutedEventArgs e)
        {
            CreateCompareTab();
        }

        private void NewManipulationMenu_Click(object sender, RoutedEventArgs e)
        {
            CreateManipulationTab();
        }

        private void CloseCurrentTabMenu_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem ti)
            {
                MainTabControl.Items.Remove(ti);
            }
        }

        private void CloseAllTabsMenu_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.Items.Clear();
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CreateCompareTab()
        {
            var view = new CompareView();
            // inherit DataContext so commands resolve
            view.DataContext = this.DataContext;

            _compareCount++;
            string headerText = $"Compare {_compareCount}";

            var tab = CreateClosableTab(headerText, view);
            MainTabControl.Items.Add(tab);
            MainTabControl.SelectedItem = tab;
        }

        private void CreateManipulationTab()
        {
            var view = new ManipulationView();
            view.DataContext = this.DataContext;

            _manipulationCount++;
            string headerText = $"Manipulation {_manipulationCount}";

            var tab = CreateClosableTab(headerText, view);
            MainTabControl.Items.Add(tab);
            MainTabControl.SelectedItem = tab;
        }

        private TabItem CreateClosableTab(string headerText, UIElement content)
        {
            var tab = new TabItem();

            // header with text and close button
            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal };
            var headerLabel = new TextBlock { Text = headerText, Margin = new Thickness(0, 0, 5, 0) };
            var closeButton = new Button { Content = "x", Width = 18, Height = 18, Padding = new Thickness(0), Margin = new Thickness(0) };
            closeButton.Click += (s, e) => MainTabControl.Items.Remove(tab);

            headerPanel.Children.Add(headerLabel);
            headerPanel.Children.Add(closeButton);

            tab.Header = headerPanel;
            tab.Content = content;

            return tab;
        }
    }
}
