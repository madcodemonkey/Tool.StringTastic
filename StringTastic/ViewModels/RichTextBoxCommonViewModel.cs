using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using StringTastic.Annotations;
using DataFormats = System.Windows.DataFormats;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace StringTastic.ViewModels
{
    public class RichTextBoxCommonViewModel : DependencyObject, INotifyPropertyChanged
    {
        #region RichTextBox - LoadFileContents
        private ICommand _loadFileContentsCommand;
        public ICommand LoadFileContentsCommand
        {
            get { return _loadFileContentsCommand ?? (_loadFileContentsCommand = new RelayCommand(LoadFileContents)); }
        }
        public void LoadFileContents(object obj)
        {
            var myRtb = CastAsRichTextBoxAndAssertIfItIsNotRichTextBox(obj);

            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                myRtb.Document.Blocks.Clear();

                using (var fs = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    var tr = new TextRange(myRtb.Document.ContentStart, myRtb.Document.ContentEnd);
                    tr.Load(fs, DataFormats.Text);
                }
            }
        }
        #endregion

        #region RichTextBox - LoadFileNames
        public static readonly DependencyProperty WhenLoadingFileNamesUseFullPathProperty =
            DependencyProperty.Register("WhenLoadingFileNamesUseFullPath", typeof(bool),
            typeof(RichTextBoxCommonViewModel), new PropertyMetadata(default(bool)));
        public bool WhenLoadingFileNamesUseFullPath
        {
            get { return (bool)GetValue(WhenLoadingFileNamesUseFullPathProperty); }
            set { SetValue(WhenLoadingFileNamesUseFullPathProperty, value); }
        }
        
        private ICommand _loadFileNamesCommand;
        public ICommand LoadFileNamesCommand
        {
            get { return _loadFileNamesCommand ?? (_loadFileNamesCommand = new RelayCommand(LoadFileNames)); }
        }
        public void LoadFileNames(object obj)
        {
            var myRtb = CastAsRichTextBoxAndAssertIfItIsNotRichTextBox(obj);

            var dialog = new OpenFileDialog {Multiselect = true};
            dialog.Title = "Select the individual files";
            if (dialog.ShowDialog() == true)
            {
                var data = new List<string>();

                foreach (var fileName in dialog.FileNames)
                {
                    data.Add(WhenLoadingFileNamesUseFullPath == false ? Path.GetFileName(fileName) : fileName);
                }

                myRtb.LogMessage(data, Brushes.Black);
            }
        }
        #endregion

        #region RichTextBox - LoadFolderNames
        public static readonly DependencyProperty WhenLoadingFolderNamesUseFullPathProperty =
            DependencyProperty.Register("WhenLoadingFolderNamesUseFullPath", typeof(bool),
            typeof(RichTextBoxCommonViewModel), new PropertyMetadata(default(bool)));
        public bool WhenLoadingFolderNamesUseFullPath
        {
            get { return (bool)GetValue(WhenLoadingFileNamesUseFullPathProperty); }
            set { SetValue(WhenLoadingFileNamesUseFullPathProperty, value); }
        }

        private ICommand _loadFolderNamesCommand;
        public ICommand LoadFolderNamesCommand
        {
            get { return _loadFolderNamesCommand ?? (_loadFolderNamesCommand = new RelayCommand(LoadFolderNames)); }
        }
        public void LoadFolderNames(object obj)
        {
            var myRtb = CastAsRichTextBoxAndAssertIfItIsNotRichTextBox(obj);

            var dialog = new FolderBrowserDialog();
            dialog.Description = "Select the parent directory";
          
            if (dialog.ShowDialog() ==  DialogResult.OK)
            {
                var data = new List<string>();
                
                foreach (var folderName in Directory.GetDirectories(dialog.SelectedPath))
                {
                    if (WhenLoadingFileNamesUseFullPath)
                        data.Add(folderName);
                    else
                    {
                       //data.Add(Path.GetFileName(folderName));
                        data.Add(new DirectoryInfo(folderName).Name);
                        // data.Add(folderName.Substring(folderName.LastIndexOf(@"\", StringComparison.Ordinal) + 1));
                    }
                }

                myRtb.LogMessage(data, Brushes.Black);
            }
        }
        #endregion

        #region RichTextBox - Clear
        private ICommand _clearRichTextBoxCommand;
        public ICommand ClearRichTextBoxCommand
        {
            get { return _clearRichTextBoxCommand ?? (_clearRichTextBoxCommand = new RelayCommand(ClearRichTextBox)); }
        }
        private void ClearRichTextBox(object obj)
        {
            var myRtb = CastAsRichTextBoxAndAssertIfItIsNotRichTextBox(obj);
            myRtb.Clear();
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Non-Public
        private static RichTextBox CastAsRichTextBoxAndAssertIfItIsNotRichTextBox(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var myRtb = obj as RichTextBox;
            if (myRtb == null)
                throw new ArgumentException("Expecting a RichTextBox, but received the following type: " + obj.GetType());
            return myRtb;
        }
    

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
