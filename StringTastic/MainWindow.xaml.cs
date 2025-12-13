using StringTastic.ViewModels;
using StringTastic.Views;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace StringTastic
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _Model = new MainViewModel();
        private int _compareCount = 0;
        private int _manipulationCount = 0;

        public ICommand CloseTabCommand { get; }
        public ICommand CloseOthersCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _Model;

            // provide commands for context menu (still available if needed)
            CloseTabCommand = new RelayCommand(o => CloseTabCommandExecute(o));
            CloseOthersCommand = new RelayCommand(o => CloseOthersCommandExecute(o));

            // Subscribe to ViewModel events to handle tab actions from commands
            _Model.RequestNewCompare += (s, e) => CreateCompareTab();
            _Model.RequestNewManipulation += (s, e) => CreateManipulationTab();
            _Model.RequestCloseCurrentTab += (s, e) =>
            {
                if (MainTabControl.SelectedItem is TabItem ti)
                    MainTabControl.Items.Remove(ti);
            };
            _Model.RequestCloseAllTabs += (s, e) => MainTabControl.Items.Clear();
            _Model.RequestExit += (s, e) => Close();

            // Create initial tabs to preserve previous behavior
            CreateCompareTab();
            CreateManipulationTab();
        }

        private void CloseTabCommandExecute(object parameter)
        {
            var target = parameter as TabItem;
            if (target == null)
            {
                // If command parameter wasn't a TabItem, try to resolve from ContextMenu placement target
                var cm = parameter as ContextMenu;
                target = cm?.PlacementTarget as TabItem;
            }

            if (target != null)
            {
                MainTabControl.Items.Remove(target);
            }
        }

        private void CloseOthersCommandExecute(object parameter)
        {
            var target = parameter as TabItem;
            if (target == null)
            {
                var cm = parameter as ContextMenu;
                target = cm?.PlacementTarget as TabItem;
            }

            if (target != null)
            {
                for (int i = MainTabControl.Items.Count - 1; i >= 0; i--)
                {
                    var item = MainTabControl.Items[i] as TabItem;
                    if (item != null && !ReferenceEquals(item, target))
                        MainTabControl.Items.RemoveAt(i);
                }

                MainTabControl.SelectedItem = target;
            }
        }

        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            if (menuItem == null)
                return;

            // Find the Popup used by this MenuItem's submenu and force its placement
            var popup = FindVisualChild<Popup>(menuItem);
            if (popup != null)
            {
                try
                {
                    popup.Placement = PlacementMode.Right;
                    popup.HorizontalOffset = 0;
                }
                catch
                {
                    // ignore any failures - don't want menu to crash
                }
            }
        }

        private static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = child as T ?? FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
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
            var closeButton = new Button { Content = "", Width = 16, Height = 16, Padding = new Thickness(0), Margin = new Thickness(2, 0, 0, 0) };

            // apply style from resources if available
            var style = TryFindResource("TabCloseButtonStyle") as Style;
            if (style != null)
                closeButton.Style = style;
            else
            {
                // fallback content if style not found
                closeButton.Content = "x";
                closeButton.FontSize = 11;
                closeButton.Foreground = Brushes.Gray;
            }

            closeButton.Click += (s, e) => MainTabControl.Items.Remove(tab);

            headerPanel.Children.Add(headerLabel);
            headerPanel.Children.Add(closeButton);

            tab.Header = headerPanel;
            tab.Content = content;

            // Add right-click context menu directly so handlers fire
            var cm = new ContextMenu();

            var miClose = new MenuItem { Header = "Close" };
            miClose.Click += (s, e) =>
            {
                if (MainTabControl.Items.Contains(tab))
                    MainTabControl.Items.Remove(tab);
            };

            var miCloseOthers = new MenuItem { Header = "Close Others" };
            miCloseOthers.Click += (s, e) =>
            {
                for (int i = MainTabControl.Items.Count - 1; i >= 0; i--)
                {
                    var item = MainTabControl.Items[i] as TabItem;
                    if (item != null && !ReferenceEquals(item, tab))
                        MainTabControl.Items.RemoveAt(i);
                }
                MainTabControl.SelectedItem = tab;
            };

            var miCloseAll = new MenuItem { Header = "Close All" };
            miCloseAll.Click += (s, e) => MainTabControl.Items.Clear();

            cm.Items.Add(miClose);
            cm.Items.Add(miCloseOthers);
            cm.Items.Add(miCloseAll);

            tab.ContextMenu = cm;

            // close tab on middle-click
            tab.PreviewMouseDown += (s, e) =>
            {
                var mbe = e as MouseButtonEventArgs;
                if (mbe != null && mbe.ChangedButton == MouseButton.Middle)
                {
                    // remove the tab
                    MainTabControl.Items.Remove(tab);
                    e.Handled = true;
                }
            };

            return tab;
        }
    }
}
