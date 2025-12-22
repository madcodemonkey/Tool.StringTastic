using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic.Views
{
    public partial class ColorPickerView : UserControl
    {
        public ColorPickerView()
        {
            InitializeComponent();
        }

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();
            
            // Try to set initial color from current color
            try
            {
                var currentColor = (Color)ColorConverter.ConvertFromString(HexColorTextBox.Text);
                colorDialog.Color = System.Drawing.Color.FromArgb(currentColor.A, currentColor.R, currentColor.G, currentColor.B);
            }
            catch
            {
                // If parsing fails, use default
            }

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedColor = colorDialog.Color;
                var hexColor = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}";
                
                HexColorTextBox.Text = hexColor;
                ColorButton.Content = hexColor;
                ColorButton.Background = new SolidColorBrush(Color.FromRgb(selectedColor.R, selectedColor.G, selectedColor.B));
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            HexColorTextBox.Text = "";
            ColorButton.Content = "#000000";
            ColorButton.Background = new SolidColorBrush(Colors.Black);
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(HexColorTextBox.Text))
            {
                Clipboard.SetText(HexColorTextBox.Text);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hexColor = HexColorTextBox.Text.Trim();
                
                if (string.IsNullOrWhiteSpace(hexColor))
                    return;
                
                // Parse the hex color
                var color = (Color)ColorConverter.ConvertFromString(hexColor);
                
                // Update the color button
                ColorButton.Content = hexColor;
                ColorButton.Background = new SolidColorBrush(color);
            }
            catch
            {
                MessageBox.Show("Invalid hex color format. Please use format like #RRGGBB or #AARRGGBB", 
                    "Invalid Color", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
