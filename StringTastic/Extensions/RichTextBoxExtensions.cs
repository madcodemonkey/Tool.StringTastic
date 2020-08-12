using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace StringTastic
{
    internal delegate void RtbLoggingDelegate(RichTextBox rtb, string message, Brush color);

    public static class RichTextBoxExtensions
    {
        /// <summary>Threadsafe way of clearing Document blocks.</summary>
        public static void Clear(this RichTextBox source)
        {
            if (source.Dispatcher.CheckAccess())
            {
                source.Document.Blocks.Clear();
            }
            else source.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<RichTextBox>(Clear), new[] { source });
        }

        /// <summary>Text retrieval method (not thread safe).</summary>
        public static List<string> ToListOfString(this RichTextBox source)
        {
            var result = new List<string>();

            using (var ms = new MemoryStream())
            {
                var myTextRange = new TextRange(source.Document.ContentStart, source.Document.ContentEnd);
                myTextRange.Save(ms, System.Windows.DataFormats.Text);

                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    while (sr.EndOfStream == false)
                    {
                        result.Add(sr.ReadLine());
                    }
                }
            }
            return result;
        }

        /// <summary>Text retrieval method (not thread safe).</summary>
        public static string ToOneString(this RichTextBox source, bool ignoreBlankLines)
        {
            var sb = new StringBuilder();

            using (var ms = new MemoryStream())
            {
                var myTextRange = new TextRange(source.Document.ContentStart, source.Document.ContentEnd);
                myTextRange.Save(ms, System.Windows.DataFormats.Text);

                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    while (sr.EndOfStream == false)
                    {
                        var theString = sr.ReadLine();
                        if (ignoreBlankLines && string.IsNullOrWhiteSpace(theString))
                        {
                            continue;
                        }

                        sb.Append(theString);
                    }
                }
            }

            return sb.ToString();
        }


        /// <summary>Threadsafe logging method.</summary>
        public static void LogError(this RichTextBox source, Exception ex, Brush color)
        {
            if (source.Dispatcher.CheckAccess())
            {
                LogMessage(source, ex.Message, color);
                LogMessage(source, ex.StackTrace, color);
                if (ex.InnerException != null)
                {
                    LogMessage(source, ex.InnerException.Message, color);
                    LogMessage(source, ex.InnerException.StackTrace, color);
                }
            }
            else source.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new RtbLoggingDelegate(LogMessage), new object[] { source, ex, color });
        }

        /// <summary>Threadsafe logging method.</summary>
        public static void LogMessage(this RichTextBox source, string message, Brush color)
        {
            if (source.Dispatcher.CheckAccess())
            {
                var p = new Paragraph(new Run(message)) { Foreground = color }; 
                source.Document.Blocks.Add(p);
            }
            else source.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new RtbLoggingDelegate(LogMessage), new object[] { source, message, color });
        }
        
        /// <summary>Threadsafe logging method.</summary>
        public static void LogMessage(this RichTextBox source, IEnumerable<string> messages, Brush color)
        {
            if (source.Dispatcher.CheckAccess())
            {
                foreach (var message in messages)
                {
                    LogMessage(source, message, color);
                }
            }
            else source.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new RtbLoggingDelegate(LogMessage), new object[] { source, messages, color });
        }


        public static void TrimItems(this RichTextBox source)
        {
            if (source.Dispatcher.CheckAccess())
            {
                List<string> listOfStrings = source.ToListOfString();

                source.Document.Blocks.Clear();
                
                foreach (var singleItem in listOfStrings)
                {
                    string message = singleItem.Trim();
                    if (string.IsNullOrEmpty(message) == false)
                        source.LogMessage(message, Brushes.Black);
                }
            }
            else source.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<RichTextBox>(TrimItems), new object[] { source });
  
        }
    }
}
