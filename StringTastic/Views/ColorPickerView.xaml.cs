using System;
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
            var colorDialog = new System.Windows.Forms.ColorDialog
            {
                AllowFullOpen = true,
                FullOpen = true,
                AnyColor = true
            };

            // Set the initial color from the current hex value
            try
            {
                var currentColor = (SolidColorBrush)ColorButton.Background;
                colorDialog.Color = System.Drawing.Color.FromArgb(
                    currentColor.Color.A,
                    currentColor.Color.R,
                    currentColor.Color.G,
                    currentColor.Color.B);
            }
            catch
            {
                // If parsing fails, use default color
                colorDialog.Color = System.Drawing.Color.Green;
            }

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var selectedColor = colorDialog.Color;
                
                // Convert System.Drawing.Color to WPF Color
                var wpfColor = Color.FromArgb(
                    selectedColor.A,
                    selectedColor.R,
                    selectedColor.G,
                    selectedColor.B);

                // Update button background
                ColorButton.Background = new SolidColorBrush(wpfColor);

                // Update hex color text
                string hexColor = $"#{wpfColor.R:X2}{wpfColor.G:X2}{wpfColor.B:X2}";
                ColorButton.Content = hexColor;
                HexColorTextBox.Text = hexColor;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            HexColorTextBox.Clear();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(HexColorTextBox.Text))
            {
                Clipboard.SetText(HexColorTextBox.Text);
            }
        }
    }
}
