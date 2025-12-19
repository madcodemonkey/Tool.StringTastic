using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StringTastic.Views
{
    public partial class GenerateGuidView : UserControl
    {
        public GenerateGuidView()
        {
            InitializeComponent();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            RtbGuids.Clear();

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

            RtbGuids.LogMessage(sb.ToString(), Brushes.Black);
        }
    }
}
