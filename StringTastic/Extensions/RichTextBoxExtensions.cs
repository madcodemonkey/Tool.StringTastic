using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Sorts the strings in the listbox.
        /// </summary>
        public static void SortRichTextBox(this RichTextBox rtb, bool sortAscending)
        {
            List<string> listOfStrings = rtb.ToListOfString();
            rtb.Clear();

            var items = sortAscending ?
                listOfStrings.OrderBy(item => item).ToList() :
                listOfStrings.OrderByDescending(item => item).ToList();

            rtb.LogMessage(items, Brushes.Black);
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

        /// <summary>
        /// Trims each item in the RichTextBox.
        /// </summary>
        public static void TrimLines(this RichTextBox source)
        {
            List<string> listOfStrings = source.ToListOfString();

            source.Clear();

            StringBuilder sb = new StringBuilder();

            foreach (var singleItem in listOfStrings)
            {
                string message = singleItem.Trim();
                if (string.IsNullOrEmpty(message) == false)
                    sb.AppendLine(message);
            }

            source.LogMessage(sb.ToString(), Brushes.Black);
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
        public static void LogMessage(this RichTextBox source, IList<string> messages, Brush color)
        {
            if (source.Dispatcher.CheckAccess())
            {
                StringBuilder sb = new StringBuilder();

                foreach (var message in messages)
                {
                    sb.AppendLine(message);
                }

                source.LogMessage(sb.ToString(), color);
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
