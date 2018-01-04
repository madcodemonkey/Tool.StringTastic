using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;

namespace StringTastic
{
    public partial class RichTextDialogBox : Window
    {
        public RichTextDialogBox(List<string> items, string title)
        {
            InitializeComponent();
            LogItems(items);
            this.Title = title;
            ItemCountLabel.Content = string.Format("{0} items listed", items.Count);
        }

        private void LogItems(List<string> items)
        {
            rtbData.Document.Blocks.Clear();

            foreach (var item in items)
            {
                Paragraph p = new Paragraph(new Run(item));
                p.Foreground = Brushes.Black;
                rtbData.Document.Blocks.Add(p);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            
            if (dialog.ShowDialog() != true)
                return;

            using (var fs = new FileStream(dialog.FileName, FileMode.Create))
            {
                var myTextRange = new TextRange(rtbData.Document.ContentStart, rtbData.Document.ContentEnd);
                myTextRange.Save(fs, DataFormats.Text);
            }
        }
    }
}
