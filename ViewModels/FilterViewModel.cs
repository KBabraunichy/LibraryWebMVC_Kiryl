namespace LibraryWebMVC.ViewModels
{
    public class FilterViewModel
    {
        public string SelectedFirstName { get; }
        public string SelectedBookName { get; }

        public FilterViewModel(string selectedFirstName, string selectedBookName)
        {
            SelectedFirstName = selectedFirstName;
            SelectedBookName = selectedBookName;
        }
    }
}
