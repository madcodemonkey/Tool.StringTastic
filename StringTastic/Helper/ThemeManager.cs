using System;
using System.Linq;
using System.Windows;

namespace StringTastic.Helper
{
    /// <summary>
    /// Manages application-wide theme switching
    /// </summary>
    public static class ThemeManager
    {
        private const string ThemeResourcePrefix = "Themes/";
        private const string ThemeResourceSuffix = "Theme.xaml";

        private static Theme _currentTheme = Theme.BattleshipGray;

        /// <summary>
        /// Gets the currently active theme
        /// </summary>
        public static Theme CurrentTheme
        {
            get => _currentTheme;
            private set => _currentTheme = value;
        }

        /// <summary>
        /// Applies the specified theme to the application
        /// </summary>
        /// <param name="theme">The theme to apply</param>
        public static void ApplyTheme(Theme theme)
        {
            // Remove existing theme dictionaries
            RemoveThemeDictionaries();

            // Build the theme resource URI
            string themeFileName = GetThemeFileName(theme);
            var themeUri = new Uri($"/{typeof(ThemeManager).Assembly.GetName().Name};component/{ThemeResourcePrefix}{themeFileName}", UriKind.Relative);

            // Load the new theme dictionary
            var themeDict = new ResourceDictionary { Source = themeUri };

            // Add it to the application resources
            Application.Current.Resources.MergedDictionaries.Add(themeDict);

            // Update current theme
            CurrentTheme = theme;
        }

        /// <summary>
        /// Removes all theme resource dictionaries from the application
        /// </summary>
        private static void RemoveThemeDictionaries()
        {
            // Find and remove all theme dictionaries
            var themeDictionaries = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null && d.Source.OriginalString.Contains(ThemeResourcePrefix))
                .ToList();

            foreach (var dict in themeDictionaries)
            {
                Application.Current.Resources.MergedDictionaries.Remove(dict);
            }
        }

        /// <summary>
        /// Gets the file name for the specified theme
        /// </summary>
        private static string GetThemeFileName(Theme theme)
        {
            switch (theme)
            {
                case Theme.Light:
                    return "LightTheme.xaml";
                case Theme.Dark:
                    return "DarkTheme.xaml";
                case Theme.BattleshipGray:
                    return "BattleshipGrayTheme.xaml";
                default:
                    throw new ArgumentOutOfRangeException(nameof(theme), theme, "Unknown theme");
            }
        }

        /// <summary>
        /// Gets a friendly display name for the theme
        /// </summary>
        public static string GetThemeDisplayName(Theme theme)
        {
            switch (theme)
            {
                case Theme.Light:
                    return "Light";
                case Theme.Dark:
                    return "Dark";
                case Theme.BattleshipGray:
                    return "Battleship Gray";
                default:
                    return theme.ToString();
            }
        }
    }

    /// <summary>
    /// Available application themes
    /// </summary>
    public enum Theme
    {
        /// <summary>
        /// Light theme with bright colors
        /// </summary>
        Light,

        /// <summary>
        /// Dark theme for low-light environments
        /// </summary>
        Dark,

        /// <summary>
        /// Default battleship gray theme (current application colors)
        /// </summary>
        BattleshipGray
    }
}
