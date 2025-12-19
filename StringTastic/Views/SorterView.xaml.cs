using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic.Views
{
    public partial class SorterView : UserControl
    {
        public SorterView()
        {
            InitializeComponent();
        }

        private void SortAscendingButton_Click(object sender, RoutedEventArgs e)
        {
            RtbSort.SortRichTextBox(sortAscending: true);
        }

        private void SortDescendingButton_Click(object sender, RoutedEventArgs e)
        {
            RtbSort.SortRichTextBox(sortAscending: false);
        }

        private void UniqueButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> listOfStrings = RtbSort.ToListOfString();

            RtbSort.Clear();

            StringBuilder sb = new StringBuilder();
            foreach (var singleItem in listOfStrings.Select(item => item).Distinct())
            {
                sb.AppendLine(singleItem);
            }

            RtbSort.LogMessage(sb.ToString(), Brushes.Black);
        }

        private void TrimButton_Click(object sender, RoutedEventArgs e)
        {
            RtbSort.TrimLines();
        }
    }
}
