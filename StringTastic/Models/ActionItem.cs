using System.Windows;

namespace StringTastic.Views
{
    public class ActionItem
    {
        public string DisplayText { get; set; }
        public string Tag { get; set; }
        public string IconKey { get; set; }

        public DataTemplate IconTemplate
        {
            get
            {
                if (!string.IsNullOrEmpty(IconKey))
                {
                    return Application.Current.TryFindResource(IconKey) as DataTemplate;
                }
                return null;
            }
        }
    }
}
