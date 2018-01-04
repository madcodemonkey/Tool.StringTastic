using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StringTastic.ViewModels;

namespace StringTastic
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _Model = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _Model;
        }
        
        private void SortLeftButton_Click(object sender, RoutedEventArgs e)
        {
            SortRichTextBox(RtbLeftItems);
        }

        private void SortRightButton_Click(object sender, RoutedEventArgs e)
        {
            SortRichTextBox(RtbRightItems);
        }

        private void PutUniqueLeftItemsInRightRtbButton_Click(object sender, RoutedEventArgs e)
        {
            MakeUniqueItems(RtbLeftItems, RtbRightItems);
        }

        private void PutUniqueRighttemsInLeftRtbButton_Click(object sender, RoutedEventArgs e)
        {
            MakeUniqueItems(RtbRightItems, RtbLeftItems);
        }

        private void MakeLeftItemsUniqueButton_Click(object sender, RoutedEventArgs e)
        {
            MakeUniqueItems(RtbLeftItems, RtbLeftItems);
        }

        private void MakeRightItemsUniqueButton_Click(object sender, RoutedEventArgs e)
        {
            MakeUniqueItems(RtbRightItems, RtbRightItems);
        }

        private void TrimLeftItemsButton_Click(object sender, RoutedEventArgs e)
        {
            TrimItems(RtbLeftItems);
        }

        private void TrimRightItemsButton_Click(object sender, RoutedEventArgs e)
        {
            TrimItems(RtbRightItems);
        }

        private void ShowDifferencesButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> leftStrings = RtbLeftItems.ToListOfString();
            List<string> rightStings = RtbRightItems.ToListOfString();

            var differences = new List<string>();

            // Find items that are not in the RIGHT list of strings
            differences.Add("In LEFT SIDE ONLY:");
            foreach (var item in leftStrings)
            {
                string itemFound = rightStings.FirstOrDefault(i => String.Compare(i, item, StringComparison.OrdinalIgnoreCase) == 0);
                if (itemFound == null)
                    differences.Add(item);
            }

            // Find items that are not in the LEFT list of strings
            differences.Add("--------------------------");
            differences.Add("In RIGHT SIDE ONLY:");
            foreach (var item in rightStings)
            {
                string itemFound = leftStrings.FirstOrDefault(i => String.Compare(i, item, StringComparison.OrdinalIgnoreCase) == 0);
                if (itemFound == null)
                    differences.Add(item);
            }

            // Show the results
            var myDialog = new RichTextDialogBox(differences, "Differences");
            myDialog.Show();
        }

        private void ShowSimilaritiesButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> leftStrings = RtbLeftItems.ToListOfString();
            List<string> rightStings = RtbRightItems.ToListOfString();

            var similarities = new List<string>();

            // Find items that are not in the RIGHT list of strings
            foreach (var item in leftStrings)
            {
                string itemFound = rightStings.FirstOrDefault(i => String.Compare(i, item, StringComparison.OrdinalIgnoreCase) == 0);
                if (itemFound != null)
                    similarities.Add(item);
            }

            // Show the results
            var myDialog = new RichTextDialogBox(similarities, "Similarities");
            myDialog.Show();
  
        }


        #region RichTextBox methods
        private void SortRichTextBox(RichTextBox rtb)
        {
            List<string> listOfStrings = rtb.ToListOfString();
            ClearRichTextBox(rtb);
            foreach (var item in listOfStrings.OrderBy(item => item))
                rtb.LogMessage(item, Brushes.Black);
        }


        private void ClearRichTextBox(RichTextBox rtb)
        {
            rtb.Document.Blocks.Clear();
        }

    

        private void MakeUniqueItems(RichTextBox source, RichTextBox destination)
        {
            List<string> listOfStrings = source.ToListOfString();

            if (source == destination)
               ClearRichTextBox(destination);

            foreach (var singleItem in listOfStrings.Select(item => item).Distinct())
            {
                destination.LogMessage(singleItem, Brushes.Black);
            }
        }

        private void TrimItems(RichTextBox source)
        {
            List<string> listOfStrings = source.ToListOfString();

            ClearRichTextBox(source);


            foreach (var singleItem in listOfStrings)
            {
                string message = singleItem.Trim();
                if (string.IsNullOrEmpty(message) == false)
                    source.LogMessage(message, Brushes.Black);
            }
        }
        #endregion

       
    }
}
