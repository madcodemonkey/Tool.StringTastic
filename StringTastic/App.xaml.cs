using System;
using System.Reflection;
using System.Windows;
using StringTastic.Helper;

namespace StringTastic
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Ensure menu submenus open to the right application-wide
            SetDropDownMenuToBeRightAligned();

            // Load and apply saved theme preference
            var savedTheme = ThemeManager.LoadThemePreference();
            ThemeManager.ApplyTheme(savedTheme);

            base.OnStartup(e);
        }

        /// <summary>
        /// This is a fix for menus so that they present properly.
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

            SystemParameters.StaticPropertyChanged += (sender, args) =>
            {
                setAlignmentValue();
            };
        }
    }
}
