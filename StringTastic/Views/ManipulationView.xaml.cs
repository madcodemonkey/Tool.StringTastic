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
                    case "JwtDecode":
                        JwtTokenDecodeButton_Click(sender, e);
                        break;
                    case "GenerateGuids":
                        GenerateGuidsButton_Click(sender, e);
                        break;
                    case "SortAscending":
                        SortAscending_Click(sender, e);
                        break;
                    case "SortDescending":
                        SortDescending_Click(sender, e);
                        break;
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

        private void JwtTokenDecodeButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                string encodedData = RtbManipulate.ToOneString(true).Trim();

                var encodedParts = encodedData.Split('.');

                if (encodedParts.Length != 3)
                {
                    RtbManipulate.LogMessage("Invalid JWT token.  It should have three distinct parts!", Brushes.Black);
                    return;
                }

                string part1 = Base64Decode(encodedParts[0]);
                string part2 = Base64Decode(encodedParts[1]);

                RtbManipulate.Clear();

                sb.AppendLine("Header:");
                var part1Object = JObject.Parse(part1);
                sb.AppendLine(part1Object.ToString());

                sb.AppendLine();
                sb.AppendLine("Payload:");
                var part2Object = JObject.Parse(part2);
                sb.AppendLine(part2Object.ToString());

                sb.AppendLine();
                sb.AppendLine("Signature:");
                sb.AppendLine("[Encoded Signature]");

                DecodeDate(sb, "Payload Issued date (iat)", "iat", part2Object);
                DecodeDate(sb, "Payload Expiration date (exp)", "exp", part2Object);
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("----------------");
                sb.AppendLine(ex.Message);
            }
            finally
            {
                RtbManipulate.LogMessage(sb.ToString(), Brushes.Black);
            }
        }

        private void DecodeDate(StringBuilder sb, string title, string propertyName, JObject part2Object)
        {
            sb.AppendLine();
            var expiration = part2Object[propertyName];

            if (expiration == null || string.IsNullOrWhiteSpace(expiration.ToString()))
            {
                sb.AppendLine($"{title} not found.");
            }
            else if (int.TryParse(expiration.ToString(), out var exp))
            {
                var startDate = new DateTime(1970, 1, 1);
                var expDate = startDate.AddSeconds(exp);
                sb.AppendLine($"{title} = {expDate}");
            }
            else
            {
                sb.AppendLine($"Unable to parse {title}.");
            }
        }

        private string Base64Decode(string base64EncodedData)
        {
            int mod4 = base64EncodedData.Length % 4;
            if (mod4 > 0)
            {
                base64EncodedData += new string('=', 4 - mod4);
            }

            var plainTextBytes = System.Convert.FromBase64String(base64EncodedData);
            var result = System.Text.Encoding.UTF8.GetString(plainTextBytes);

            return result;
        }

        private void GenerateGuidsButton_Click(object sender, RoutedEventArgs e)
        {
            RtbManipulate.Clear();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("10 new GUIDs style D (lowercase)");
            for (int i = 0; i < 10; i++)
            {
                sb.AppendLine(Guid.NewGuid().ToString("D").ToLower());
            }

            sb.AppendLine();
            sb.AppendLine("10 new GUIDs style D (uppercase)");
            for (int i = 0; i < 10; i++)
            {
                sb.AppendLine(Guid.NewGuid().ToString("D").ToUpper());
            }

            sb.AppendLine();
            sb.AppendLine("10 new GUIDs style N (lowercase)");
            for (int i = 0; i < 10; i++)
            {
                sb.AppendLine(Guid.NewGuid().ToString("N").ToLower());
            }

            sb.AppendLine();
            sb.AppendLine("10 new GUIDs style N (uppercase)");
            for (int i = 0; i < 10; i++)
            {
                sb.AppendLine(Guid.NewGuid().ToString("N").ToUpper());
            }

            RtbManipulate.LogMessage(sb.ToString(), Brushes.Black);
        }


        private void SortAscending_Click(object sender, RoutedEventArgs e)
        {
            RtbManipulate.SortRichTextBox(sortAscending: true);
        }

        private void SortDescending_Click(object sender, RoutedEventArgs e)
        {
            RtbManipulate.SortRichTextBox(sortAscending: false);
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
