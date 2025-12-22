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
    }
}
