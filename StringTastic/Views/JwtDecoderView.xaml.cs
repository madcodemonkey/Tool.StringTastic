using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic.Views
{
    public partial class JwtDecoderView : UserControl
    {
        public JwtDecoderView()
        {
            InitializeComponent();
        }

        private void TrimButton_Click(object sender, RoutedEventArgs e)
        {
            RtbInput.TrimLines();
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                string encodedData = RtbInput.ToOneString(true).Trim();

                var encodedParts = encodedData.Split('.');

                if (encodedParts.Length != 3)
                {
                    RtbOutput.Clear();
                    RtbOutput.LogMessage("Invalid JWT token.  It should have three distinct parts!", Brushes.Black);
                    return;
                }

                string part1 = Base64Decode(encodedParts[0]);
                string part2 = Base64Decode(encodedParts[1]);

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

                RtbOutput.Clear();
                RtbOutput.LogMessage(sb.ToString(), Brushes.Black);
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("----------------");
                sb.AppendLine(ex.Message);
                
                RtbOutput.Clear();
                RtbOutput.LogMessage(sb.ToString(), Brushes.Black);
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
    }
}
