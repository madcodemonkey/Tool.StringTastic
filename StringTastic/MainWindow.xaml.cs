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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _Model;

            // Ensure menu submenus open to the right
            AddHandler(MenuItem.SubmenuOpenedEvent, new RoutedEventHandler(MenuItem_SubmenuOpened), true);

            // Create initial tabs to preserve previous behavior
            CreateCompareTab();
            CreateManipulationTab();
            SetDropDownMenuToBeRightAligned();
        }

        /// <summary>
        /// This is a fix for menus so that the present properly.
        /// DO NOT REMOVE!
        /// </summary>
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
