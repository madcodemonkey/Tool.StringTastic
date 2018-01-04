using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace StringTastic.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            RichTextBoxCommon = new RichTextBoxCommonViewModel();
        }

        public RichTextBoxCommonViewModel RichTextBoxCommon { get; set; }


        
        protected void OnPropertyChanged(string propertyname)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyname));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}