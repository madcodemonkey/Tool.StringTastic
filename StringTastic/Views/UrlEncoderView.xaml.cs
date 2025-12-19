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
            string plainText = RtbUrlEncoder.ToOneString(true);
            string encodedString = System.Web.HttpUtility.UrlEncode(plainText);

            RtbUrlEncoder.Clear();
            RtbUrlEncoder.LogMessage(encodedString, Brushes.Black);
        }

        private void DecodeButton_Click(object sender, RoutedEventArgs e)
        {
            string encodedData = RtbUrlEncoder.ToOneString(true);
            string decodedString = System.Web.HttpUtility.UrlDecode(encodedData);

            RtbUrlEncoder.Clear();
            RtbUrlEncoder.LogMessage(decodedString, Brushes.Black);
        }
    }
}
