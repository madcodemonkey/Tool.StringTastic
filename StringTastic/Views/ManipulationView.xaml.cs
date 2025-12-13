using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace StringTastic.Views
{
    public partial class ManipulationView : UserControl
    {
        public ManipulationView()
        {
            InitializeComponent();
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

            string decodedString = System.Web.HttpUtility.UrlDecode(encodedData); ;

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
                RtbManipulate.LogMessage("----------------", Brushes.Black);
                RtbManipulate.LogMessage(ex.Message, Brushes.Black);
            }
        }

        private void DecodeDate(string title, string propertyName, JObject part2Object)
        {
            RtbManipulate.LogMessage(" ", Brushes.Black);
            var expiration = part2Object[propertyName];

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

            string encodedString = System.Web.HttpUtility.UrlEncode(plainText);

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
                RtbManipulate.Clear();
            }
            catch (Exception ex)
            {
                RtbManipulate.LogMessage("----------------", Brushes.Black);
                message = ex.Message;
            }

            RtbManipulate.LogMessage(message, Brushes.Black);
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
            RtbManipulate.LogMessage("10 new GUIDs style D (lowercase)", Brushes.Black);
            for (int i = 0; i < 10; i++)
            {
                RtbManipulate.LogMessage(Guid.NewGuid().ToString("D").ToLower(), Brushes.Black);
            }

            RtbManipulate.LogMessage(" ", Brushes.Black);
            RtbManipulate.LogMessage("10 new GUIDs style D (uppercase)", Brushes.Black);
            for (int i = 0; i < 10; i++)
            {
                RtbManipulate.LogMessage(Guid.NewGuid().ToString("D").ToUpper(), Brushes.Black);
            }

            RtbManipulate.LogMessage(" ", Brushes.Black);
            RtbManipulate.LogMessage("10 new GUIDs style N (lowercase)", Brushes.Black);
            for (int i = 0; i < 10; i++)
            {
                RtbManipulate.LogMessage(Guid.NewGuid().ToString("N").ToLower(), Brushes.Black);
            }

            RtbManipulate.LogMessage(" ", Brushes.Black);
            RtbManipulate.LogMessage("10 new GUIDs style N (uppercase)", Brushes.Black);
            for (int i = 0; i < 10; i++)
            {
                RtbManipulate.LogMessage(Guid.NewGuid().ToString("N").ToUpper(), Brushes.Black);
            }
        }
    }
}
