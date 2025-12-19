using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic.Views
{
    public partial class ManipulationView : UserControl
    {
        public ManipulationView()
        {
            InitializeComponent();
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActionComboBox.SelectedItem is ActionItem selectedItem)
            {
                string action = selectedItem.Tag;

                switch (action)
                {
                    case "Trim":
                        Trim_Click(sender, e);
                        break;
                    case "Unique":
                        Unique_Click(sender, e);
                        break;
                    default:
                        MessageBox.Show("Please select an action from the dropdown.", "No Action Selected", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Please select an action from the dropdown.", "No Action Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Unique_Click(object sender, RoutedEventArgs e)
        {
            List<string> listOfStrings = RtbManipulate.ToListOfString();

            RtbManipulate.Clear();

            StringBuilder sb = new StringBuilder();
            foreach (var singleItem in listOfStrings.Select(item => item).Distinct())
            {
                sb.AppendLine(singleItem);
            }

            RtbManipulate.LogMessage(sb.ToString(), Brushes.Black);
        }

        private void Trim_Click(object sender, RoutedEventArgs e)
        {
            RtbManipulate.TrimLines();
        }
    }
}
