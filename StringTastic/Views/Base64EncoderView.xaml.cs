using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic.Views
{
    public partial class Base64EncoderView : UserControl
    {
        public Base64EncoderView()
        {
            InitializeComponent();
        }

        private void TrimButton_Click(object sender, RoutedEventArgs e)
        {
            RtbInput.TrimLines();
        }

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            string plainText = RtbInput.ToOneString(true);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            string encodedBase64String = System.Convert.ToBase64String(plainTextBytes);

            RtbOutput.Clear();
            RtbOutput.LogMessage(encodedBase64String, Brushes.Black);
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            string message;

            try
            {
                string base64EncodedData = RtbInput.ToOneString(true);
                message = Base64Decode(base64EncodedData);
                RtbOutput.Clear();
            }
            catch (Exception ex)
            {
                RtbOutput.Clear();
                RtbOutput.LogMessage("----------------", Brushes.Black);
                message = ex.Message;
            }

            RtbOutput.LogMessage(message, Brushes.Black);
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
