using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
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

        private void PutUniqueRightItemsInLeftRtbButton_Click(object sender, RoutedEventArgs e)
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


        private void Base64EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            string plainText = RtbManipulate.ToOneString(true);
            
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            string encodedBase64String = System.Convert.ToBase64String(plainTextBytes);

            RtbManipulate.Clear();
            RtbManipulate.LogMessage(encodedBase64String, Brushes.Black);
        }

        private void UrlDecodeButton_Click(object sender, RoutedEventArgs e)
        {
            string encodedData = RtbManipulate.ToOneString(true);
            
            string decodedString = HttpUtility.UrlDecode(encodedData);;

            RtbManipulate.Clear();
            RtbManipulate.LogMessage(decodedString, Brushes.Black);
        }

        private void JwtTokenDecodeButton_Click(object sender, RoutedEventArgs e)
        {
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
                RtbManipulate.LogMessage("Header:", Brushes.Black);
                var part1Object = JObject.Parse(part1);
                RtbManipulate.LogMessage(part1Object.ToString(), Brushes.Black);

                RtbManipulate.LogMessage(" ", Brushes.Black);
                RtbManipulate.LogMessage("Payload:", Brushes.Black);
                var part2Object = JObject.Parse(part2);
                RtbManipulate.LogMessage(part2Object.ToString(), Brushes.Black);


                RtbManipulate.LogMessage(" ", Brushes.Black);
                RtbManipulate.LogMessage("Signature:", Brushes.Black);
                RtbManipulate.LogMessage("[Encoded Signature]", Brushes.Black);
                
                DecodeDate("Payload Issued date (iat)", "iat", part2Object);
                DecodeDate("Payload Expiration date (exp)", "exp", part2Object);
            }
            catch (Exception ex)
            {
                RtbManipulate.Clear();
                RtbManipulate.LogMessage(ex.Message, Brushes.Black);
            }
        }

        private void DecodeDate(string title, string propertyName, JObject part2Object)
        {
            RtbManipulate.LogMessage(" ", Brushes.Black);
            JToken expiration = part2Object[propertyName];

            if (expiration == null || string.IsNullOrWhiteSpace(expiration.ToString()))
            {
                RtbManipulate.LogMessage($"{title} not found.", Brushes.Black);
            }
            else if (int.TryParse(expiration.ToString(), out var exp))
            {
                var startDate = new DateTime(1970, 1, 1);
                var expDate = startDate.AddSeconds(exp);
                RtbManipulate.LogMessage($"{title} = {expDate}", Brushes.Black);
            }
            else
            {
                RtbManipulate.LogMessage($"Unable to parse {title}.", Brushes.Black);
            }

        }


        private void UrlEncodeButton_Click(object sender, RoutedEventArgs e)
        {
            string plainText = RtbManipulate.ToOneString(true);
            
            string encodedString = HttpUtility.UrlEncode(plainText);

            RtbManipulate.Clear();
            RtbManipulate.LogMessage(encodedString, Brushes.Black);
        }

        private void Base64DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            string message;

            try
            {
                string base64EncodedData = RtbManipulate.ToOneString(true);
                message = Base64Decode(base64EncodedData);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            RtbManipulate.Clear();
            RtbManipulate.LogMessage(message, Brushes.Black);
        }

        private string Base64Decode(string base64EncodedData)
        {
            // Sometimes you have to add = on to the end so that you avoid the 
            // "Invalid length for a Base-64 char array" error.
            // https://stackoverflow.com/a/2925959/97803
            int mod4 = base64EncodedData.Length % 4;
            if (mod4 > 0)
            {
                base64EncodedData += new string('=', 4 - mod4);
            }

            var plainTextBytes = System.Convert.FromBase64String(base64EncodedData);
            var result  = System.Text.Encoding.UTF8.GetString(plainTextBytes);
            
            return result;
        }

        private void GenerateGuidsButton_Click(object sender, RoutedEventArgs e)
        {
            RtbManipulate.Clear();
            RtbManipulate.LogMessage("10 new GUIDs style D", Brushes.Black);
            for (int i = 0; i < 10; i++)
            {
                RtbManipulate.LogMessage(Guid.NewGuid().ToString("D").ToLower(), Brushes.Black);
            }

            RtbManipulate.LogMessage(" ", Brushes.Black);
            RtbManipulate.LogMessage("10 new GUIDs style N", Brushes.Black);
            for (int i = 0; i < 10; i++)
            {
                RtbManipulate.LogMessage(Guid.NewGuid().ToString("N").ToLower(), Brushes.Black);
            }
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
