using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic.Views
{
    public partial class UrlEncoderView : UserControl
    {
        public UrlEncoderView()
        {
            InitializeComponent();
        }

        private void EncodeButton_Click(object sender, RoutedEventArgs e)
        {
            string plainText = RtbInput.ToOneString(true);
            string encodedString = System.Web.HttpUtility.UrlEncode(plainText);

            RtbOutput.Clear();
            RtbOutput.LogMessage(encodedString, Brushes.Black);
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            string encodedData = RtbInput.ToOneString(true);
            string decodedString = System.Web.HttpUtility.UrlDecode(encodedData);

            RtbOutput.Clear();
            RtbOutput.LogMessage(decodedString, Brushes.Black);
        }
    }
}
